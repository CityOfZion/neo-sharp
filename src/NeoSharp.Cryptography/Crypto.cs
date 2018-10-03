using System;
using System.Linq;
using System.Numerics;
using System.Text;

namespace NeoSharp.Cryptography
{
    public abstract class Crypto
    {
        //Use StaticDI to inject ICrypto
        public static Crypto Default { get; private set; } = new BouncyCastleCrypto();

        public static void Initialize(Crypto crypto) 
        {
            Default = crypto;
        }

        /// <summary>
        /// base58 Alphabet
        /// </summary>
        private const string Alphabet58 = "123456789ABCDEFGHJKLMNPQRSTUVWXYZabcdefghijkmnopqrstuvwxyz";
        /// <summary>
        /// Base
        /// </summary>
        private static readonly BigInteger Base58 = new BigInteger(58);

        /// <summary>
        /// Sha256 digests
        /// </summary>
        /// <param name="message">Message bytearray</param>
        /// <param name="offset">Offset</param>
        /// <param name="count">Length</param>
        /// <returns>Hash bytearray</returns>
        public abstract byte[] Sha256(byte[] message, int offset, int count);

        /// <summary>
        /// Sha256 digests
        /// </summary>
        /// <param name="message">Message bytearray</param>
        /// <returns>Hash bytearray</returns>
        public byte[] Sha256(byte[] message)
        {
            return Sha256(message, 0, message.Length);
        }

        /// <summary>
        /// RIPEMD160 digests
        /// </summary>
        /// <param name="message">Message bytearray</param>
        /// <param name="offset">Offset</param>
        /// <param name="count">Length</param>
        /// <returns>Hash bytearray</returns>
        public abstract byte[] RIPEMD160(byte[] message, int offset, int count);

        /// <summary>
        /// RIPEMD160 digests
        /// </summary>
        /// <param name="message">Message bytearray</param>
        /// <returns>Hash bytearray</returns>
        public byte[] RIPEMD160(byte[] message)
        {
            return RIPEMD160(message, 0, message.Length);
        }

        /// <summary>
        /// Hash256
        /// </summary>
        /// <param name="message">Message bytearray</param>
        /// <param name="offset">Offset</param>
        /// <param name="count">Length</param>
        /// <returns>Hash bytearray</returns>
        public byte[] Hash256(byte[] message, int offset, int count)
        {
            return Sha256(Sha256(message, offset, count));
        }

        /// <summary>
        /// Hash256
        /// </summary>
        /// <param name="message">Message bytearray</param>
        /// <returns>Hash bytearray</returns>
        public byte[] Hash256(byte[] message)
        {
            return Sha256(Sha256(message));
        }

        /// <summary>
        /// Hash160
        /// </summary>
        /// <param name="message">Message bytearray</param>
        /// <param name="offset">Offset</param>
        /// <param name="count">Length</param>
        /// <returns>Hash bytearray</returns>
        public byte[] Hash160(byte[] message, int offset, int count)
        {
            return RIPEMD160(Sha256(message, offset, count));
        }

        /// <summary>
        /// Hash160
        /// </summary>
        /// <param name="message">Message bytearray</param>
        /// <returns>Hash bytearray</returns>
        public byte[] Hash160(byte[] message)
        {
            return Hash160(message, 0, message.Length);
        }

        /// <summary>
        /// Murmur3 hash function
        /// </summary>
        /// <param name="message">Message bytearray</param>
        /// <param name="seed">Seed</param>
        /// <returns>Hash bytearray</returns>
        public abstract byte[] Murmur3(byte[] message, uint seed);

        /// <summary>
        /// Murmur3 hash function
        /// </summary>
        /// <param name="message">Message bytearray</param>
        /// <param name="seed">Seed</param>
        /// <returns>Hash uint32</returns>
        public uint Murmur32(byte[] message, uint seed)
        {
            return BitConverter.ToUInt32(Murmur3(message, seed));
        }

        /// <summary>
        /// Check ECDSA Signature (secp256r1)
        /// </summary>
        /// <param name="message">Message</param>
        /// <param name="signature">Signature</param>
        /// <param name="pubkey">Public Key</param>
        /// <returns>Bool</returns>
        public abstract bool VerifySignature(byte[] message, byte[] signature, byte[] pubkey);

        /// <summary>
        /// Sign sha256 Message (secp256r1)
        /// </summary>
        /// <param name="message">Message</param>
        /// <param name="prikey">Private Key</param>
        /// <returns>Siganture bytearray</returns>
        public abstract byte[] Sign(byte[] message, byte[] prikey);

        /// <summary>
        /// Derive Public Key from private
        /// </summary>
        /// <param name="privateKey">Private Key</param>
        /// <param name="compress">Compress pubkey</param>
        /// <returns>Bytearray Public Key</returns>
        public abstract byte[] ComputePublicKey(byte[] privateKey, bool compress);

        /// <summary>
        /// Decode Public Key
        /// </summary>
        /// <param name="pubkey">Data</param>
        /// <param name="compress">Compress public key</param>
        /// <param name="x">X</param>
        /// <param name="y">Y</param>
        /// <returns>Public key bytearray</returns>
        public abstract byte[] DecodePublicKey(byte[] pubkey, bool compress, out BigInteger x, out BigInteger y);

