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
            this._mockRepository = new MockRepository(MockBehavior.Loose);
            this.AutoMockContainer = new UnityAutoMockContainer(this._mockRepository);
        }

        public void VerifyAll()
        {
            this._mockRepository.VerifyAll();
        }
    }
}
