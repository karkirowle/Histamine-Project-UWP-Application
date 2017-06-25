using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Security.Cryptography;
using Windows.Security.Cryptography.Core;
using Windows.Storage.Streams;
using System.Diagnostics;


namespace App1
{
    // TODO: Implement this class to create a public and private key for our new meat doctor
    class keyGenerator
    {
        public keyGenerator(string pass, out string publicToUpload, out string privateToUpload, out string hash, out string salt)
        {
            // Create an asymmetric key pair.
            String strAsymmetricAlgName = AsymmetricAlgorithmNames.RsaPkcs1;
            UInt32 asymmetricKeyLength = 512;
            IBuffer buffPublicKey; // public
            IBuffer buffKeyPair;
            this.SampleCreateAsymmetricKeyPair(
                strAsymmetricAlgName,
                asymmetricKeyLength,
                out buffPublicKey, out buffKeyPair);

            // Public key simply uploaded as a BE string
           // publicToUpload = CryptographicBuffer.ConvertBinaryToString(BinaryStringEncoding.Utf16BE, buffPublicKey);
            publicToUpload = CryptographicBuffer.EncodeToBase64String(buffPublicKey);

            // Private key -  encrypt with pass
            string strKeyPair = CryptographicBuffer.EncodeToBase64String(buffKeyPair);
            privateToUpload = Encrypt(strKeyPair, pass);
            string encryptedString, decryptedString;
            AsymmetricEncrypt("kutya", publicToUpload, out encryptedString);
            AsymmetricDecrypt(strKeyPair, encryptedString, out decryptedString);
            Debug.WriteLine(decryptedString);
            salt = GenerateRandomData();
            string saltedPass = pass + salt;

            // Hash pashword with salt
            IBuffer hashBuffer = GetMD5Hash(saltedPass);
            // everything to  base64
            hash = CryptographicBuffer.EncodeToBase64String(hashBuffer);
           // CryptographicEngine.Encrypt(
        }

        public static void AsymmetricEncrypt(
           string stringToEncrypt,
           string publicKey,
           out string encryptedString)
        {
            IBuffer buffPublicKey = CryptographicBuffer.DecodeFromBase64String(publicKey);
            //IBuffer buffPublicKey = CryptographicBuffer.ConvertStringToBinary(publicKey, BinaryStringEncoding.Utf16BE);
            AsymmetricEncrypt(stringToEncrypt, buffPublicKey, out encryptedString);
        }
        public static void AsymmetricEncrypt(
           string stringToEncrypt,
           IBuffer buffPublicKey,
           out string encryptedString)
        {
            // Open the algorithm provider for the specified asymmetric algorithm.
            AsymmetricKeyAlgorithmProvider objAlgProv = AsymmetricKeyAlgorithmProvider.OpenAlgorithm(AsymmetricAlgorithmNames.RsaPkcs1);

            // Import the public key from a buffer.
            CryptographicKey publicKey = objAlgProv.ImportPublicKey(buffPublicKey);

            // Convert string to encrypt to big endian
            IBuffer buffStringToEncrypt = CryptographicBuffer.ConvertStringToBinary(stringToEncrypt, BinaryStringEncoding.Utf16BE);

            // Encrypt the session key by using the public key.
            IBuffer encryptedBuffer = CryptographicEngine.Encrypt(publicKey, buffStringToEncrypt, null);

            // Encode encrypted string to base64 
            encryptedString = CryptographicBuffer.EncodeToBase64String(encryptedBuffer);
        }
        public static void AsymmetricDecrypt(
        string privateKey,
        string cipherString, out string decryptedString)
        {
            // Was encoded in base64
            IBuffer privateBuffer = CryptographicBuffer.DecodeFromBase64String(privateKey);
            AsymmetricDecrypt(privateBuffer, cipherString, out decryptedString);
        }
        public static void AsymmetricDecrypt(
         IBuffer privateKey,
         string  cipherString, out string decryptedString)
        {
            // Open the algorithm provider for the specified asymmetric algorithm.
            AsymmetricKeyAlgorithmProvider objAsymmAlgProv = AsymmetricKeyAlgorithmProvider.OpenAlgorithm(AsymmetricAlgorithmNames.RsaPkcs1);

            // Import the public key from a buffer. You should keep your private key
            // secure. For the purposes of this example, however, the private key is
            // just stored in a static class variable.
            CryptographicKey keyPair = objAsymmAlgProv.ImportKeyPair(privateKey);

            IBuffer cipherBuffer = CryptographicBuffer.DecodeFromBase64String(cipherString);

            // Use the private key embedded in the key pair to decrypt the session key.
            IBuffer buffDecryptedString = CryptographicEngine.Decrypt(keyPair, cipherBuffer, null);

            // Convert back from big endian to normal text
            decryptedString = CryptographicBuffer.ConvertBinaryToString(BinaryStringEncoding.Utf16BE, buffDecryptedString);

       
        }



        public static Boolean verifyUser(string user, string password, string hash, string salt)
        {

            string saltedPass = password + salt;
            IBuffer hash1Buffer = GetMD5Hash(saltedPass);
            IBuffer hash2Buffer = CryptographicBuffer.DecodeFromBase64String(hash);
            Boolean bVal_1 = CryptographicBuffer.Compare(hash1Buffer, hash2Buffer);
            return bVal_1;
        }

