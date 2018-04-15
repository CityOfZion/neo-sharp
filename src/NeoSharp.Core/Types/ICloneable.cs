namespace NeoSharp.Core.Types
{
    public interface ICloneable<T>
    {
        T Clone();
        void FromReplica(T replica);
    }
}
