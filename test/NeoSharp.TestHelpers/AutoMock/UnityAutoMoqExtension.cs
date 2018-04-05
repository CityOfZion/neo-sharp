using Moq;
using Unity.Builder;
using Unity.Extension;

namespace NeoSharp.TestHelpers.AutoMock
{
    public class UnityAutoMoqExtension : UnityContainerExtension
    {
        private readonly MockRepository _mockRepository;
        private readonly UnityAutoMockContainer _autoMockContainer;

        public UnityAutoMoqExtension(
            MockRepository mockRepository,
            UnityAutoMockContainer autoMockContainer)
        {
            this._mockRepository = mockRepository;
            this._autoMockContainer = autoMockContainer;
        }

        protected override void Initialize()
        {
            Context.Strategies.Add(
                new UnityAutoMoqBuilderStrategy(this._mockRepository, this._autoMockContainer),
                UnityBuildStage.PreCreation);
        }
    }
}
