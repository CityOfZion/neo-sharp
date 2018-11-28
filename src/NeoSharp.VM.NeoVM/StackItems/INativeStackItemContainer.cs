using Newtonsoft.Json;

namespace NeoSharp.VM.NeoVM.StackItems
{
    public interface INativeStackItemContainer
    {
        [JsonIgnore]
        Neo.VM.StackItem NativeStackItem { get; }
    }
}