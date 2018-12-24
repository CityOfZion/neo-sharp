using Neo.VM;

namespace NeoSharp.VM.NeoVM
{
    public class ScriptContainerWrapper : IScriptContainer
    {
        private readonly IMessageProvider _messageProvider;

        public ScriptContainerWrapper(IMessageProvider messageProvider)
        {
            _messageProvider = messageProvider;
        }

        public byte[] GetMessage()
        {
            return _messageProvider.GetMessageData(0);
        }
    }
}