using Moq;
using NeoSharp.TestHelpers.AutoMock;
using System;
using System.Linq;

namespace NeoSharp.TestHelpers
{
    public abstract class TestBase
    {
        private readonly MockRepository _mockRepository;
        protected readonly Random _rand;

        public IAutoMockContainer AutoMockContainer { get; private set; }

        /// <summary>
        /// Constructor
        /// </summary>
        protected TestBase()
        {
            _rand = new Random(Environment.TickCount);
            _mockRepository = new MockRepository(MockBehavior.Loose);
            AutoMockContainer = new UnityAutoMockContainer(_mockRepository);
        }

        /// <summary>
        /// Generate random strings
        /// </summary>
        /// <param name="length">String lenght</param>
        /// <returns>String</returns>
        public string RandomString(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            return new string(Enumerable.Repeat(chars, length)
              .Select(s => s[_rand.Next(s.Length)]).ToArray());
        }

        /// <summary>
        /// Verify All
        /// </summary>
        public void VerifyAll()
        {
            _mockRepository.VerifyAll();
        }
    }
}