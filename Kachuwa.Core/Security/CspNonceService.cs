using System;
using System.Security.Cryptography;

namespace Kachuwa.Security
{
    public class CspNonceService : ICspNonceService
    {
        private static readonly RandomNumberGenerator _rng = RandomNumberGenerator.Create();
        private readonly string _nonce;

        public CspNonceService(int nonceByteAmount = 32)
        {
            byte[] nonceBytes = new byte[nonceByteAmount];
            _rng.GetBytes(nonceBytes);
            _nonce = Convert.ToBase64String(nonceBytes);
        }

        public string GetNonce()
        {
            return _nonce;
        }
    }
}