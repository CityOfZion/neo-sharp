using Moq;
using NeoSharp.TestHelpers.AutoMock;
using System;

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

        public void VerifyAll()
        {
            _mockRepository.VerifyAll();
        }
    }
}