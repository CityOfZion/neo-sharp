using System;
using System.Threading.Tasks;
using NeoSharp.BinarySerialization;
using NeoSharp.Core.DI;
using NeoSharp.Core.Logging;
using NeoSharp.Core.Models;
using NeoSharp.VM;

namespace NeoSharp.Core.Blockchain.Processing
{
    public class InvocationTransactionPersister: ITransactionPersister<InvocationTransaction>
    {
        private readonly IVMFactory _vmFactory;
        private readonly IBinarySerializer _binarySerializer;
        private readonly IContainer _container;

        public InvocationTransactionPersister(
            IVMFactory vmFactory,
            IBinarySerializer binarySerializer,
            IContainer container)
        {
            _vmFactory = vmFactory ?? throw new ArgumentNullException(nameof(vmFactory));
            _binarySerializer = binarySerializer ?? throw new ArgumentNullException(nameof(binarySerializer));
            _container = container ?? throw new ArgumentNullException(nameof(container));
        }

        public Task Persist(InvocationTransaction transaction)
        {
            var messageContainer = _container.Resolve<IMessageContainer>();
            var executionEngineArgs = GetExecutionEngineArgs(_container, messageContainer);

            PrepareMessage(transaction, messageContainer);

            using (var engine = _vmFactory.Create(executionEngineArgs))
            {
                engine.LoadScript(transaction.Script);

                if (!engine.Execute((uint)transaction.Gas.Value))
                {
                    throw new InvalidOperationException($"The transaction {transaction.Hash} cannot be executed by VM.");
                }
            }

            return Task.CompletedTask;
        }

        private void PrepareMessage(InvocationTransaction transaction, IMessageContainer messageContainer)
        {
            var message = _binarySerializer.Serialize(transaction);

            messageContainer.RegisterMessage(message);
        }

        private static ExecutionEngineArgs GetExecutionEngineArgs(IContainer container, IMessageProvider messageProvider)
        {
            var args = new ExecutionEngineArgs
            {
                Trigger = ETriggerType.Application,
                InteropService = container.Resolve<InteropService>(),
                ScriptTable = container.Resolve<IScriptTable>(),
                MessageProvider = messageProvider,
                Logger = container.Resolve<ExecutionEngineLogger>()
            };

            var logger = container.Resolve<ILogger<ExecutionEngineLogger>>();

            args.InteropService.OnLog += (s, e) => logger.LogDebug("Log: " + e.Message);
            args.InteropService.OnNotify += (s, e) => logger.LogDebug("Notification: " + e.State.ToString());
            args.Logger.OnStepInto += ctx => logger.LogDebug(ctx.ToString());

            return args;
        }
    }
}