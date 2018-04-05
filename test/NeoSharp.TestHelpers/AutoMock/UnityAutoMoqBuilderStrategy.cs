using System;
using System.Collections.Generic;
using System.Reflection;
using Moq;
using Unity.Builder;
using Unity.Builder.Strategy;

namespace NeoSharp.TestHelpers.AutoMock
{
    public class UnityAutoMoqBuilderStrategy : BuilderStrategy
    {
        private readonly UnityAutoMockContainer _autoMockContainer;
        private readonly MethodInfo _createMethod;
        private readonly MockRepository _mockRepository;
        private readonly Dictionary<Type, object> _mocks;

        public UnityAutoMoqBuilderStrategy(
            MockRepository mockRepository,
            UnityAutoMockContainer autoMockContainer)
        {
            this._mockRepository = mockRepository;
            this._autoMockContainer = autoMockContainer;

            this._createMethod = mockRepository.GetType().GetMethod("Create", new Type[] { });

            this._mocks = new Dictionary<Type, object>();
        }

        public override void PreBuildUp(IBuilderContext context)
        {
            var type = context.OriginalBuildKey.Type;

            if (type.IsInterface || type.IsAbstract)
            {
                context.Existing = this.GetOrCreateMock(type);
                context.BuildComplete = true;
            }
        }

        private object GetOrCreateMock(Type t)
        {
            if (this._mocks.ContainsKey(t))
            {
                return this._mocks[t];
            }

            var genericType = typeof(Mock<>).MakeGenericType(t);

            var specificCreateMethod = this._createMethod.MakeGenericMethod(t);
            var mock = (Mock)specificCreateMethod.Invoke(this._mockRepository, null);

            var interfaceImplementations = this._autoMockContainer.GetInterfaceImplementations(t);
            if (interfaceImplementations != null)
            {
                foreach (var implementation in interfaceImplementations.GetImplementations())
                {
                    genericType.GetMethod("As").MakeGenericMethod(implementation).Invoke(mock, null);
                }
            }

            var mockedInstance = mock.Object;
            this._mocks.Add(t, mockedInstance);

            return mockedInstance;
        }
    }
}
