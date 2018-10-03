using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NeoSharp.Core.Types;
using NeoSharp.TestHelpers;
using FluentAssertions;
using NeoSharp.Types;
using NeoSharp.Types.ExtensionMethods;

namespace NeoSharp.Core.Test.Types
{
    [TestClass]
    public class UtStampedPool : TestBase
    {

        [TestMethod]
        public void Test_StampedPool_StampToString()
        {
            var pool = new StampedPool<UInt256, UInt256>(PoolMaxBehaviour.RemoveFromEnd, 3, x => x.Value, (x, y) => x.Date.CompareTo(y.Date));

            var add = new UInt256("3A259DBA256600620C6C91094F3A300B30F0CBAECEE19C6114DEFFD3288957D7".HexToBytes());

            pool.Push(add).Should().BeTrue();
            pool.Count.Should().Be(1);

            pool.Peek()[0].ToString().Should().Be(add.ToString());
        }

        [TestMethod]
        public void Test_StampedPool_SamePush()
        {
            var pool = new StampedPool<UInt256, UInt256>(PoolMaxBehaviour.RemoveFromEnd, 3, x => x.Value, (x, y) => x.Date.CompareTo(y.Date));

            var add = new UInt256[]
            {
                new UInt256("3A259DBA256600620C6C91094F3A300B30F0CBAECEE19C6114DEFFD3288957D7".HexToBytes()),
                new UInt256("3A259DBA256600620C6C91094F3A300B30F0CBAECEE19C6114DEFFD3288957D7".HexToBytes()),
            };

            pool.Push(add[0]).Should().BeTrue();
            pool.Push(add[1]).Should().BeFalse();

            pool.Count.Should().Be(1);
        }

        [TestMethod]
        public void Test_StampedPool_PeekFirstOrDefault()
        {
            var pool = new StampedPool<UInt256, UInt256>(PoolMaxBehaviour.RemoveFromEnd, 3, x => x.Value, (x, y) => x.Value.CompareTo(y.Value));

            var add = new UInt256[]
            {
                new UInt256("3A259DBA256600620C6C91094F3A300B30F0CBAECEE19C6114DEFFD3288957D7".HexToBytes()),
                new UInt256("2A259DBA256600620C6C91094F3A300B30F0CBAECEE19C6114DEFFD3288957D7".HexToBytes()),
                new UInt256("1A259DBA256600620C6C91094F3A300B30F0CBAECEE19C6114DEFFD3288957D7".HexToBytes()),
            };

            foreach (var t in add) pool.Push(t).Should().BeTrue();

            pool.PeekFirstOrDefault().Value.Should().Be(add[2]);
            pool.Count.Should().Be(3);
        }

        [TestMethod]
        public void Test_StampedPool_PopFirstOrDefault()
        {
            var pool = new StampedPool<UInt256, UInt256>(PoolMaxBehaviour.RemoveFromEnd, 3, x => x.Value, (x, y) => x.Value.CompareTo(y.Value));

            var add = new UInt256[]
            {
                new UInt256("3A259DBA256600620C6C91094F3A300B30F0CBAECEE19C6114DEFFD3288957D7".HexToBytes()),
                new UInt256("2A259DBA256600620C6C91094F3A300B30F0CBAECEE19C6114DEFFD3288957D7".HexToBytes()),
                new UInt256("1A259DBA256600620C6C91094F3A300B30F0CBAECEE19C6114DEFFD3288957D7".HexToBytes()),
            };

            foreach (var t in add) pool.Push(t).Should().BeTrue();

            pool.PopFirstOrDefault().Value.Should().Be(add[2]);
            pool.Count.Should().Be(2);
        }

        [TestMethod]
        public void Test_StampedPool_Remove()
        {
            var pool = new StampedPool<UInt256, UInt256>(PoolMaxBehaviour.RemoveFromEnd, 3, x => x.Value, (x, y) => x.Date.CompareTo(y.Date));

            var add = new UInt256[]
            {
                new UInt256("3A259DBA256600620C6C91094F3A300B30F0CBAECEE19C6114DEFFD3288957D7".HexToBytes()),
                new UInt256("2A259DBA256600620C6C91094F3A300B30F0CBAECEE19C6114DEFFD3288957D7".HexToBytes()),
                new UInt256("1A259DBA256600620C6C91094F3A300B30F0CBAECEE19C6114DEFFD3288957D7".HexToBytes()),
            };

            foreach (var t in add) pool.Push(t).Should().BeTrue();

            pool.Remove(UInt256.Zero).Should().BeFalse();
            pool.Remove(add[0]).Should().BeTrue();
            pool.Remove(add[1]).Should().BeTrue();
            pool.Remove(add[2]).Should().BeTrue();

            pool.Count.Should().Be(0);
        }