        /// <summary>
        /// Encrypt using ECB
        /// </summary>
        /// <param name="data">Data</param>
        /// <param name="key">Key</param>
        /// <returns>Bytearray</returns>
        public abstract byte[] AesEncrypt(byte[] data, byte[] key);

        /// <summary>
        /// Decrypt using ECB
        /// </summary>
        /// <param name="data">Data</param>
        /// <param name="key">Key</param>
        /// <returns>Bytearray</returns>
        public abstract byte[] AesDecrypt(byte[] data, byte[] key);

        /// <summary>
        /// Encrypt using CBC
        /// </summary>
        /// <param name="data">Data</param>
        /// <param name="key">Key</param>
        /// <param name="iv">IV</param>
        /// <returns>Bytearray</returns>
        public abstract byte[] AesEncrypt(byte[] data, byte[] key, byte[] iv);

        /// <summary>
        /// Decrypt using CBC
        /// </summary>
        /// <param name="data">Data</param>
        /// <param name="key">Key</param>
        /// <param name="iv">IV</param>
        /// <returns>Bytearray</returns>
        public abstract byte[] AesDecrypt(byte[] data, byte[] key, byte[] iv);

        /// <summary>
        /// Generate SCrypt key
        /// </summary>
        /// <param name="P">Password</param>
        /// <param name="S">Salt</param>
        /// <param name="N">CPU/Memory cost parameter. Must be larger than 1, a power of 2 and less than 2^(128 * r / 8).</param>
        /// <param name="r">Block size, must be >= 1.</param>
        /// <param name="p">Parallelization. Must be a positive integer less than or equal to Int32.MaxValue / (128 * r * 8).</param>
        /// <param name="dkLen">Generate key length</param>
        public abstract byte[] SCrypt(byte[] P, byte[] S, int N, int r, int p, int dkLen);

        /// <summary>
        /// Decode
        /// </summary>
        /// <param name="input">String to be decoded</param>
        /// <returns>Decoded Byte Array</returns>
        public virtual byte[] Base58Decode(string input)
        {
            BigInteger bi = BigInteger.Zero;
            for (int i = input.Length - 1; i >= 0; i--)
            {
                int index = Alphabet58.IndexOf(input[i]);
                if (index == -1)
                    throw new FormatException();
                bi += index * BigInteger.Pow(Base58, input.Length - 1 - i);
            }
            byte[] bytes = bi.ToByteArray();
            Array.Reverse(bytes);
            bool stripSignByte = bytes.Length > 1 && bytes[0] == 0 && bytes[1] >= 0x80;
            int leadingZeros = 0;
            for (int i = 0; i < input.Length && input[i] == Alphabet58[0]; i++)
            {
                leadingZeros++;
            }
            byte[] tmp = new byte[bytes.Length - (stripSignByte ? 1 : 0) + leadingZeros];
            Array.Copy(bytes, stripSignByte ? 1 : 0, tmp, leadingZeros, tmp.Length - leadingZeros);
            return tmp;
        }

        /// <summary>
        /// Encode
        /// </summary>
        /// <param name="input">Byte Array to encode</param>
        /// <returns>Encoded string</returns>
        public virtual string Base58Encode(byte[] input)
        {
            BigInteger value = new BigInteger(new byte[1].Concat(input).Reverse().ToArray());
            StringBuilder sb = new StringBuilder();
            while (value >= Base58)
            {
                BigInteger mod = value % Base58;
                sb.Insert(0, Alphabet58[(int)mod]);
                value /= Base58;
            }
            sb.Insert(0, Alphabet58[(int)value]);
            foreach (byte b in input)
            {
                if (b == 0)
                    sb.Insert(0, Alphabet58[0]);
                else
                    break;
            }
            return sb.ToString();
        }

        /// <summary>
        /// Encode to Base58Check
        /// </summary>
        /// <param name="data">Bytearray</param>
        /// <returns>Base58Check string</returns>
        public virtual string Base58CheckEncode(byte[] data)
        {
            byte[] checksum = Sha256(Sha256(data));
            byte[] buffer = new byte[data.Length + 4];
            Buffer.BlockCopy(data, 0, buffer, 0, data.Length);
            Buffer.BlockCopy(checksum, 0, buffer, data.Length, 4);
            return Base58Encode(buffer);
        }

        /// <summary>
        /// Decode Base58Check string
        /// </summary>
        /// <param name="input">Base58Check string</param>
        /// <returns>Bytearray</returns>
        public virtual byte[] Base58CheckDecode(string input)
        {
            byte[] buffer = Base58Decode(input);
            if (buffer.Length < 4) throw new FormatException();
            byte[] checksum = Sha256(Sha256(buffer, 0, buffer.Length - 4));
            if (!buffer.Skip(buffer.Length - 4).SequenceEqual(checksum.Take(4)))
                throw new FormatException();
            return buffer.Take(buffer.Length - 4).ToArray();
        }

        /// <summary>
        /// Generates random bytes
        /// </summary>
        /// <param name="length">Length</param>
        /// <returns>Random bytearray</returns>
        public abstract byte[] GenerateRandomBytes(int length);
    }
}