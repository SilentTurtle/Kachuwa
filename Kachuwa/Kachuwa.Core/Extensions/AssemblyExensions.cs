using System;
using System.IO;
using System.Reflection;

namespace Kachuwa.Extensions
{
    public static class AssemblyExensions
    {
        public static string GetDbInstallScript(this Assembly assembly)
        {
            try
            {
                string dbprovider = "mssql";
                string assemblyNamespace = assembly.GetName().Name;
                var resourceName = $"{assemblyNamespace}.db.{dbprovider}.install.sql";

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
                throw e;
            }
        }
        public static string GetDbUnInstallScript(this Assembly assembly)
        {
            try
            {
                string dbprovider = "mssql";
                string assemblyNamespace = assembly.GetName().Name;
                var resourceName = $"{assemblyNamespace}.db.{dbprovider}.uninstall.sql";

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
                throw e;
            }
        }
        public static byte[] GetResourceFile(this Assembly assembly, string filepath)
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
                throw e;
            }
        }
    }
}