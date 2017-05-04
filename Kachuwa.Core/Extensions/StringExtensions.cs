using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace Kachuwa.Extensions
{
    public static class StringExtensions
    {

        public static int IndexOfNth(this string str, string value, int nth = 1)
        {
            if (nth <= 0)
                throw new ArgumentException("Can not find the zeroth index of substring in string. Must start with 1");
            int offset = str.IndexOf(value);
            for (int i = 1; i < nth; i++)
            {
                if (offset == -1) return -1;
                offset = str.IndexOf(value, offset + 1);
            }
            return offset;
        }

        public static string ToMd5(this string stringToHash)
        {
            var md5 = MD5.Create();
            // Convert the input string to a byte array and compute the hash.
            byte[] data = md5.ComputeHash(Encoding.UTF8.GetBytes(stringToHash));

            // Create a new Stringbuilder to collect the bytes
            // and create a string.
            StringBuilder sBuilder = new StringBuilder();

            // Loop through each byte of the hashed data 
            // and format each one as a hexadecimal string.
            for (int i = 0; i < data.Length; i++)
            {
                sBuilder.Append(data[i].ToString("x2"));
            }

            // Return the hexadecimal string.
            return sBuilder.ToString();
        }

        // Verify a hash against a string.
        public static bool VerifyMd5Hash(this string input, string hash)
        {
            var md5 = MD5.Create();
            // Hash the input.
            string hashOfInput = input.ToMd5();

            // Create a StringComparer an compare the hashes.
            StringComparer comparer = StringComparer.OrdinalIgnoreCase;

            if (0 == comparer.Compare(hashOfInput, hash))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        public static string DecodeUrl(this string url)
        {
            return url;//Replace("-", " ");
        }
    }
}
