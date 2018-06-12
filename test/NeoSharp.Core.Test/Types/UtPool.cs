using Microsoft.VisualStudio.TestTools.UnitTesting;
using NeoSharp.Core.Extensions;
using NeoSharp.Core.Models;
using NeoSharp.Core.Types;
using NeoSharp.TestHelpers;
using System;

namespace NeoSharp.Core.Test.Types
{
    [TestClass]
    public class UtPool : TestBase
    {
        [TestMethod]
        public void Test_Pool_RemoveFromEnd()
        {
            Pool<UInt256, Transaction> pool = new Pool<UInt256, Transaction>(
                PoolMaxBehaviour.RemoveFromEnd, 3, x => x.Hash, (a, b) => a.Hash.CompareTo(b.Hash));

            Assert.AreEqual(PoolMaxBehaviour.RemoveFromEnd, pool.Behaviour);
            Assert.AreEqual(3, pool.Max);
            Assert.AreEqual(0, pool.Count);

            Transaction[] add = new Transaction[]
            {
                new Transaction(){ Hash=new UInt256("3A259DBA256600620C6C91094F3A300B30F0CBAECEE19C6114DEFFD3288957D7".HexToBytes())},
                new Transaction(){ Hash=new UInt256("2A259DBA256600620C6C91094F3A300B30F0CBAECEE19C6114DEFFD3288957D7".HexToBytes())},
                new Transaction(){ Hash=new UInt256("1A259DBA256600620C6C91094F3A300B30F0CBAECEE19C6114DEFFD3288957D7".HexToBytes())},
            };

            foreach (Transaction t in add) Assert.IsTrue(pool.Push(t));

            add = new Transaction[]
            {
                new Transaction(){ Hash=new UInt256("03259DBA256600620C6C91094F3A300B30F0CBAECEE19C6114DEFFD3288957D7".HexToBytes())},
                new Transaction(){ Hash=new UInt256("02259DBA256600620C6C91094F3A300B30F0CBAECEE19C6114DEFFD3288957D7".HexToBytes())},
                new Transaction(){ Hash=new UInt256("01259DBA256600620C6C91094F3A300B30F0CBAECEE19C6114DEFFD3288957D7".HexToBytes())},
            };

            Assert.IsTrue(pool.Push(add[0]));
            Assert.IsTrue(pool.Push(add[1]));
            Assert.IsTrue(pool.Push(add[2]));

            // Test order

            Array.Reverse(add);

            var peek = pool.Pop(add.Length);

            for (int x = 0; x < peek.Length; x++)
                Assert.AreEqual(add[x], peek[x]);

            pool.Clear();
            Assert.AreEqual(0, pool.Count);
        }

        [TestMethod]
        public void Test_Pool_DontAllowMore()
        {
            Pool<UInt256, Transaction> pool = new Pool<UInt256, Transaction>(
                PoolMaxBehaviour.DontAllowMore, 3, x => x.Hash, (a, b) => a.Hash.CompareTo(b.Hash));

            Assert.AreEqual(PoolMaxBehaviour.DontAllowMore, pool.Behaviour);
            Assert.AreEqual(3, pool.Max);
            Assert.AreEqual(0, pool.Count);

            Transaction[] add = new Transaction[]
            {
                new Transaction(){ Hash=new UInt256("3A259DBA256600620C6C91094F3A300B30F0CBAECEE19C6114DEFFD3288957D7".HexToBytes())},
                new Transaction(){ Hash=new UInt256("2A259DBA256600620C6C91094F3A300B30F0CBAECEE19C6114DEFFD3288957D7".HexToBytes())},
                new Transaction(){ Hash=new UInt256("1A259DBA256600620C6C91094F3A300B30F0CBAECEE19C6114DEFFD3288957D7".HexToBytes())},
            };

            foreach (Transaction t in add) Assert.IsTrue(pool.Push(t));
            foreach (Transaction t in add) Assert.IsFalse(pool.Push(t));

            // Test order

            Array.Reverse(add);

            // Check

            Transaction[] peek;
            for (int xx = 0; xx < add.Length; xx++)
            {
                peek = pool.Peek(xx);

                for (int x = 0; x < peek.Length; x++)
                    Assert.AreEqual(add[x], peek[x]);
            }

            peek = pool.Pop(add.Length);

            for (int x = 0; x < peek.Length; x++)
                Assert.AreEqual(add[x], peek[x]);

            pool.Clear();
            Assert.AreEqual(0, pool.Count);
        }
    }
}