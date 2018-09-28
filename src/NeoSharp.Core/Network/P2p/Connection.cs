using System;
using System.Net;
using Akka.Actor;
using Akka.IO;

namespace NeoSharp.Core.Network.P2p
{
    public abstract class Connection : UntypedActor
    {
        internal class Timer { public static Timer Instance = new Timer(); }
        internal class Ack : Akka.IO.Tcp.Event { public static Ack Instance = new Ack(); }

        public IPEndPoint Remote { get; }
        public IPEndPoint Local { get; }
        public abstract int ListenerPort { get; }

        private ICancelable _timer;
        private readonly IActorRef _tcp;
        private bool _disconnected;

        protected Connection(IActorRef tcp, IPEndPoint remote, IPEndPoint local)
        {
            _tcp = tcp ?? throw new ArgumentNullException(nameof(tcp));
            Remote = remote ?? throw new ArgumentNullException(nameof(remote));
            Local = local ?? throw new ArgumentNullException(nameof(local));
            _timer = Context.System.Scheduler.ScheduleTellOnceCancelable(TimeSpan.FromSeconds(10), Self, Timer.Instance, ActorRefs.NoSender);
        }

        public void Disconnect(bool abort = false)
        {
            _disconnected = true;
            _tcp.Tell(abort ? (Akka.IO.Tcp.CloseCommand)Akka.IO.Tcp.Abort.Instance : Akka.IO.Tcp.Close.Instance);
            Context.Stop(Self);
        }

        protected virtual void OnAck()
        {
        }

        protected abstract void OnData(ByteString data);

        protected override void OnReceive(object message)
        {
            switch (message)
            {
                case Timer _:
                    Disconnect(true);
                    break;
                case Ack _:
                    OnAck();
                    break;
                case Akka.IO.Tcp.Received received:
                    OnReceived(received.Data);
                    break;
                case Akka.IO.Tcp.ConnectionClosed _:
                    Context.Stop(Self);
                    break;
            }
        }

        private void OnReceived(ByteString data)
        {
            _timer.CancelIfNotNull();
            _timer = Context.System.Scheduler.ScheduleTellOnceCancelable(TimeSpan.FromMinutes(1), Self, Timer.Instance, ActorRefs.NoSender);

            try
            {
                OnData(data);
            }
            catch
            {
                Disconnect(true);
            }
        }

        protected override void PostStop()
        {
            if (!_disconnected) _tcp.Tell(Akka.IO.Tcp.Close.Instance);
            _timer.CancelIfNotNull();
            base.PostStop();
        }

        protected void SendData(ByteString data)
        {
            _tcp.Tell(Akka.IO.Tcp.Write.Create(data, Ack.Instance));
        }
    }
}