using System;
using System.Linq;
using Org.BouncyCastle.Asn1;
using Org.BouncyCastle.Asn1.Sec;
using Org.BouncyCastle.Asn1.X9;
using Org.BouncyCastle.Crypto.Digests;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Crypto.Prng;
using Org.BouncyCastle.Crypto.Signers;
using Org.BouncyCastle.Math;
using Org.BouncyCastle.Security;

namespace NeoSharp.Core.Cryptography
{
    public class BouncyCastleCrypto : Crypto
    {
        static readonly X9ECParameters _curve = SecNamedCurves.GetByName("secp256r1");
        static readonly ECDomainParameters _domain = new ECDomainParameters(_curve.Curve, _curve.G, _curve.N, _curve.H, _curve.GetSeed());

        /// <summary>
        /// Sha256 digests
        /// </summary>
        /// <param name="message">Message bytearray</param>
        /// <param name="offset">Offset</param>
        /// <param name="count">Length</param>
        /// <returns>Hash bytearray</returns>
        public override byte[] Sha256(byte[] message, int offset, int count)
        {
            var hash = new Sha256Digest();
            hash.BlockUpdate(message, offset, count);

            byte[] result = new byte[32];
            hash.DoFinal(result, 0);

            return result;
        }

        /// <summary>
        /// RIPEMD160 digests
        /// </summary>
        /// <param name="message">Message bytearray</param>
        /// <param name="offset">Offset</param>
        /// <param name="count">Length</param>
        /// <returns>Hash bytearray</returns>
        public override byte[] RIPEMD160(byte[] message, int offset, int count)
        {
            var hash = new RipeMD160Digest();
            hash.BlockUpdate(message, offset, count);

            byte[] result = new byte[20];
            hash.DoFinal(result, 0);

            return result;
        }

        /// <summary>
        /// Murmur3 hash function
        /// </summary>
        /// <param name="message">Message bytearray</param>
        /// <param name="seed">Seed</param>
        /// <returns>Hash bytearray</returns>
        public override byte[] Murmur3(byte[] message, uint seed)
        {
            using (var murmur = new Murmur3(seed))
            {
                return murmur.ComputeHash(message);
            }
        }

        /// <summary>
        /// Check ECDSA Signature (secp256r1)
        /// </summary>
        /// <param name="message">Message</param>
        /// <param name="signature">Signature</param>
        /// <param name="pubkey">Public Key</param>
        /// <returns>Bool</returns>
        public override bool VerifySignature(byte[] message, byte[] signature, byte[] pubkey)
        {
            byte[] fullpubkey = DecodePublicKey(pubkey, false, out System.Numerics.BigInteger x, out System.Numerics.BigInteger y);

            Org.BouncyCastle.Math.EC.ECPoint point = _curve.Curve.DecodePoint(fullpubkey);
            var keyParameters = new ECPublicKeyParameters(point, _domain);

            var signer = SignerUtilities.GetSigner("SHA256withECDSA");
            signer.Init(false, keyParameters);
            signer.BlockUpdate(message, 0, message.Length);

            if (signature.Length == 64)
            {
                signature = new DerSequence(
                new DerInteger(new BigInteger(1, signature.Take(32).ToArray())),
                new DerInteger(new BigInteger(1, signature.Skip(32).ToArray())))
                .GetDerEncoded();
            }

            return signer.VerifySignature(signature);
        }

