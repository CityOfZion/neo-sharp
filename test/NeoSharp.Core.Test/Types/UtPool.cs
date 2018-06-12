using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NeoSharp.Core.Extensions;
using NeoSharp.Core.Types;
using NeoSharp.TestHelpers;

namespace NeoSharp.Core.Test.Types
{
    [TestClass]
    public class UtPool : TestBase
    {
        [TestMethod]
        public void Test_Pool_RemoveFromEnd()
        {
            Pool<UInt256, UInt256> pool = new Pool<UInt256, UInt256>(
                PoolMaxBehaviour.RemoveFromEnd, 3, x => x, (a, b) => a.CompareTo(b));

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
                Assert.AreEqual(add[x], peek[x]);

            pool.Clear();
            Assert.AreEqual(0, pool.Count);
        }

        [TestMethod]
        public void Test_Pool_DontAllowMore()
        {
            Pool<UInt256, UInt256> pool = new Pool<UInt256, UInt256>(
                PoolMaxBehaviour.DontAllowMore, 3, x => x, (a, b) => a.CompareTo(b));

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

            UInt256[] peek;
            for (int xx = 0; xx < add.Length; xx++)
            {
                peek = pool.Peek(xx);

                for (var x = 0; x < peek.Length; x++)
                    Assert.AreEqual(add[x], peek[x]);
            }

            peek = pool.Pop(add.Length);

            for (var x = 0; x < peek.Length; x++)
                Assert.AreEqual(add[x], peek[x]);

            pool.Clear();
            Assert.AreEqual(0, pool.Count);
        }
    }
}