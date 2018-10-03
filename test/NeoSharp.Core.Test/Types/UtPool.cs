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
    public class UtPool : TestBase
    {
        [TestMethod]
        public void Test_Pool_SamePush()
        {
            var pool = new Pool<UInt256, UInt256>(PoolMaxBehaviour.RemoveFromEnd, 3, x => x, (a, b) => a.CompareTo(b));

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
        public void Test_Pool_PeekFirstOrDefault()
        {
            var pool = new Pool<UInt256, UInt256>(PoolMaxBehaviour.RemoveFromEnd, 3, x => x, (a, b) => a.CompareTo(b));

            var add = new UInt256[]
            {
                new UInt256("3A259DBA256600620C6C91094F3A300B30F0CBAECEE19C6114DEFFD3288957D7".HexToBytes()),
                new UInt256("2A259DBA256600620C6C91094F3A300B30F0CBAECEE19C6114DEFFD3288957D7".HexToBytes()),
                new UInt256("1A259DBA256600620C6C91094F3A300B30F0CBAECEE19C6114DEFFD3288957D7".HexToBytes()),
            };

            foreach (var t in add) pool.Push(t).Should().BeTrue();

            pool.PeekFirstOrDefault().Should().Be(add[2]);
            pool.Count.Should().Be(3);
        }

        [TestMethod]
        public void Test_Pool_PopFirstOrDefault()
        {
            var pool = new Pool<UInt256, UInt256>(PoolMaxBehaviour.RemoveFromEnd, 3, x => x, (a, b) => a.CompareTo(b));

            var add = new UInt256[]
            {
                new UInt256("3A259DBA256600620C6C91094F3A300B30F0CBAECEE19C6114DEFFD3288957D7".HexToBytes()),
                new UInt256("2A259DBA256600620C6C91094F3A300B30F0CBAECEE19C6114DEFFD3288957D7".HexToBytes()),
                new UInt256("1A259DBA256600620C6C91094F3A300B30F0CBAECEE19C6114DEFFD3288957D7".HexToBytes()),
            };

            foreach (var t in add) pool.Push(t).Should().BeTrue();

            pool.PopFirstOrDefault().Should().Be(add[2]);
            pool.Count.Should().Be(2);
        }

        [TestMethod]
        public void Test_Pool_Remove()
        {
            var pool = new Pool<UInt256, UInt256>(PoolMaxBehaviour.RemoveFromEnd, 3, x => x, (a, b) => a.CompareTo(b));

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
        public void Test_Pool_RemoveFromEnd()
        {
            var pool = new Pool<UInt256, UInt256>(PoolMaxBehaviour.RemoveFromEnd, 3, x => x, (a, b) => a.CompareTo(b));

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
                peek[x].Should().Be(add[x]);

            pool.Clear();
            pool.Count.Should().Be(0);
        }

        [TestMethod]
        public void Test_Pool_DontAllowMore()
        {
            var pool = new Pool<UInt256, UInt256>(PoolMaxBehaviour.DontAllowMore, 3, x => x, (a, b) => a.CompareTo(b));

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

            UInt256[] peek;
            for (int xx = 0; xx < add.Length; xx++)
            {
                peek = pool.Peek(xx);

                for (var x = 0; x < peek.Length; x++)
                    peek[x].Should().Be(add[x]);
            }

            peek = pool.Pop(add.Length);

            for (var x = 0; x < peek.Length; x++)
                peek[x].Should().Be(add[x]);

            pool.Clear();
            pool.Count.Should().Be(0);
        }
    }
}