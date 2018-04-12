using Moq;
using NeoSharp.TestHelpers.AutoMock;

namespace NeoSharp.TestHelpers
{
    public abstract class TestBase
    {
        private readonly MockRepository _mockRepository;

        public IAutoMockContainer AutoMockContainer { get; private set; }

        protected TestBase()
        {
            _mockRepository = new MockRepository(MockBehavior.Loose);
            AutoMockContainer = new UnityAutoMockContainer(_mockRepository);
        }

        public void VerifyAll()
        {
            _mockRepository.VerifyAll();
        }
    }
}
