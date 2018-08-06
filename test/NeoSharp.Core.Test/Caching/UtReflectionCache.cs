using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NeoSharp.Core.Caching;

namespace NeoSharp.Core.Test.Caching
{
    [TestClass]
    public class UtReflectionCache
    {
        // Arrange
        public enum DummyEnum
        {
            [ReflectionCache(typeof(Byte))]
            t_byte = 0x01,
            [ReflectionCache(typeof(Int32))]
            t_int32 = 0x02,
            [ReflectionCache(typeof(Int64))]
            t_int64 = 0x03
        }

        [TestMethod]
        public void Create_from_Enum()
        {
            // Arrange
            var enumValues = Enum.GetValues(typeof(DummyEnum));
            var enumType = typeof(DummyEnum);

            // Act
            var Cache = ReflectionCache<DummyEnum>.CreateFromEnum<DummyEnum>();

            // Assert
            CollectionAssert.AreEqual(enumValues, Cache.Keys);
            foreach (var t in enumValues)
            {
                var memInfo = enumType.GetMember(t.ToString());
                var attribute = memInfo[0].GetCustomAttributes(typeof(ReflectionCacheAttribute), false)
                    .Cast<ReflectionCacheAttribute>()
                    .FirstOrDefault();

                Cache.TryGetValue((DummyEnum)t, out var tValue);
                Assert.AreEqual(attribute.Type, tValue);
            }
        }

        [TestMethod]
        public void Create_from_Enum_Attr()
        {
            // Arrange
            var enumValues = Enum.GetValues(typeof(DummyEnum));
            var enumType = typeof(DummyEnum);

            // Act
            var Cache = ReflectionCache<DummyEnum, ReflectionCacheAttribute>.CreateFromEnum();

            // Assert
            CollectionAssert.AreEqual(enumValues, Cache.Keys);
            foreach (var t in enumValues)
            {
                var memInfo = enumType.GetMember(t.ToString());
                var attribute = memInfo[0].GetCustomAttributes(typeof(ReflectionCacheAttribute), false)
                    .Cast<ReflectionCacheAttribute>()
                    .FirstOrDefault();

                Cache.TryGetValue((DummyEnum)t, out var tValue);
                Assert.AreEqual(attribute.Type, tValue.Type);
            }
        }

        [TestMethod]
        public void Instance()
        {
            // Arrange
            var Cache = ReflectionCache<DummyEnum>.CreateFromEnum<DummyEnum>();

            // Act
            var inst1 = Cache.CreateInstance(DummyEnum.t_byte);
            var inst2 = Cache.CreateInstance(DummyEnum.t_int32);
            var inst3 = Cache.CreateInstance(DummyEnum.t_int64);

            // Assert
            Assert.AreEqual(typeof(System.Byte), inst1.GetType());
            Assert.AreEqual(typeof(System.Int32), inst2.GetType());
            Assert.AreEqual(typeof(System.Int64), inst3.GetType());
        }

        [TestMethod]
        public void Instance_Generic()
        {
            // Arrange
            var Cache = ReflectionCache<DummyEnum>.CreateFromEnum<DummyEnum>();

            // Act
            var inst1 = Cache.CreateInstance<Byte>(DummyEnum.t_byte);
            var inst2 = Cache.CreateInstance<Int32>(DummyEnum.t_int32);
            var inst3 = Cache.CreateInstance<Int64>(DummyEnum.t_int64);

            //Assert
            Assert.AreEqual(typeof(System.Byte), inst1.GetType());
            Assert.AreEqual(typeof(System.Int32), inst2.GetType());
            Assert.AreEqual(typeof(System.Int64), inst3.GetType());
        }
    }
}