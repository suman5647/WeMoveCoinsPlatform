using Org.BouncyCastle.Math;
using System;

namespace WMC.Web.Utilities
{
    public class Base58
    {
        /// <summary>
        /// Converts a base-58 string to a byte array, returning null if it wasn't valid.
        /// </summary>
        public static byte[] ToByteArray(string base58)
        {
            BigInteger bi2 = new BigInteger("0");
            string b58 = "123456789ABCDEFGHJKLMNPQRSTUVWXYZabcdefghijkmnopqrstuvwxyz";

            foreach (char c in base58)
                if (b58.IndexOf(c) != -1)
                {
                    bi2 = bi2.Multiply(new BigInteger("58"));
                    bi2 = bi2.Add(new BigInteger(b58.IndexOf(c).ToString()));
                }
                else
                    return null;

            byte[] bb = bi2.ToByteArrayUnsigned();

            // interpret leading '1's as leading zero bytes
            foreach (char c in base58)
            {
                if (c != '1') break;
                byte[] bbb = new byte[bb.Length + 1];
                Array.Copy(bb, 0, bbb, 1, bb.Length);
                bb = bbb;
            }

            return bb;
        }
    }
}