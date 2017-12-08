using System.Globalization;
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

        public static byte[] FromHex(this string input)
        {
            if (input.StartsWith("0x"))
                input = input.Substring(2);

            byte[] bl = new byte[input.Length / 2];
            for (int x = 0, m = input.Length; x < m; x += 2)
            {
                bl[x / 2] = byte.Parse(input.Substring(x, 2), NumberStyles.HexNumber, CultureInfo.InvariantCulture);
            }

            return bl;
        }
    }
}