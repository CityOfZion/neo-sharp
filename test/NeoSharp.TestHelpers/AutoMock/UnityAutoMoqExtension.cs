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
            _mockRepository = mockRepository;
            _autoMockContainer = autoMockContainer;
        }

        protected override void Initialize()
        {
            Context.Strategies.Add(
                new UnityAutoMoqBuilderStrategy(_mockRepository, _autoMockContainer),
                UnityBuildStage.PreCreation);
        }
    }
}
