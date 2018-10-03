using NeoSharp.Types;

namespace NeoSharp.Core.Network
{
    //Gubanotorious 4.11.18 - removed IVerifiable for now, tons of coupling / dependencies that needs to be
    //Handled in restructuring. 
    public interface IInventory //:IVerifiable
    {
        UInt256 Hash { get; }

        InventoryType InventoryType { get; }

        bool Verify();
    }
}
