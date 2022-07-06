using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Kachuwa.Storage;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;

namespace Kachuwa.Web.Service
{
    public interface IDbBakRestoreService
    {
        Task<bool> Backup();
    }
    public class DbBakRestoreService: IDbBakRestoreService
    {
        private readonly IConfiguration _configuration;
        private readonly IWebHostEnvironment _hostEnvironment;
        private readonly IStorageProvider _storageProvider;


        /* Database names to backup */
         string[] saDatabases = new string[] {  };
         string backupDir = "";

        /* Delete backups older than DeletionDays. Set this to 0 to never delete backups */
        private int DeletionDays = 10;
         Mutex mutex = new Mutex(true, "Global\\SimpleDBBackupMutex");
        public DbBakRestoreService(IConfiguration configuration,IWebHostEnvironment hostEnvironment,IStorageProvider storageProvider)
        {
            _configuration = configuration;
            _hostEnvironment = hostEnvironment;
            _storageProvider = storageProvider;
            var rootpath=_hostEnvironment.ContentRootPath;
            backupDir = Path.Combine(rootpath, "db", "backup");
            var builder = new System.Data.SqlClient.SqlConnectionStringBuilder(configuration.GetConnectionString("DefaultConnection"));
            saDatabases = new[] {builder.InitialCatalog};
        }

        public async Task<bool> Backup()
        {
            // allow only single instance of the app
            if (!mutex.WaitOne(TimeSpan.Zero, true))
            {
                Console.WriteLine("Program already running!");
                return false;
            }

            if (DeletionDays > 0)
                DeleteOldBackups();

            DateTime dtNow = DateTime.Now;

            try
            {
                using (SqlConnection sqlConnection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
                {
                    sqlConnection.Open();

                    foreach (string dbName in saDatabases)
                    {
                        string backupFileNameWithoutExt = String.Format("{0}\\{1}_{2:yyyy-MM-dd_hh-mm-ss-tt}", backupDir, dbName, dtNow);
                        string backupFileNameWithExt = String.Format("{0}.bak", backupFileNameWithoutExt);
                        string zipFileName = String.Format("{0}.zip", backupFileNameWithoutExt);

                        string cmdText = string.Format("BACKUP DATABASE {0}\r\nTO DISK = '{1}'", dbName, backupFileNameWithExt);

                        using (SqlCommand sqlCommand = new SqlCommand(cmdText, sqlConnection))
                        {
                            sqlCommand.CommandTimeout = 0;
                            sqlCommand.ExecuteNonQuery();
                        }
                        ZipFile.CreateFromDirectory(
                            backupFileNameWithExt,
                            zipFileName,
                            compressionLevel: CompressionLevel.Fastest,
                            includeBaseDirectory: false,
                            entryNameEncoding: Encoding.UTF8);

                        File.Delete(backupFileNameWithExt);
                    }

                    sqlConnection.Close();
                    
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                
            }
            mutex.ReleaseMutex();
            return true;
        }
         void DeleteOldBackups()
        {
            try
            {
                string[] files = Directory.GetFiles(backupDir);

                foreach (string file in files)
                {
                    FileInfo fi = new FileInfo(file);
                    if (fi.CreationTime < DateTime.Now.AddDays(-DeletionDays))
                        fi.Delete();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }
    }
}
