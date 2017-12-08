using Org.BouncyCastle.Asn1.Sec;
using Org.BouncyCastle.Asn1.X9;
using Org.BouncyCastle.Crypto.Digests;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Math;
using Org.BouncyCastle.Math.EC;
using System.Linq;

namespace CoinStealer
{
    /// <summary>
    /// https://kobl.one/blog/create-full-ethereum-keypair-and-address/
    /// </summary>
    public class EthAddress
    {
        public readonly byte[] PrivateKey, PublicKey;
        public readonly string Address;

        public EthAddress(byte[] privateKey)
        {
            PrivateKey = privateKey;
            PublicKey = ToPublicKey(privateKey);

            byte[] raw = CalculateHash(PublicKey);
            raw = raw.Skip(12).ToArray();
            Address = raw.ToHex();
        }

        static X9ECParameters curve = SecNamedCurves.GetByName("secp256k1");
        static ECDomainParameters domain = new ECDomainParameters(curve.Curve, curve.G, curve.N, curve.H);

        public static byte[] ToPublicKey(byte[] privateKey)
        {
            BigInteger d = new BigInteger(privateKey);
            ECPoint q = domain.G.Multiply(d);

            var publicParams = new ECPublicKeyParameters(q, domain);
            return publicParams.Q.GetEncoded(false).Skip(1).ToArray();
        }

        byte[] CalculateHash(byte[] data)
        {
            KeccakDigest digest = new KeccakDigest(256);
            byte[] raw = new byte[digest.GetDigestSize()];
            digest.BlockUpdate(data, 0, data.Length);
            digest.DoFinal(raw, 0);
            return raw;
        }
    }
}