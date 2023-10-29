using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace WMC.Logic
{
    public class SecurityUtil
    {
        static string key = "m1ocn5n9iawee1m3obveeec2o1ian9s6";

        public static string Encrypt(string plainText)
        {
            byte[] iv = new byte[16];
            byte[] array;

            using (Aes aes = Aes.Create())
            {
                aes.Key = Encoding.UTF8.GetBytes(key);
                aes.IV = iv;

                ICryptoTransform encryptor = aes.CreateEncryptor(aes.Key, aes.IV);

                using (MemoryStream memoryStream = new MemoryStream())
                {
                    using (CryptoStream cryptoStream = new CryptoStream((Stream)memoryStream, encryptor, CryptoStreamMode.Write))
                    {
                        using (StreamWriter streamWriter = new StreamWriter((Stream)cryptoStream))
                        {
                            streamWriter.Write(plainText);
                        }

                        array = memoryStream.ToArray();
                    }
                }
            }

            return Convert.ToBase64String(array);
        }

        public static string Decrypt(string cipherText)
        {
            byte[] iv = new byte[16];
            byte[] buffer = Convert.FromBase64String(cipherText);

            using (Aes aes = Aes.Create())
            {
                aes.Key = Encoding.UTF8.GetBytes(key);
                aes.IV = iv;
                ICryptoTransform decryptor = aes.CreateDecryptor(aes.Key, aes.IV);

                using (MemoryStream memoryStream = new MemoryStream(buffer))
                {
                    using (CryptoStream cryptoStream = new CryptoStream((Stream)memoryStream, decryptor, CryptoStreamMode.Read))
                    {
                        using (StreamReader streamReader = new StreamReader((Stream)cryptoStream))
                        {
                            return streamReader.ReadToEnd();
                        }
                    }
                }
            }
        }

        #region crypto

        private const int SALT_BYTE_LENGTH = 32;
        private const int PBKDF2_ITERATIONS = 500;
        private const int PASSWORD_BYTE_LENGTH = 24;

        /// <summary>The validate password.</summary>
        /// <param name="password">The password.</param>
        /// <param name="salt">The salt.</param>
        /// <param name="hash">The hash.</param>
        /// <returns>The <see cref="bool"/>.</returns>
        public static bool ValidatePassword(string password, string salt, string hash)
        {
            byte[] saltBytes = Convert.FromBase64String(salt);
            byte[] rsa_hash = GenerateHash(password, saltBytes);
            string rsaText = Convert.ToBase64String(rsa_hash);

            return rsaText == hash;
        }

        /// <summary>The encrypt password.</summary>
        /// <param name="password">The password.</param>
        /// <param name="salt">The salt.</param>
        /// <returns>The <see cref="string"/>.</returns>
        public static string EncryptPassword(string password, out string salt)
        {
            var saltBytes = GenerateSalt();
            salt = Convert.ToBase64String(saltBytes);
            byte[] rsa_hash = GenerateHash(password, saltBytes);
            return Convert.ToBase64String(rsa_hash);
        }

        private static byte[] GenerateHash(string password, byte[] saltBytes)
        {
            var toBeHashed = Encoding.UTF8.GetBytes(password);
            using (var sha256 = SHA256.Create())
            {
                var combinedHash = Combine(toBeHashed, saltBytes);
                var rsa_hash = sha256.ComputeHash(combinedHash);
                return rsa_hash;
            }
        }

        private static byte[] GenerateSalt()
        {
            using (var randomNumberGenerator = new RNGCryptoServiceProvider())
            {
                var randomNumber = new byte[SALT_BYTE_LENGTH];
                randomNumberGenerator.GetBytes(randomNumber);

                return randomNumber;
            }
        }

        private static byte[] Combine(byte[] first, byte[] second)
        {
            var ret = new byte[first.Length + second.Length];

            Buffer.BlockCopy(first, 0, ret, 0, first.Length);
            Buffer.BlockCopy(second, 0, ret, first.Length, second.Length);

            return ret;
        }

        #endregion

    }
}