        [TestMethod]
        public void Test_StampedPool_RemoveFromEnd()
        {
            var pool = new StampedPool<UInt256, UInt256>(PoolMaxBehaviour.RemoveFromEnd, 3, x => x.Value, (x, y) => x.Value.CompareTo(y.Value));

            pool.Behaviour.Should().Be(PoolMaxBehaviour.RemoveFromEnd);
            pool.Max.Should().Be(3);
            pool.Count.Should().Be(0);

            var add = new UInt256[]
            {
                new UInt256("3A259DBA256600620C6C91094F3A300B30F0CBAECEE19C6114DEFFD3288957D7".HexToBytes()),
                new UInt256("2A259DBA256600620C6C91094F3A300B30F0CBAECEE19C6114DEFFD3288957D7".HexToBytes()),
                new UInt256("1A259DBA256600620C6C91094F3A300B30F0CBAECEE19C6114DEFFD3288957D7".HexToBytes()),
            };

            foreach (var t in add) pool.Push(t).Should().BeTrue();

            add = new UInt256[]
            {
                new UInt256("03259DBA256600620C6C91094F3A300B30F0CBAECEE19C6114DEFFD3288957D7".HexToBytes()),
                new UInt256("02259DBA256600620C6C91094F3A300B30F0CBAECEE19C6114DEFFD3288957D7".HexToBytes()),
                new UInt256("01259DBA256600620C6C91094F3A300B30F0CBAECEE19C6114DEFFD3288957D7".HexToBytes()),
            };

            pool.Push(add[0]).Should().BeTrue();
            pool.Push(add[1]).Should().BeTrue();
            pool.Push(add[2]).Should().BeTrue();

            // Test order

            Array.Reverse(add);

            var peek = pool.Pop(add.Length);

            for (var x = 0; x < peek.Length; x++)
            {
                peek[x].Value.Should().Be(add[x]);
                if (x < peek.Length - 1)
                    (peek[x].Date >= peek[x + 1].Date).Should().BeTrue();
            }

            pool.Clear();
            pool.Count.Should().Be(0);
        }

        [TestMethod]
        public void Test_StampedPool_DontAllowMore()
        {
            var pool = new StampedPool<UInt256, UInt256>(PoolMaxBehaviour.DontAllowMore, 3, x => x.Value, (x, y) => x.Value.CompareTo(y.Value));

            pool.Behaviour.Should().Be(PoolMaxBehaviour.DontAllowMore);
            pool.Max.Should().Be(3);
            pool.Count.Should().Be(0);

            var add = new UInt256[]
            {
                new UInt256("3A259DBA256600620C6C91094F3A300B30F0CBAECEE19C6114DEFFD3288957D7".HexToBytes()),
                new UInt256("2A259DBA256600620C6C91094F3A300B30F0CBAECEE19C6114DEFFD3288957D7".HexToBytes()),
                new UInt256("1A259DBA256600620C6C91094F3A300B30F0CBAECEE19C6114DEFFD3288957D7".HexToBytes()),
            };

            foreach (var t in add) pool.Push(t).Should().BeTrue();
            foreach (var t in add) pool.Push(t).Should().BeFalse();

            // Test order

            Array.Reverse(add);

            // Check

            Stamp<UInt256>[] peek;
            for (int xx = 0; xx < add.Length; xx++)
            {
                peek = pool.Peek(xx);

                for (var x = 0; x < peek.Length; x++)
                {
                    peek[x].Value.Should().Be(add[x]);
                    if (x < peek.Length - 1)
                        (peek[x].Date >= peek[x + 1].Date).Should().BeTrue();
                }
            }

            peek = pool.Pop(add.Length);

            for (var x = 0; x < peek.Length; x++)
            {
                peek[x].Value.Should().Be(add[x]);
                if (x < peek.Length - 1)
                    (peek[x].Date >= peek[x + 1].Date).Should().BeTrue();
            }

            pool.Clear();
            pool.Count.Should().Be(0);
        }
    }
}
