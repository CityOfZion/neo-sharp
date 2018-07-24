using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NeoSharp.Core.Extensions;
using NeoSharp.Core.Types;
using NeoSharp.TestHelpers;

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

            Assert.IsTrue(pool.Push(add));
            Assert.AreEqual(1, pool.Count);

            Assert.AreEqual(add.ToString(), pool.Peek()[0].ToString());
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

            Assert.IsTrue(pool.Push(add[0]));
            Assert.IsFalse(pool.Push(add[1]));

            Assert.AreEqual(1, pool.Count);
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

            foreach (var t in add) Assert.IsTrue(pool.Push(t));

            Assert.AreEqual(pool.PeekFirstOrDefault().Value, add[2]);
            Assert.AreEqual(3, pool.Count);
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

            foreach (var t in add) Assert.IsTrue(pool.Push(t));

            Assert.AreEqual(pool.PopFirstOrDefault().Value, add[2]);
            Assert.AreEqual(2, pool.Count);
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

            foreach (var t in add) Assert.IsTrue(pool.Push(t));

            Assert.IsFalse(pool.Remove(UInt256.Zero));
            Assert.IsTrue(pool.Remove(add[0]));
            Assert.IsTrue(pool.Remove(add[1]));
            Assert.IsTrue(pool.Remove(add[2]));

            Assert.AreEqual(0, pool.Count);
        }

        [TestMethod]
        public void Test_StampedPool_RemoveFromEnd()
        {
            var pool = new StampedPool<UInt256, UInt256>(PoolMaxBehaviour.RemoveFromEnd, 3, x => x.Value, (x, y) => x.Value.CompareTo(y.Value));

            Assert.AreEqual(PoolMaxBehaviour.RemoveFromEnd, pool.Behaviour);
            Assert.AreEqual(3, pool.Max);
            Assert.AreEqual(0, pool.Count);

            var add = new UInt256[]
            {
                new UInt256("3A259DBA256600620C6C91094F3A300B30F0CBAECEE19C6114DEFFD3288957D7".HexToBytes()),
                new UInt256("2A259DBA256600620C6C91094F3A300B30F0CBAECEE19C6114DEFFD3288957D7".HexToBytes()),
                new UInt256("1A259DBA256600620C6C91094F3A300B30F0CBAECEE19C6114DEFFD3288957D7".HexToBytes()),
            };

            foreach (var t in add) Assert.IsTrue(pool.Push(t));

            add = new UInt256[]
            {
                new UInt256("03259DBA256600620C6C91094F3A300B30F0CBAECEE19C6114DEFFD3288957D7".HexToBytes()),
                new UInt256("02259DBA256600620C6C91094F3A300B30F0CBAECEE19C6114DEFFD3288957D7".HexToBytes()),
                new UInt256("01259DBA256600620C6C91094F3A300B30F0CBAECEE19C6114DEFFD3288957D7".HexToBytes()),
            };

            Assert.IsTrue(pool.Push(add[0]));
            Assert.IsTrue(pool.Push(add[1]));
            Assert.IsTrue(pool.Push(add[2]));

            // Test order

            Array.Reverse(add);

            var peek = pool.Pop(add.Length);

            for (var x = 0; x < peek.Length; x++)
            {
                Assert.AreEqual(add[x], peek[x].Value);
                if (x < peek.Length - 1)
                    Assert.IsTrue(peek[x].Date >= peek[x + 1].Date);
            }

            pool.Clear();
            Assert.AreEqual(0, pool.Count);
        }

        [TestMethod]
        public void Test_StampedPool_DontAllowMore()
        {
            var pool = new StampedPool<UInt256, UInt256>(PoolMaxBehaviour.DontAllowMore, 3, x => x.Value, (x, y) => x.Value.CompareTo(y.Value));

            Assert.AreEqual(PoolMaxBehaviour.DontAllowMore, pool.Behaviour);
            Assert.AreEqual(3, pool.Max);
            Assert.AreEqual(0, pool.Count);

            var add = new UInt256[]
            {
                new UInt256("3A259DBA256600620C6C91094F3A300B30F0CBAECEE19C6114DEFFD3288957D7".HexToBytes()),
                new UInt256("2A259DBA256600620C6C91094F3A300B30F0CBAECEE19C6114DEFFD3288957D7".HexToBytes()),
                new UInt256("1A259DBA256600620C6C91094F3A300B30F0CBAECEE19C6114DEFFD3288957D7".HexToBytes()),
            };

            foreach (var t in add) Assert.IsTrue(pool.Push(t));
            foreach (var t in add) Assert.IsFalse(pool.Push(t));

            // Test order

            Array.Reverse(add);

            // Check

            Stamp<UInt256>[] peek;
            for (int xx = 0; xx < add.Length; xx++)
            {
                peek = pool.Peek(xx);

                for (var x = 0; x < peek.Length; x++)
                {
                    Assert.AreEqual(add[x], peek[x].Value);
                    if (x < peek.Length - 1)
                        Assert.IsTrue(peek[x].Date >= peek[x + 1].Date);
                }
            }

            peek = pool.Pop(add.Length);

            for (var x = 0; x < peek.Length; x++)
            {
                Assert.AreEqual(add[x], peek[x].Value);
                if (x < peek.Length - 1)
                    Assert.IsTrue(peek[x].Date >= peek[x + 1].Date);
            }

            pool.Clear();
            Assert.AreEqual(0, pool.Count);
        }
    }
}
