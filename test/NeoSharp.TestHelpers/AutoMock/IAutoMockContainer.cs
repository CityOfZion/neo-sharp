using Moq;

namespace NeoSharp.TestHelpers.AutoMock
{
    public interface IAutoMockContainer
    {
        T Create<T>();

        T Get<T>() where T : class;

        Mock<T> GetMock<T>() where T : class;

        void Register<TService, TImplementation>() where TImplementation : TService;

        void Register<TService>(TService instance);

        void Register<TService>(TService instance, string name);
    }
}
