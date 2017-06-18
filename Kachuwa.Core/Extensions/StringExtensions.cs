using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;

namespace Kachuwa.Extensions
{
    public static class StringExtensions
    {

        public static string Truncate(this string text, int length, string ellipsis, bool keepFullWordAtEnd)
        {
            if (string.IsNullOrEmpty(text))
                return string.Empty;

            if (text.Length < length) return text;

            text = text.Substring(0, length);

            if (keepFullWordAtEnd)
            {
                text = text.Substring(0, text.LastIndexOf(' '));
            }

            return text + ellipsis;
        }
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

    public static class AssemblyExensions
    {
        public static string GetDbInstallScript(this Assembly assembly)
        {
            try
            {
                string assemblyNamespace = assembly.GetName().Name;
                var resourceName = $"{assemblyNamespace}.db.install.sql";

                using (Stream stream = assembly.GetManifestResourceStream(resourceName))
                {
                    if (stream != null)
                    {
                        using (StreamReader reader = new StreamReader(stream))
                        {
                            return reader.ReadToEnd();
                        }
                    }
                    return "";
                }
                //var stream= assembly.GetManifestResourceStream("db.install.sql");
                //byte[] buffer = new byte[16 * 1024];
                //using (MemoryStream ms = new MemoryStream())
                //{
                //    int read;
                //    while ((read = stream.Read(buffer, 0, buffer.Length)) > 0)
                //    {
                //        ms.Write(buffer, 0, read);
                //    }
                //     ms.ToArray();
                //}
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }
        public static string GetDbUnInstallScript(this Assembly assembly)
        {
            try
            {
                string assemblyNamespace = assembly.GetName().Name;
                var resourceName = $"{assemblyNamespace}.db.install.sql";

                using (Stream stream = assembly.GetManifestResourceStream(resourceName))
                {
                    if (stream != null)
                    {
                        using (StreamReader reader = new StreamReader(stream))
                        {
                            return reader.ReadToEnd();
                        }
                    }
                    return "";
                }
               
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }
        public static byte[] GetResourceFile(this Assembly assembly,string filepath)
        {
            try
            {
                string assemblyNamespace = assembly.GetName().Name;
                var resourceName = $"{assemblyNamespace}.{filepath}";

                using (Stream stream = assembly.GetManifestResourceStream(resourceName))
                {
                    byte[] buffer = new byte[16 * 1024];
                    using (MemoryStream ms = new MemoryStream())
                    {
                        int read;
                        while ((read = stream.Read(buffer, 0, buffer.Length)) > 0)
                        {
                            ms.Write(buffer, 0, read);
                        }
                        return ms.ToArray();
                    }
                }

            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }
    }
}
