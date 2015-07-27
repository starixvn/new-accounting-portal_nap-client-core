namespace Fanex.NAP.SignIn;

using System;
using System.IO;
using System.Security.Cryptography;

public static class Cryptography
{
#pragma warning disable S2278 // Neither DES (Data Encryption Standard) nor DESede (3DES) should be used
    private static readonly DESCryptoServiceProvider CryptoServiceProvider = new();
#pragma warning restore S2278 // Neither DES (Data Encryption 
    private static readonly byte[] RgbKey = [0x21, 0x63, 0x54, 0x5a, 0x91, 0xac, 110, 0xcc];
    private static readonly byte[] RgbIv = "&％#@?,:*!~^."u8.ToArray();

    public static string DecryptPassword(string encryptedPassword)
    {
        if (encryptedPassword == null)
        {
            throw new ArgumentNullException(nameof(encryptedPassword));
        }

        using var decryptor= CryptoServiceProvider.CreateDecryptor(RgbKey, RgbIv);
        var standardizedEncryptedPassword = StandardizeEncryptedPasswordLength(encryptedPassword);
        var encryptedPasswordInBytes = Convert.FromBase64String(standardizedEncryptedPassword);

        using var memoryStream = new MemoryStream(encryptedPasswordInBytes);
        var cryptoStream = new CryptoStream(memoryStream, decryptor, CryptoStreamMode.Read);

        using var streamReader = new StreamReader(cryptoStream);
        return streamReader.ReadToEnd().Trim();
    }

    private static string StandardizeEncryptedPasswordLength(string encryptedPassword)
    {
        if (string.IsNullOrWhiteSpace(encryptedPassword))
        {
            return string.Empty;
        }

        var allowedLength = encryptedPassword.Length >> 2 << 2;

        return encryptedPassword[..allowedLength];
    }
}