using NeoSharp.Cryptography;
using NeoSharp.Types;

namespace NeoSharp.Core.Models.OperationManger
{
    public class WitnessOperationsManager : IWitnessOperationsManager
    {
        #region Private Fields

        private readonly Crypto _crypto;

        #endregion

        #region Constructor 

        public WitnessOperationsManager(Crypto crypto)
        {
            _crypto = crypto;
        }

        #endregion

        #region IWitnessOperationsManager implementation 

        public void Sign(Witness witness)
        {
            witness.Hash = new UInt160(_crypto.Hash160(witness.VerificationScript));
        }

        public bool Verify(Witness witness)
        {
            // TODO [AboimPinto]: need to be implemented.
            return true;
        }
        
        #endregion
    }
}