        /// <summary>
        /// Sign sha256 Message (secp256r1)
        /// </summary>
        /// <param name="message">Message</param>
        /// <param name="prikey">Private Key</param>
        /// <returns>Siganture bytearray</returns>
        public override byte[] Sign(byte[] message, byte[] prikey)
        {
            ECPrivateKeyParameters priv = new ECPrivateKeyParameters("ECDSA", (new BigInteger(1, prikey)), _domain);
            var signer = new ECDsaSigner();
            var fullsign = new byte[64];

            message = Sha256(message);
            signer.Init(true, priv);
            var signature = signer.GenerateSignature(message);
            var r = signature[0].ToByteArray();
            var s = signature[1].ToByteArray();
            var rLen = r.Length;
            var sLen = s.Length;

            // Buid Signature ensuring Neo expected format. 32byte r + 32byte s.
            if (rLen < 32)
            {
                Array.Copy(r, 0, fullsign, 32 - rLen, rLen);
            }
            else
            {
                Array.Copy(r, rLen - 32, fullsign, 0, 32);
            }
            if (sLen < 32)
            {
                Array.Copy(s, 0, fullsign, 64 - sLen, sLen);
            }
            else
            {
                Array.Copy(s, sLen - 32, fullsign, 32, 32);
            }

            return fullsign;
        }

        /// <summary>
        /// Derive Public Key from private
        /// </summary>
        /// <param name="privateKey">Private Key</param>
        /// <param name="compress">Compress pubkey</param>
        /// <returns>Bytearray Public Key</returns>
        public override byte[] ComputePublicKey(byte[] privateKey, bool compress = false)
        {
            if (privateKey == null) throw new ArgumentException(nameof(privateKey));

            var q = _domain.G.Multiply(new BigInteger(1, privateKey));
            var publicParams = new ECPublicKeyParameters(q, _domain);

            return publicParams.Q.GetEncoded(compress);
        }

        /// <summary>
        /// Decode Public Key
        /// </summary>
        /// <param name="pubkey">Data</param>
        /// <param name="compress">Compress public key</param>
        /// <param name="x">X</param>
        /// <param name="y">Y</param>
        /// <returns>Public key bytearray</returns>
        public override byte[] DecodePublicKey(byte[] pubkey, bool compress, out System.Numerics.BigInteger x, out System.Numerics.BigInteger y)
        {
            if (pubkey == null || pubkey.Length != 33 && pubkey.Length != 64 && pubkey.Length != 65)
            {
                throw new ArgumentException(nameof(pubkey));
            }

            if (pubkey.Length == 33 && pubkey[0] != 0x02 && pubkey[0] != 0x03) throw new ArgumentException(nameof(pubkey));
            if (pubkey.Length == 65 && pubkey[0] != 0x04) throw new ArgumentException(nameof(pubkey));

            byte[] fullpubkey;

            if (pubkey.Length == 64)
            {
                fullpubkey = new byte[65];
                fullpubkey[0] = 0x04;
                Array.Copy(pubkey, 0, fullpubkey, 1, pubkey.Length);
            }
            else
            {
                fullpubkey = pubkey;
            }

            var ret = new ECPublicKeyParameters("ECDSA", _curve.Curve.DecodePoint(fullpubkey), _domain).Q;
            var x0 = ret.XCoord.ToBigInteger();
            var y0 = ret.YCoord.ToBigInteger();

            x = System.Numerics.BigInteger.Parse(x0.ToString());
            y = System.Numerics.BigInteger.Parse(y0.ToString());

            return ret.GetEncoded(compress);
        }

        /// <summary>
        /// Encrypt using ECB
        /// </summary>
        /// <param name="data">Data</param>
        /// <param name="key">Key</param>
        /// <returns>Bytearray</returns>
        public override byte[] AesEncrypt(byte[] data, byte[] key)
        {
            if (data == null || data.Length % 16 != 0) throw new ArgumentException(nameof(data));
            if (key == null || key.Length != 32) throw new ArgumentException(nameof(key));

            var cipher = CipherUtilities.GetCipher("AES/ECB/NoPadding");
            cipher.Init(true, ParameterUtilities.CreateKeyParameter("AES", key));

            return cipher.DoFinal(data);
        }

