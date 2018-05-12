namespace NeoSharp.Core.Cryptography
{
    public interface ICrypto
    {
        byte[] Hash160(byte[] message);
        byte[] Hash256(byte[] message);
        bool VerifySignature(byte[] message, byte[] signature, byte[] pubkey);
    }
}