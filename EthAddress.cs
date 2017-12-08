using Org.BouncyCastle.Crypto.Digests;
using System.Linq;

namespace CoinStealer
{
    public class EthAddress
    {
        public readonly byte[] PrivateKey, PublicKey;
        public readonly string Address;

        public EthAddress(byte[] privateKey, byte[] publicKey)
        {
            PrivateKey = privateKey;
            PublicKey = publicKey;

            byte[] raw = CalculateHash(PublicKey);

            raw = raw.Skip(12).ToArray();
            Address = raw.ToHex();
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