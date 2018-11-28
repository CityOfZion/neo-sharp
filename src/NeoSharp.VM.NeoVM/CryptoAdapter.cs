using Neo.VM;
using NeoSharp.Cryptography;

namespace NeoSharp.VM.NeoVM
{
    public class CryptoAdapter : ICrypto
    {
        public byte[] Hash160(byte[] message)
        {
            return Crypto.Default.Hash160(message);
        }

        public byte[] Hash256(byte[] message)
        {
            return Crypto.Default.Hash256(message);
        }

        public bool VerifySignature(byte[] message, byte[] signature, byte[] pubkey)
        {
            return Crypto.Default.VerifySignature(message, signature, pubkey);
        }
    }
}