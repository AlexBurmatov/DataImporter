using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace SenderApp
{
    public static class CryptoService
    {
        private static readonly RSACryptoServiceProvider RSAProvider = new RSACryptoServiceProvider();

        private static readonly TripleDESCryptoServiceProvider DESProvider = new TripleDESCryptoServiceProvider();

        static CryptoService()
        {
            DESProvider.GenerateKey();
            DESProvider.IV = new byte[8];
        }

        public static void SetPublicKey(string info)
        {
            var key = Convert.FromBase64String(info);

            RSAProvider.ImportRSAPublicKey(new ReadOnlySpan<byte>(key, 0, key.Length), out int p);
        }

        public static string GetEncryptedSymmetricKey()
        {
            var encryptedKey = RSAProvider.Encrypt(DESProvider.Key, false);

            return Convert.ToBase64String(encryptedKey);
        }

        public static string GetSymmetricKey()
        {
            return Convert.ToBase64String(DESProvider.Key);
        }

        public static string EncryptMessage(string text)
        {
            MemoryStream mStream = new MemoryStream();

            // Create a CryptoStream using the MemoryStream 
            // and the passed key and initialization vector (IV).
            CryptoStream cStream = new CryptoStream(mStream,
                DESProvider.CreateEncryptor(),
                CryptoStreamMode.Write);

            // Convert the passed string to a byte array.
            byte[] toEncrypt = new ASCIIEncoding().GetBytes(text);

            // Write the byte array to the crypto stream and flush it.
            cStream.Write(toEncrypt, 0, toEncrypt.Length);
            cStream.FlushFinalBlock();

            // Get an array of bytes from the 
            // MemoryStream that holds the 
            // encrypted data.
            byte[] ret = mStream.ToArray();

            // Close the streams.
            cStream.Close();
            mStream.Close();

            // Return the encrypted buffer.
            return Convert.ToBase64String(ret);
        }
    }
}
