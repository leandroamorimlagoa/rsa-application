using System;
using System.Linq;
using System.Text;

namespace rsa_application.Tools
{
    public static class CustomConverter
    {
        #region ByteArrayToHex
        public static string ByteArrayToHex(byte[] bytes)
        {
            StringBuilder sb = new StringBuilder();
            foreach (byte b in bytes)
                sb.Append(b.ToString("X2"));

            string hexString = sb.ToString().ToUpper();
            return hexString;
        }
        #endregion

        #region HexToByteArray
        public static byte[] HexToByteArray(string hex)
            => Enumerable.Range(0, hex.Length)
                     .Where(x => x % 2 == 0)
                     .Select(x => Convert.ToByte(hex.Substring(x, 2), 16))
                     .ToArray();
        #endregion

        #region ByteArrayToBase64String
        public static string ByteArrayToBase64String(byte[] bytes)
            => Convert.ToBase64String(bytes);
        #endregion

        #region Base64StringToByteArray
        public static byte[] Base64StringToByteArray(string text)
            => Convert.FromBase64String(text);
        #endregion
    }
}
