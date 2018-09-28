using Akka.Actor;
using Akka.IO;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using NeoSharp.Core.Extensions;

namespace NeoSharp.Core.Network.P2p
{
    public abstract class Peer : UntypedActor
    {
        public class Start { public int Port; }
        public class Peers { public IEnumerable<IPEndPoint> EndPoints; }
        public class Connect { public IPEndPoint EndPoint; }
        private class Timer { }

        private const int MaxConnectionsPerAddress = 3;

        private static readonly IActorRef TcpManager = Context.System.Tcp();
        private IActorRef _tcpListener;
        private ICancelable _timer;
        protected ActorSelection Connections => Context.ActorSelection("connection_*");

        private static readonly HashSet<IPAddress> LocalAddresses = new HashSet<IPAddress>();
        private readonly Dictionary<IPAddress, int> _connectedAddresses = new Dictionary<IPAddress, int>();
        protected readonly ConcurrentDictionary<IActorRef, IPEndPoint> ConnectedPeers = new ConcurrentDictionary<IActorRef, IPEndPoint>();
        protected ImmutableHashSet<IPEndPoint> UnconnectedPeers = ImmutableHashSet<IPEndPoint>.Empty;

        public int ListenerPort { get; private set; }
        protected abstract int ConnectedMax { get; }
        protected abstract int UnconnectedMax { get; }

        static Peer()
        {
            LocalAddresses.UnionWith(NetworkInterface.GetAllNetworkInterfaces().SelectMany(p => p.GetIPProperties().UnicastAddresses).Select(p => p.Address.Unmap()));
        }

        protected void AddPeers(IEnumerable<IPEndPoint> peers)
        {
            if (UnconnectedPeers.Count < UnconnectedMax)
            {
                peers = peers.Where(p => p.Port != ListenerPort || !LocalAddresses.Contains(p.Address));
                ImmutableInterlocked.Update(ref UnconnectedPeers, p => p.Union(peers));
            }
        }

        protected void ConnectToPeer(IPEndPoint endPoint)
        {
            endPoint = endPoint.Unmap();
            if (endPoint.Port == ListenerPort && LocalAddresses.Contains(endPoint.Address)) return;
            if (_connectedAddresses.TryGetValue(endPoint.Address, out var count) && count >= MaxConnectionsPerAddress)
                return;
            if (ConnectedPeers.Values.Contains(endPoint)) return;
            TcpManager.Tell(new Akka.IO.Tcp.Connect(endPoint));
        }

        private static bool IsIntranetAddress(IPAddress address)
        {
            var data = address.MapToIPv4().GetAddressBytes();
            Array.Reverse(data);
            var value = data.ToUInt32();
            return (value & 0xff000000) == 0x0a000000 || (value & 0xff000000) == 0x7f000000 || (value & 0xfff00000) == 0xac100000 || (value & 0xffff0000) == 0xc0a80000 || (value & 0xffff0000) == 0xa9fe0000;
        }

        protected abstract void NeedMorePeers(int count);

        protected override void OnReceive(object message)
        {
            switch (message)
            {
                case Start start:
                    OnStart(start.Port);
                    break;
                case Timer _:
                    OnTimer();
                    break;
                case Peers peers:
                    AddPeers(peers.EndPoints);
                    break;
                case Connect connect:
                    ConnectToPeer(connect.EndPoint);
                    break;
                case Akka.IO.Tcp.Connected connected:
                    OnTcpConnected(((IPEndPoint)connected.RemoteAddress).Unmap(), ((IPEndPoint)connected.LocalAddress).Unmap());
                    break;
                case Akka.IO.Tcp.Bound _:
                    _tcpListener = Sender;
                    break;
                case Akka.IO.Tcp.CommandFailed _:
                    break;
                case Terminated terminated:
                    OnTerminated(terminated.ActorRef);
                    break;
            }
        }

        private void OnStart(int port)
        {
            ListenerPort = port;
            _timer = Context.System.Scheduler.ScheduleTellRepeatedlyCancelable(0, 5000, Context.Self, new Timer(), ActorRefs.NoSender);
            if (port > 0
                && LocalAddresses.All(p => !p.IsIPv4MappedToIPv6 || IsIntranetAddress(p))
                && UPnP.Discover())
            {
                try
                {
                    LocalAddresses.Add(UPnP.GetExternalIP());
                    if (port > 0)
                        UPnP.ForwardPort(port, ProtocolType.Tcp, "NEO");
                }
                catch
                {
                    // ignored
                }
            }
            if (port > 0)
            {
                TcpManager.Tell(new Akka.IO.Tcp.Bind(Self, new IPEndPoint(IPAddress.Any, port), options: new[] { new Inet.SO.ReuseAddress(true) }));
            }
        }

        private void OnTcpConnected(IPEndPoint remote, IPEndPoint local)
        {
            _connectedAddresses.TryGetValue(remote.Address, out var count);
            if (count >= MaxConnectionsPerAddress)
            {
                Sender.Tell(Akka.IO.Tcp.Abort.Instance);
            }
            else
            {
                _connectedAddresses[remote.Address] = count + 1;
                var connection = Context.ActorOf(ProtocolProps(Sender, remote, local), $"connection_{Guid.NewGuid()}");
                Context.Watch(connection);
                Sender.Tell(new Akka.IO.Tcp.Register(connection));
                ConnectedPeers.TryAdd(connection, remote);
            }
        }

        private void OnTerminated(IActorRef actorRef)
        {
            if (ConnectedPeers.TryRemove(actorRef, out var endPoint))
            {
                _connectedAddresses.TryGetValue(endPoint.Address, out var count);
                if (count > 0) count--;
                if (count == 0)
                    _connectedAddresses.Remove(endPoint.Address);
                else
                    _connectedAddresses[endPoint.Address] = count;
            }
        }

        private void OnTimer()
        {
            if (ConnectedPeers.Count >= ConnectedMax) return;
            if (UnconnectedPeers.Count == 0)
                NeedMorePeers(ConnectedMax - ConnectedPeers.Count);
            var endpoints = UnconnectedPeers.Take(ConnectedMax - ConnectedPeers.Count).ToArray();
            ImmutableInterlocked.Update(ref UnconnectedPeers, p => p.Except(endpoints));
            foreach (var endpoint in endpoints)
            {
                ConnectToPeer(endpoint);
            }
        }

        protected override void PostStop()
        {
            _timer.CancelIfNotNull();
            _tcpListener?.Tell(Akka.IO.Tcp.Unbind.Instance);
            base.PostStop();
        }

        protected abstract Props ProtocolProps(object connection, IPEndPoint remote, IPEndPoint local);
    }
}