        public string GenerateRandomData()
        {
            // Define the length, in bytes, of the buffer.
            uint length = 32;

            // Generate random data and copy it to a buffer.
            IBuffer buffer = CryptographicBuffer.GenerateRandom(length);

            // Encode the buffer to a hexadecimal string (for display).
            string randomHex = CryptographicBuffer.EncodeToHexString(buffer);

            return randomHex;
        }


        private static IBuffer GetMD5Hash(string key)
        {
            // Convert the message string to binary data.
            IBuffer buffUtf8Msg = CryptographicBuffer.ConvertStringToBinary(key, BinaryStringEncoding.Utf16BE);

            // Create a HashAlgorithmProvider object.
            HashAlgorithmProvider objAlgProv = HashAlgorithmProvider.OpenAlgorithm(HashAlgorithmNames.Md5);

            // Hash the message.
            IBuffer buffHash = objAlgProv.HashData(buffUtf8Msg);

            // Verify that the hash length equals the length specified for the algorithm.
            if (buffHash.Length != objAlgProv.HashLength)
            {
                throw new Exception("There was an error creating the hash");
            }

            return buffHash;
        }

        /// <summary>
        /// Encrypt a string using dual encryption method. Returns an encrypted text.
        /// </summary>
        /// <param name="toEncrypt">String to be encrypted</param>
        /// <param name="key">Unique key for encryption/decryption</param>m>
        /// <returns>Returns encrypted string.</returns>
        public static string Encrypt(string toEncrypt, string key)
        {
            try
            {
                // Get the MD5 key hash (you can as well use the binary of the key string)
                var keyHash = GetMD5Hash(key);

                // Create a buffer that contains the encoded message to be encrypted.
                var toDecryptBuffer = CryptographicBuffer.ConvertStringToBinary(toEncrypt, BinaryStringEncoding.Utf8);

                // Open a symmetric algorithm provider for the specified algorithm.
                var aes = SymmetricKeyAlgorithmProvider.OpenAlgorithm(SymmetricAlgorithmNames.AesEcbPkcs7);

                // Create a symmetric key.
                var symetricKey = aes.CreateSymmetricKey(keyHash);

                // The input key must be securely shared between the sender of the cryptic message
                // and the recipient. The initialization vector must also be shared but does not
                // need to be shared in a secure manner. If the sender encodes a message string
                // to a buffer, the binary encoding method must also be shared with the recipient.
                var buffEncrypted = CryptographicEngine.Encrypt(symetricKey, toDecryptBuffer, null);

                // Convert the encrypted buffer to a string (for display).
                // We are using Base64 to convert bytes to string since you might get unmatched characters
                // in the encrypted buffer that we cannot convert to string with UTF8.
                var strEncrypted = CryptographicBuffer.EncodeToBase64String(buffEncrypted);

                return strEncrypted;
            }
            catch (Exception ex)
            {
                // MetroEventSource.Log.Error(ex.Message);
                return "";
            }
        }

        public void SampleCreateAsymmetricKeyPair(
          String strAsymmetricAlgName,
          UInt32 keyLength,
          out IBuffer buffPublicKey, out IBuffer buffKeyPair)
        {
            // Open the algorithm provider for the specified asymmetric algorithm.
            AsymmetricKeyAlgorithmProvider objAlgProv = AsymmetricKeyAlgorithmProvider.OpenAlgorithm(strAsymmetricAlgName);

            // Demonstrate use of the AlgorithmName property (not necessary to create a key pair).
            String strAlgName = objAlgProv.AlgorithmName;

            // Create an asymmetric key pair.
            CryptographicKey keyPair = objAlgProv.CreateKeyPair(keyLength);

            // Export the public key to a buffer for use by others.
            buffPublicKey = keyPair.ExportPublicKey();


            // You should keep your private key (embedded in the key pair) secure. For  
            // the purposes of this example, however, we're just copying it into a
            // static class variable for later use during decryption.
            buffKeyPair = keyPair.Export();
        }

        public static string DecryptB(string cipherString, string key)
        {
            return Decrypt(cipherString, key);
        }
        /// <summary>
        /// Decrypt a string using dual encryption method. Return a Decrypted clear string
        /// </summary>
        /// <param name="cipherString">Encrypted string</param>
        /// <param name="key">Unique key for encryption/decryption</param>
        /// <returns>Returns decrypted text.</returns>
        public static string Decrypt(string cipherString, string key)
        {
            try
            {
                // Get the MD5 key hash (you can as well use the binary of the key string)
                var keyHash = GetMD5Hash(key);

                // Create a buffer that contains the encoded message to be decrypted.
                IBuffer toDecryptBuffer = CryptographicBuffer.DecodeFromBase64String(cipherString);

                // Open a symmetric algorithm provider for the specified algorithm.
                SymmetricKeyAlgorithmProvider aes = SymmetricKeyAlgorithmProvider.OpenAlgorithm(SymmetricAlgorithmNames.AesEcbPkcs7);

                // Create a symmetric key.
                var symetricKey = aes.CreateSymmetricKey(keyHash);

                var buffDecrypted = CryptographicEngine.Decrypt(symetricKey, toDecryptBuffer, null);

                string strDecrypted = CryptographicBuffer.ConvertBinaryToString(BinaryStringEncoding.Utf8, buffDecrypted);

                return strDecrypted;
            }
            catch (Exception ex)
            {
                // MetroEventSource.Log.Error(ex.Message);
                //throw;
                return "";
            }
        }
    }
}
