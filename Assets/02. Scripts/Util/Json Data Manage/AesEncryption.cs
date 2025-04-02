using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

public static class AesEncryption // 문의 : 수민
{
    //private static string key = "1234567890123456";
    private const string KEY = "1234567890123456";
    //private static string iv = "1234567890123456";  // 16-byte IV (128-bit)

    public static string Encrypt(string plainText)
    {
        using (Aes aesAlg = Aes.Create())
        {
            aesAlg.Key = Encoding.UTF8.GetBytes(KEY);
            aesAlg.GenerateIV();
            byte[] iv = aesAlg.IV;

            ICryptoTransform encryptor = aesAlg.CreateEncryptor(aesAlg.Key, iv);

            using (MemoryStream ms = new MemoryStream())
            {
                ms.Write(iv, 0, iv.Length);

                using (CryptoStream cs = new CryptoStream(ms, encryptor, CryptoStreamMode.Write))
                {
                    using (StreamWriter sw = new StreamWriter(cs))
                    {
                        sw.Write(plainText);
                    }
                }
                return Convert.ToBase64String(ms.ToArray());
            }
        }
    }

    public static string Decrypt(string cipherText)
    {
        using (Aes aesAlg = Aes.Create())
        {
            aesAlg.Key = Encoding.UTF8.GetBytes(KEY);
            byte[] fullCipher = Convert.FromBase64String(cipherText);
            byte[] iv = new byte[16];
            Array.Copy(fullCipher, 0, iv, 0, iv.Length);

            ICryptoTransform decryptor = aesAlg.CreateDecryptor(aesAlg.Key, iv);

            using (MemoryStream ms = new MemoryStream(fullCipher, iv.Length, fullCipher.Length - iv.Length))
            {
                using (CryptoStream cs = new CryptoStream(ms, decryptor, CryptoStreamMode.Read))
                {
                    using (StreamReader sr = new StreamReader(cs))
                    {
                        return sr.ReadToEnd();
                    }
                }
            }
        }
    }
}
