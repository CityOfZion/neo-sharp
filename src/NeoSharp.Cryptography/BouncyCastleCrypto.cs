using System;
using System.Linq;
using Org.BouncyCastle.Asn1;
using Org.BouncyCastle.Asn1.Sec;
using Org.BouncyCastle.Asn1.X9;
using Org.BouncyCastle.Crypto.Digests;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Crypto.Signers;
using Org.BouncyCastle.Math;
using Org.BouncyCastle.Security;

namespace NeoSharp.Cryptography
{
    public class BouncyCastleCrypto : Crypto
    {
        static readonly X9ECParameters _curve = SecNamedCurves.GetByName("secp256r1");
        static readonly ECDomainParameters _domain = new ECDomainParameters(_curve.Curve, _curve.G, _curve.N, _curve.H, _curve.GetSeed());

        /// <inheritdoc />
        public override byte[] Sha256(byte[] message, int offset, int count)
        {
            var hash = new Sha256Digest();
            hash.BlockUpdate(message, offset, count);

            byte[] result = new byte[32];
            hash.DoFinal(result, 0);

            return result;
        }

        /// <inheritdoc />
        public override byte[] RIPEMD160(byte[] message, int offset, int count)
        {
            var hash = new RipeMD160Digest();
            hash.BlockUpdate(message, offset, count);

            byte[] result = new byte[20];
            hash.DoFinal(result, 0);

            return result;
        }

        /// <inheritdoc />
        public override byte[] Murmur3(byte[] message, uint seed)
        {
            using (var murmur = new Murmur3(seed))
            {
                return murmur.ComputeHash(message);
            }
        }

        /// <inheritdoc />
        public override bool VerifySignature(byte[] message, byte[] signature, byte[] pubkey)
        {
            var fullpubkey = DecodePublicKey(pubkey, false, out _, out _);

            var point = _curve.Curve.DecodePoint(fullpubkey);
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

        /// <inheritdoc />
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

        /// <inheritdoc />
        public override byte[] ComputePublicKey(byte[] privateKey, bool compress = false)
        {
            if (privateKey == null) throw new ArgumentException(nameof(privateKey));

            var q = _domain.G.Multiply(new BigInteger(1, privateKey));
            var publicParams = new ECPublicKeyParameters(q, _domain);

            return publicParams.Q.GetEncoded(compress);
        }

        /// <inheritdoc />
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

        /// <inheritdoc />
        public override byte[] AesEncrypt(byte[] data, byte[] key)
        {
            if (data == null || data.Length % 16 != 0) throw new ArgumentException(nameof(data));
            if (key == null || key.Length != 32) throw new ArgumentException(nameof(key));

            var cipher = CipherUtilities.GetCipher("AES/ECB/NoPadding");
            cipher.Init(true, ParameterUtilities.CreateKeyParameter("AES", key));

            return cipher.DoFinal(data);
        }

        /// <inheritdoc />
        public override byte[] AesDecrypt(byte[] data, byte[] key)
        {
            if (data == null || data.Length % 16 != 0) throw new ArgumentException(nameof(data));
            if (key == null || key.Length != 32) throw new ArgumentException(nameof(key));

            var cipher = CipherUtilities.GetCipher("AES/ECB/NoPadding");
            cipher.Init(false, ParameterUtilities.CreateKeyParameter("AES", key));

            return cipher.DoFinal(data);
        }

        /// <inheritdoc />
        public override byte[] AesEncrypt(byte[] data, byte[] key, byte[] iv)
        {
            if (data == null || data.Length % 16 != 0) throw new ArgumentException(nameof(data));
            if (key == null || key.Length != 32) throw new ArgumentException(nameof(key));
            if (iv == null || iv.Length != 16) throw new ArgumentException(nameof(iv));

            var cipher = CipherUtilities.GetCipher("AES/CBC/NoPadding");
            cipher.Init(true, new ParametersWithIV(ParameterUtilities.CreateKeyParameter("AES", key), iv));

            return cipher.DoFinal(data);
        }

        /// <inheritdoc />
        public override byte[] AesDecrypt(byte[] data, byte[] key, byte[] iv)
        {
            if (data == null || data.Length % 16 != 0) throw new ArgumentException(nameof(data));
            if (key == null || key.Length != 32) throw new ArgumentException(nameof(key));
            if (iv == null || iv.Length != 16) throw new ArgumentException(nameof(iv));

            var cipher = CipherUtilities.GetCipher("AES/CBC/NoPadding");
            cipher.Init(false, new ParametersWithIV(ParameterUtilities.CreateKeyParameter("AES", key), iv));

            return cipher.DoFinal(data);
        }

        /// <inheritdoc />
        public override byte[] SCrypt(byte[] P, byte[] S, int N, int r, int p, int dkLen)
        {
            if (P == null) throw new ArgumentException(nameof(P));
            if (S == null) throw new ArgumentException(nameof(S));
            if ((N & (N - 1)) != 0 || N < 2 || N >= Math.Pow(2, 128 * r / 8)) throw new ArgumentException(nameof(N));
            if (r < 1) throw new ArgumentException(nameof(r));
            if (p < 1 || p > int.MaxValue / (128 * r * 8)) throw new ArgumentException(nameof(p));
            if (dkLen < 1) throw new ArgumentException(nameof(dkLen));

            return Org.BouncyCastle.Crypto.Generators.SCrypt.Generate(P, S, N, r, p, dkLen);
        }

        /// <inheritdoc />
        public override byte[] GenerateRandomBytes(int length)
        {
            if (length < 1) throw new ArgumentException(nameof(length));

            var privateKey = new byte[length]; 
            using (System.Security.Cryptography.RandomNumberGenerator rng = System.Security.Cryptography.RandomNumberGenerator.Create()) 
            { 
                rng.GetBytes(privateKey); 
            } 
 
            return privateKey; 
        }
    }
}