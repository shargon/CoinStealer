using System.Text;

namespace CoinStealer
{
    public static class ByteArrayExtensions
    {
        public static string ToHex(this byte[] privateKey)
        {
            StringBuilder sb = new StringBuilder();
            foreach (byte b in privateKey)
                sb.Append(b.ToString("x2"));

            return sb.ToString();
        }
    }
}