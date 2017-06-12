using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Security.Cryptography;
using Windows.Security.Cryptography.Core;
using Windows.Storage.Streams;

namespace App1
{
    class securityAlgorithms
    {
      //public string keyGet;
        public securityAlgorithms()
        {
            // Initialize the Application.

            // Derive key material from a password-based key derivation function.
            String strKdfAlgName = KeyDerivationAlgorithmNames.Pbkdf2Sha256;
            UInt32 targetKeySize = 32;
            UInt32 iterationCount = 10000;
            IBuffer buffKeyMatl = this.SampleDeriveKeyMaterialPbkdf(
                strKdfAlgName,
                targetKeySize,
                iterationCount);

            // Create a key.
            CryptographicKey key = this.SampleCreateKDFKey(
                strKdfAlgName,
                buffKeyMatl);
          //  key.
            //keyGet = key.ToString();
        }

        public IBuffer SampleDeriveKeyMaterialPbkdf(
            String strAlgName,
            UInt32 targetKeySize,
            UInt32 iterationCount)
        {

            // Open the specified algorithm.
            KeyDerivationAlgorithmProvider objKdfProv = KeyDerivationAlgorithmProvider.OpenAlgorithm(strAlgName);

            // Demonstrate how to retrieve the algorithm name.
            String strAlgUsed = objKdfProv.AlgorithmName;

            // Create a buffer that contains the secret used during derivation.
            String strSecret = "MyPassword";
            IBuffer buffSecret = CryptographicBuffer.ConvertStringToBinary(strSecret, BinaryStringEncoding.Utf8);

            // Create a random salt value.
            IBuffer buffSalt = CryptographicBuffer.GenerateRandom(32);

            // Create the derivation parameters.
            KeyDerivationParameters pbkdf2Params = KeyDerivationParameters.BuildForPbkdf2(buffSalt, iterationCount);

            // Create a key from the secret value.
            CryptographicKey keyOriginal = objKdfProv.CreateKey(buffSecret);

            // Derive a key based on the original key and the derivation parameters.
            IBuffer keyMaterial = CryptographicEngine.DeriveKeyMaterial(
                keyOriginal,
                pbkdf2Params,
                targetKeySize);

            // Demonstrate checking the iteration count.
            UInt32 iterationCountOut = pbkdf2Params.IterationCount;

            // Demonstrate returning the derivation parameters to a buffer.
            IBuffer buffParams = pbkdf2Params.KdfGenericBinary;

            // return the KDF key material.
            return keyMaterial;
        }

        public CryptographicKey SampleCreateKDFKey(
            String strAlgName,
            IBuffer buffKeyMaterial)
        {
            // Create a KeyDerivationAlgorithmProvider object and open the specified algorithm.
            KeyDerivationAlgorithmProvider objKdfAlgProv = KeyDerivationAlgorithmProvider.OpenAlgorithm(strAlgName);

            // Create a key by using the KDF parameters.
            CryptographicKey key = objKdfAlgProv.CreateKey(buffKeyMaterial);

            return key;
        }
    }
}