        /// <summary>
        /// Decrypt using ECB
        /// </summary>
        /// <param name="data">Data</param>
        /// <param name="key">Key</param>
        /// <returns>Bytearray</returns>
        public override byte[] AesDecrypt(byte[] data, byte[] key)
        {
            if (data == null || data.Length % 16 != 0) throw new ArgumentException(nameof(data));
            if (key == null || key.Length != 32) throw new ArgumentException(nameof(key));

            var cipher = CipherUtilities.GetCipher("AES/ECB/NoPadding");
            cipher.Init(false, ParameterUtilities.CreateKeyParameter("AES", key));

            return cipher.DoFinal(data);
        }

        /// <summary>
        /// Encrypt/Decrypt using CBC
        /// </summary>
        /// <param name="data">Data</param>
        /// <param name="key">Key</param>
        /// <param name="iv">IV</param>
        /// <returns>Bytearray</returns>
        public override byte[] AesEncrypt(byte[] data, byte[] key, byte[] iv)
        {
            if (data == null || data.Length % 16 != 0) throw new ArgumentException(nameof(data));
            if (key == null || key.Length != 32) throw new ArgumentException(nameof(key));
            if (iv == null || iv.Length != 16) throw new ArgumentException(nameof(iv));

            var cipher = CipherUtilities.GetCipher("AES/CBC/NoPadding");
            cipher.Init(true, new ParametersWithIV(ParameterUtilities.CreateKeyParameter("AES", key), iv));

            return cipher.DoFinal(data);
        }

        /// <summary>
        /// Encrypt/Decrypt using CBC
        /// </summary>
        /// <param name="data">Data</param>
        /// <param name="key">Key</param>
        /// <param name="iv">IV</param>
        /// <returns>Bytearray</returns>
        public override byte[] AesDecrypt(byte[] data, byte[] key, byte[] iv)
        {
            if (data == null || data.Length % 16 != 0) throw new ArgumentException(nameof(data));
            if (key == null || key.Length != 32) throw new ArgumentException(nameof(key));
            if (iv == null || iv.Length != 16) throw new ArgumentException(nameof(iv));

            var cipher = CipherUtilities.GetCipher("AES/CBC/NoPadding");
            cipher.Init(false, new ParametersWithIV(ParameterUtilities.CreateKeyParameter("AES", key), iv));

            return cipher.DoFinal(data);
        }

        /// <summary>
        /// Generate SCrypt key
        /// </summary>
        /// <param name="P">Password</param>
        /// <param name="S">Salt</param>
        /// <param name="N">CPU/Memory cost parameter. Must be larger than 1, a power of 2 and less than 2^(128 * r / 8).</param>
        /// <param name="r">Block size, must be >= 1.</param>
        /// <param name="p">Parallelization. Must be a positive integer less than or equal to Int32.MaxValue / (128 * r * 8).</param>
        /// <param name="dkLen">Generate key length</param>
        public override byte[] SCrypt(byte[] P, byte[] S, int N, int r, int p, int dkLen)
        {
            if (P == null) throw new ArgumentException(nameof(P));
            if (S == null) throw new ArgumentException(nameof(S));
            if ((N & (N - 1)) != 0 || N < 2 || N >= Math.Pow(2, (128 * r / 8))) throw new ArgumentException(nameof(N));
            if (r < 1) throw new ArgumentException(nameof(r));
            if (p < 1 || p > Int32.MaxValue / (128 * r * 8)) throw new ArgumentException(nameof(p));
            if (dkLen < 1) throw new ArgumentException(nameof(dkLen));

            return Org.BouncyCastle.Crypto.Generators.SCrypt.Generate(P, S, N, r, p, dkLen);
        }

        /// <summary>
        /// Generates random bytes
        /// </summary>
        /// <param name="length">Length</param>
        /// <returns>Random bytearray</returns>
        public override byte[] GenerateRandomBytes(int length)
        {
            if (length < 1) throw new ArgumentException(nameof(length));

            // NativePRNG
            SecureRandom random = new SecureRandom();
            var seed = new ThreadedSeedGenerator();
            random.SetSeed(seed.GenerateSeed(32, false));

            var randombytes = new byte[length];
            random.NextBytes(randombytes);

            return randombytes;
        }
    }
}