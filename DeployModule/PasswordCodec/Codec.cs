using System;
using System.Security.Cryptography;
using System.Text;

namespace PasswordCodec
{
    public class Codec
    {
        private static string keyEncode = Environment.GetEnvironmentVariable("MasterKey");

        public static string EncryptText(string text)
        {
            if (isKeyEncode())
                throw new ArgumentNullException("Environment Variable is missing");
            return Encrypter.EncryptString(text, Encoder.DecodeBase64(keyEncode));
        }
        public static string DecryptText(string text)
        {
            if (isKeyEncode())
                throw new ArgumentNullException("Environment Variable is missing");
            return Encrypter.DecryptString(text, Encoder.DecodeBase64(keyEncode));
        }
        public static string EncodeText(string text)
        {
            return Encoder.EncodeBase64(text);
        }
        private static bool isKeyEncode()
        {
            return keyEncode == null ? true : false;
        }
    }
    internal class Encoder
    {
        internal static string EncodeBase64(string text)
        {
            if (text == null)
            {
                return null;
            }

            return Convert.ToBase64String(Encoding.UTF8.GetBytes(text));
        }
        internal static string DecodeBase64(string text)
        {
            if (text == null)
            {
                throw new ArgumentNullException("text");
            }

            byte[] bytes = Convert.FromBase64String(text);
            return Encoding.UTF8.GetString(bytes);
        }
    }
    internal class Encrypter
    {
        internal static string EncryptString(string text, string secureKey)
        {
            string rezult = null;
            if (secureKey == null)
                return null;
            byte[] encrypt = Encoding.UTF8.GetBytes(text);
            using (MD5CryptoServiceProvider md5Service = new MD5CryptoServiceProvider())
            {
                byte[] key = md5Service.ComputeHash(Encoding.UTF8.GetBytes(secureKey));
                using (TripleDESCryptoServiceProvider desService = new TripleDESCryptoServiceProvider())
                {
                    desService.Key = key;
                    desService.Mode = CipherMode.ECB;
                    desService.Padding = PaddingMode.PKCS7;
                    using (ICryptoTransform cryptoTransform = desService.CreateEncryptor())
                    {
                        byte[] rezultArray = cryptoTransform.TransformFinalBlock(encrypt, 0, encrypt.Length);
                        rezult = Convert.ToBase64String(rezultArray, 0, rezultArray.Length);
                    }
                }
            }
            return rezult;
        }
        internal static string DecryptString(string text, string secureKey)
        {
            string rezult = null;
            if (secureKey == null)
                return null;
            byte[] bytes = Convert.FromBase64String(text);
            using (MD5CryptoServiceProvider md5Service = new MD5CryptoServiceProvider())
            {
                byte[] key = md5Service.ComputeHash(Encoding.UTF8.GetBytes(secureKey));
                using (TripleDESCryptoServiceProvider desService = new TripleDESCryptoServiceProvider())
                {
                    desService.Key = key;
                    desService.Mode = CipherMode.ECB;
                    desService.Padding = PaddingMode.PKCS7;
                    using (ICryptoTransform cryptoTransform = desService.CreateDecryptor())
                    {
                        byte[] rezultArray = cryptoTransform.TransformFinalBlock(bytes, 0, bytes.Length);
                        rezult = Encoding.UTF8.GetString(rezultArray);
                    }
                }
            }
            return rezult;
        }
    }
}
