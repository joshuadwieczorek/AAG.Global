using System;
using System.Text;
using System.Security.Cryptography;

namespace AAG.Global.Security
{
    public sealed class CryptographyProvider
    {
        private string key;
        private byte[] iv;
        private int saltByteLength;
        private int passwordByteLength;
        private int hashIterations;


        /// <summary>
        /// AES 256 Encrypter.
        /// </summary>
        /// <param name="securityConfiguration"></param>
        public CryptographyProvider(SecurityConfiguration securityConfiguration)
        {
            key = securityConfiguration.Key;
            SetIV(securityConfiguration.IV);
            saltByteLength = 100;
            passwordByteLength = 100;
            hashIterations = 100000;
        }


        /// <summary>
        /// Encrypt string.
        /// </summary>
        /// <param name="plainTextString">Plain text string to encrypt.</param>
        /// <returns></returns>
        public string Encrypt(string plainTextString)
        {
            if (string.IsNullOrEmpty(plainTextString))
                return plainTextString;

            using AesCryptoServiceProvider aes = CreateProvider();
            byte[] plainTextBytes = UTF8Encoding.UTF8.GetBytes(plainTextString);
            ICryptoTransform enc = aes.CreateEncryptor();
            byte[] bytes = enc.TransformFinalBlock(plainTextBytes, 0, plainTextBytes.Length);
            return Convert.ToBase64String(bytes);
        }


        /// <summary>
        /// Decrypt string to plain text.
        /// </summary>
        /// <param name="encryptedString">Encrypted string to decrypt.</param>
        /// <returns></returns>
        public string Decrypt(string encryptedString)
        {
            if (string.IsNullOrEmpty(encryptedString))
                return encryptedString;

            using AesCryptoServiceProvider aes = CreateProvider();
            byte[] encryptedBytes = Convert.FromBase64String(encryptedString);
            ICryptoTransform dec = aes.CreateDecryptor();
            byte[] bytes = dec.TransformFinalBlock(encryptedBytes, 0, encryptedBytes.Length);
            return UTF8Encoding.UTF8.GetString(bytes);
        }


        /// <summary>
        /// Create AES service provider.
        /// </summary>
        /// <returns></returns>
        private AesCryptoServiceProvider CreateProvider()
        {
            return new AesCryptoServiceProvider()
            {
                BlockSize = 128,
                KeySize = 256,
                Key = UTF8Encoding.UTF8.GetBytes(key.Substring(29, 32)),
                IV = iv,
                Mode = CipherMode.CBC
            };
        }


        /// <summary>
        /// Set encryption/decryption IV.
        /// </summary>
        /// <param name="iv"></param>
        private void SetIV(string iv)
        {
            this.iv = new byte[16] { 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0 };
        }


        /// <summary>
        /// Create hash string.
        /// </summary>
        /// <param name="plainTextString"></param>
        /// <returns></returns>
        public string CreateHash(string plainTextString)
        {
            try
            {
                byte[] saltBytes;
                byte[] hashedTextBytes;
                byte[] finalHash = new byte[200];

                new RNGCryptoServiceProvider().GetBytes(saltBytes = new byte[saltByteLength]);

                using Rfc2898DeriveBytes hashProvider = new Rfc2898DeriveBytes(plainTextString, saltBytes, hashIterations);
                hashedTextBytes = hashProvider.GetBytes(passwordByteLength);

                Array.Copy(saltBytes, 0, finalHash, 0, saltByteLength);
                Array.Copy(hashedTextBytes, 0, finalHash, saltByteLength, passwordByteLength);

                return Convert.ToBase64String(finalHash);
            }
            catch (Exception e)
            {
                CryptographyException cryptoException = new CryptographyException("Hasher create exception!", e);
                cryptoException.Data.Add("PlainTextString", plainTextString);
                throw cryptoException;
            }
        }


        /// <summary>
        /// Verify hash against plain text value.
        /// </summary>
        /// <param name="plainTextString"></param>
        /// <param name="hashedString"></param>
        /// <returns></returns>
        public bool VerifyHash(
              string plainTextString
            , string hashedString)
        {
            try
            {
                byte[] saltBytes = new byte[saltByteLength];
                byte[] originalHashBytes = new byte[passwordByteLength];
                byte[] hashedTextBytes = Convert.FromBase64String(hashedString);
                byte[] compareHash;

                Array.Copy(hashedTextBytes, 0, saltBytes, 0, saltByteLength);
                Array.Copy(hashedTextBytes, saltByteLength, originalHashBytes, 0, passwordByteLength);

                using Rfc2898DeriveBytes hashProvider = new Rfc2898DeriveBytes(plainTextString, saltBytes, hashIterations);
                compareHash = hashProvider.GetBytes(passwordByteLength);

                Boolean isCorrect = true;

                for (int i = 0; i < 20; ++i)
                    if (originalHashBytes[i + 4] != compareHash[i + 4])
                    {
                        isCorrect = false;
                        break;
                    }

                return isCorrect;

            }
            catch (Exception e)
            {
                CryptographyException cryptoException = new CryptographyException("Hasher verify exception!", e);
                cryptoException.Data.Add("PlainTextString", plainTextString);
                cryptoException.Data.Add("HashedString", hashedString);
                throw cryptoException;
            }
        }


        /// <summary>
        /// Set byte length of salt byte array. Default value is 100.
        /// </summary>
        /// <param name="saltByteLength"></param>
        public void SetSaltByteLength(int saltByteLength)
            => this.saltByteLength = saltByteLength;


        /// <summary>
        /// Set byte length of password byte array. Default value is 100.
        /// </summary>
        /// <param name="passwordByteLength"></param>
        public void SetPasswordByteLength(int passwordByteLength)
            => this.passwordByteLength = passwordByteLength;



        /// <summary>
        /// Set hash iterations. Default value is 100,000.
        /// </summary>
        /// <param name="hashIterations"></param>
        public void SetHashIterations(int hashIterations)
            => this.hashIterations = hashIterations;
    }
}