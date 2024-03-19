using Emeint.Core.BE.Domain.Enums;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace Emeint.Core.BE.Domain.Managers
{
    public static class HashingManager
    {
        public static string HashMessage(string messageToHash, HashAlgorithm hashAlgorithm, string secureKey, TextEncodingType format = TextEncodingType.Base64)
        {
            try
            {

                messageToHash = messageToHash + secureKey;

                byte[] data = Encoding.UTF8.GetBytes(messageToHash);
                using (hashAlgorithm)
                {
                    byte[] encryptedBytes = hashAlgorithm.TransformFinalBlock(data, 0, data.Length);

                    if (format == TextEncodingType.Hex)
                        return Convert.ToHexString(hashAlgorithm.Hash);
                    else return Convert.ToBase64String(hashAlgorithm.Hash);

                }
            }
            catch (Exception ex)
            {
                //Handle Exception Business Logic
            }
            return messageToHash;
        }
    }
}
