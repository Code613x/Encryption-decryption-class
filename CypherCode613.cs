using System;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

public class CypherCode613
{
    private const int SaltSize = 32;

    public static void EncryptFolder(string folderPath, string password)
    {
        // Przechodzimy przez wszystkie pliki w folderze i szyfrujemy każdy z nich
        foreach (string filePath in Directory.GetFiles(folderPath))
        {
            EncryptFile(filePath, password);
        }

        // Przechodzimy przez wszystkie podfoldery i rekurencyjnie szyfrujemy ich zawartość
        foreach (string subFolderPath in Directory.GetDirectories(folderPath))
        {
            EncryptFolder(subFolderPath, password);
        }
    }

    public static void EncryptFile(string inputFile, string password)
    {
        byte[] salt = GenerateRandomBytes(SaltSize);
        byte[] key = GenerateKey(password, salt);

        using (var aes = Aes.Create())
        {
            aes.Key = key;
            aes.GenerateIV();

            string outputFile = inputFile + ".encrypted";

            using (var fsInput = new FileStream(inputFile, FileMode.Open, FileAccess.Read))
            {
                using (var fsOutput = new FileStream(outputFile, FileMode.Create, FileAccess.Write))
                {
                    fsOutput.Write(salt, 0, SaltSize);
                    fsOutput.Write(aes.IV, 0, 16);

                    using (var cryptoStream = new CryptoStream(fsOutput, aes.CreateEncryptor(), CryptoStreamMode.Write))
                    {
                        fsInput.CopyTo(cryptoStream);
                    }
                }
            }

            File.Delete(inputFile);
            File.Move(outputFile, inputFile);
        }
    }

    private static byte[] GenerateRandomBytes(int size)
    {
        var randomBytes = new byte[size];

        using (var rng = new RNGCryptoServiceProvider())
        {
            rng.GetBytes(randomBytes); 
        }

        return randomBytes;
    }

    private static byte[] GenerateKey(string password, byte[] salt)
    {
        const int KeySize = 256;
        const int Iterations = 1000;

        var keyGenerator = new Rfc2898DeriveBytes(password, salt, Iterations);
        return keyGenerator.GetBytes(KeySize / 8);
    }

    public static void DecryptDirectory(string directoryPath, string password)
    {
        if (!Directory.Exists(directoryPath))
        {
            throw new DirectoryNotFoundException("Directory not found: " + directoryPath);
        }

        string[] files = Directory.GetFiles(directoryPath, "*.encrypted", SearchOption.AllDirectories);

        foreach (string file in files)
        {
            try
            {
                DecryptFile(file, password);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error decrypting file: " + file);
                Console.WriteLine(ex.Message);
            }
        }
    }
    public static void DecryptFile(string inputFile, string password)
    {
        byte[] salt = new byte[32];
        byte[] key = new byte[32];
        byte[] iv = new byte[16];

        using (FileStream fsIn = new FileStream(inputFile, FileMode.Open, FileAccess.Read))
        {
            fsIn.Read(salt, 0, salt.Length);
            Rfc2898DeriveBytes pbkdf2 = new Rfc2898DeriveBytes(password, salt, 10000);
            key = pbkdf2.GetBytes(32);
            fsIn.Read(iv, 0, iv.Length);

            using (Aes aes = Aes.Create())
            {
                aes.KeySize = 256;
                aes.Key = key;
                aes.IV = iv;

                using (CryptoStream cs = new CryptoStream(fsIn, aes.CreateDecryptor(), CryptoStreamMode.Read))
                {
                    string outputFile = Path.Combine(Path.GetDirectoryName(inputFile), Path.GetFileNameWithoutExtension(inputFile));
                    using (FileStream fsOut = new FileStream(outputFile, FileMode.Create, FileAccess.Write))
                    {
                        cs.CopyTo(fsOut);
                    }
                }
            }
        }

        File.Delete(inputFile);
    }
    public static string EncryptText(string text, string password)
    {
        byte[] salt = GenerateRandomBytes(SaltSize);
        byte[] key = GenerateKey(password, salt);

        using (var aes = Aes.Create())
        {
            aes.Key = key;
            aes.GenerateIV();

            using (var msOutput = new MemoryStream())
            {
                msOutput.Write(salt, 0, SaltSize);
                msOutput.Write(aes.IV, 0, 16);

                using (var cryptoStream = new CryptoStream(msOutput, aes.CreateEncryptor(), CryptoStreamMode.Write))
                {
                    byte[] bytesToEncrypt = Encoding.UTF8.GetBytes(text);
                    cryptoStream.Write(bytesToEncrypt, 0, bytesToEncrypt.Length);
                }

                return Convert.ToBase64String(msOutput.ToArray());
            }
        }
    }

    public static string DecryptText(string encryptedText, string password)
    {
        byte[] encryptedBytes = Convert.FromBase64String(encryptedText);
        byte[] salt = encryptedBytes.Take(SaltSize).ToArray();
        byte[] iv = encryptedBytes.Skip(SaltSize).Take(16).ToArray();
        byte[] key = GenerateKey(password, salt);

        using (var aes = Aes.Create())
        {
            aes.Key = key;
            aes.IV = iv;

            using (var msInput = new MemoryStream(encryptedBytes, SaltSize + 16, encryptedBytes.Length - SaltSize - 16))
            {
                using (var cryptoStream = new CryptoStream(msInput, aes.CreateDecryptor(), CryptoStreamMode.Read))
                {
                    using (var srDecrypt = new StreamReader(cryptoStream))
                    {
                        return srDecrypt.ReadToEnd();
                    }
                }
            }
        }
    }
}
