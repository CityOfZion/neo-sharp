using NeoSharp.Core.Cryptography;
using NeoSharp.Core.Types;

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
            this._crypto = crypto;
        }
        #endregion

        #region IWitnessOperationsManager implementation 
        public void Sign(Witness witness)
        {
            witness.Hash = new UInt160(this._crypto.Hash160(witness.VerificationScript));
        }

        public bool Verify(Witness witness)
        {
            throw new System.NotImplementedException();
        }
        #endregion
    }
}