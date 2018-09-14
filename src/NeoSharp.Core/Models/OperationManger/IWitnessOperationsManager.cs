namespace NeoSharp.Core.Models.OperationManger
{
    public interface IWitnessOperationsManager
    {
        void Sign(Witness witness);

        bool Verify(Witness witness);
    }
}
