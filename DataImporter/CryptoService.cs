using System;
using System.Collections.Generic;
using System.Text;
using System.Security.Cryptography;
using System.IO;
using System.Net.Sockets;

namespace DataImporter
{
    public static class CryptoService
    {
        private static readonly RSACryptoServiceProvider RSAProvider = new RSACryptoServiceProvider();

        private static readonly TripleDESCryptoServiceProvider DESProvider = new TripleDESCryptoServiceProvider();

        private static Dictionary<int, byte[]> KeysDict = new Dictionary<int, byte[]>();

        static CryptoService()
        {
        }

        public static string GetPublicKey()
        {
            var key = RSAProvider.ExportRSAPublicKey();
            
            return Convert.ToBase64String(key);
        }

        public static void SetSymmetricKey(int id, string key)
        {
            var decryptedKey = RSAProvider.Decrypt(Convert.FromBase64String(key), false);

            KeysDict.Add(id, decryptedKey);

            DESProvider.Key = decryptedKey;
            DESProvider.IV = new byte[8];
        }

        public static string GetSymmetricKey()
        {
            return Convert.ToBase64String(DESProvider.Key);
        }

        public static string DecryptMessage(int id, string text)
        {
            try
            {
                DESProvider.Key = KeysDict[id];

                byte[] data = Convert.FromBase64String(text);

                MemoryStream msDecrypt = new MemoryStream(data);

                CryptoStream csDecrypt = new CryptoStream(msDecrypt,
                    DESProvider.CreateDecryptor(),
                    CryptoStreamMode.Read);

                byte[] fromEncrypt = new byte[data.Length];

                csDecrypt.Read(fromEncrypt, 0, fromEncrypt.Length);

                return new UTF8Encoding().GetString(fromEncrypt);
            }
            catch
            {
                Console.WriteLine("Incorrect encryption key!");
                return null;
            }
        }
    }
}
