using System;
using Kachuwa.Data.Crud;

namespace Kachuwa.Installer
{
    public class InstallerViewModel
    {
        public string DatabaseServer { get; set; }
        public string DatabaseName { get; set; }
        public string DatabaseUser { get; set; }
        public string DatabasePassword { get; set; }

        public string DatabaseProvider { get; set; } = "SQLServer";
        public string ConnectionStrings { get; set; }
        public int Port { get; set; }
        public string SiteName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }

        public override string ToString()
        {
            //Server=124.41.193.135;Port=3306;Database=k4;User Id=root;Password=thisisme@sushil;persistsecurityinfo=True;SslMode=none;"
            //Server=124.41.193.135;Database=tixalaya;Persist Security Info=False;User ID=sa;Password=admin12345;;MultipleActiveResultSets=true;Connection Timeout=30;
            //Server=127.0.0.1;Port=5432;Database=kachuwaps;User Id=postgres;Password=binod;CommandTimeout=30;
            string msSqlConnectionString = $"Server={this.DatabaseServer};Database={this.DatabaseName};Persist Security Info=False;User ID={this.DatabaseUser};Password={this.DatabasePassword};;MultipleActiveResultSets=true;Connection Timeout=30";
            string mySqlConnectionString = $"Server={this.DatabaseServer};Port={this.Port};Database={this.DatabaseName};User Id={this.DatabaseUser};Password={this.DatabasePassword};persistsecurityinfo=True;SslMode=none;";
            string npgSqlConnectionString = $"Server={this.DatabaseServer};Port={this.Port};Database={this.DatabaseName};User Id={this.DatabaseUser};Password={this.DatabasePassword};CommandTimeout=30";

            if (!string.IsNullOrEmpty(ConnectionStrings))
            {
                return this.ConnectionStrings;
            }
            else
            {
                var dialect = (Dialect)Enum.Parse(typeof(Dialect), this.DatabaseProvider);
                switch (dialect)
                {
                    case Dialect.MySQL:
                        return mySqlConnectionString;
                    case Dialect.SQLServer:
                        return msSqlConnectionString;
                    case Dialect.PostgreSQL:
                        return npgSqlConnectionString;
                    case Dialect.SQLite:
                        return "";
                }
                return "";
            }

        }


    }
}