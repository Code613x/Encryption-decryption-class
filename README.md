# Encryption and decryption class made in c# .net framework 4.8.
> __Note__ 
> **If you are utilizing my program for commercial purposes, kindly provide attribution to my GitHub account.**

> __Warning__ 
> **I hereby declare that I am not liable for any damages caused by the program. If you experience any problems with my code, feel free to reach out to me on Discord.**


## About project
The Encryption and Decryption class created in C# .NET Framework 4.8 is a high-level implementation of symmetric key encryption using the Advanced Encryption Standard (AES) algorithm. The class provides methods to encrypt and decrypt files and folders, as well as plain text strings, using a user-provided password.

The encryption process uses a random salt to generate a secure key, which is derived from the user's password using the Password-Based Key Derivation Function 2 (PBKDF2) with a fixed number of iterations. The resulting key is used to encrypt the data using AES in Cipher Block Chaining (CBC) mode, with a random Initialization Vector (IV) generated for each encryption operation. The encrypted data is then saved to a file or returned as a byte array.

The decryption process uses the same salt and password to regenerate the key, and the IV to decrypt the data. The class also supports decryption of entire directories and their contents, as well as multiple files in one operation.

This Encryption and Decryption class is designed to be easy to use and integrate into existing C# .NET projects, with a focus on performance, security, and compatibility with the .NET Framework 4.8. It is suitable for a wide range of applications, such as protecting sensitive data, secure file transfer, and secure communication over networks.

## Contact
In case you require any assistance with your project, please do not hesitate to contact me on Discord. **Code613**

## Code
To encrypt a file, you can use the following code:
```c#
CypherCode613.EncryptFile(File name or path of the file, password);
```

To decrypt a file, you can use the following code:
```c#
CypherCode613.DecryptFile(File name or path of the file, password);
```

To encrypt the folder and all its subdirectories, use the following code:
```c#
CypherCode613.EncryptFolder(Path, Password);
```

To decrypt the folder and all its subdirectories, use the following code:
```c#
CypherCode613.DecryptDirectory(Path, Password);
```

To encrypt text, use the following code:
```c#
CypherCode613.EncryptText(text, Password);
```

To decrypt text, use the following code:
```c#
CypherCode613.DecryptText(text, Password);
```
