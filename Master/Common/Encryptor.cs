
using System.Security.Cryptography;
using System.Text;

namespace Common
{
    /// <summary>
    /// Class used to enctypt/decrypt text
    /// </summary>
    public static class Encryptor
    {
        /// <summary>
        /// Encrypt string
        /// </summary>
        /// <param name="key">Key</param>
        /// <param name="plainText">Text</param>
        /// <returns>Encrypted string</returns>
        public static string EncryptString(string key, string plainText)
        {
            byte[] iv = new byte[16];
            byte[] array;
            byte[] keyArray = new byte[16];
            byte[] keyBytes = Encoding.UTF8.GetBytes(key);

            for (int i = 0; i < keyBytes.Length && i < keyArray.Length; i++)
            {
                keyArray[i] = keyBytes[i];
            }

            using (Aes aes = Aes.Create())
            {
                aes.Key = keyArray;
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

        /// <summary>
        /// Decrypt string
        /// </summary>
        /// <param name="key">Key</param>
        /// <param name="cipherText">Text</param>
        /// <returns>Decrypted string</returns>
        public static string DecryptString(string key, string cipherText)
        {
            byte[] iv = new byte[16];
            byte[] buffer = Convert.FromBase64String(cipherText);
            byte[] keyArray = new byte[16];
            byte[] keyBytes = Encoding.UTF8.GetBytes(key);

            for (int i = 0; i < keyBytes.Length && i < keyArray.Length; i++)
            {
                keyArray[i] = keyBytes[i];
            }

            using (Aes aes = Aes.Create())
            {
                aes.Key = keyArray;
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
    }
}
