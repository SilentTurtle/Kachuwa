using System;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace Kachuwa.Localization
{
    public class DbResourceProvider : BaseResourceProvider
    {
        // Database connection string        
        private static string connectionString = null;

        public DbResourceProvider()
        {

            //connectionString = ConfigurationManager.ConnectionStrings["MvcInternationalization"].ConnectionString;
        }

        public DbResourceProvider(string connection)
        {
            connectionString = connection;
        }

        protected override IList<Resource> ReadResources()
        {
            var resources = new List<Resource>();

            const string sql = "select Culture, Name, Value from dbo.Resources;";

            using (var con = new SqlConnection(connectionString))
            {
                var cmd = new SqlCommand(sql, con);

                con.Open();

                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        resources.Add(new Resource
                        {
                            Name = reader["Name"].ToString(),
                            Value = reader["Value"].ToString(),
                            Culture = reader["Culture"].ToString()
                        });
                    }

                    if (!reader.HasRows) throw new Exception("No resources were found");
                }
            }

            return resources;

        }

        protected override Resource ReadResource(string name, string culture)
        {
            Resource resource = null;

            const string sql = "select Culture, Name, Value from dbo.Resources where culture = @culture and name = @name;";

            using (var con = new SqlConnection(connectionString))
            {
                var cmd = new SqlCommand(sql, con);

                cmd.Parameters.AddWithValue("@culture", culture);
                cmd.Parameters.AddWithValue("@name", name);

                con.Open();

                using (var reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        resource = new Resource
                        {
                            Name = reader["Name"].ToString(),
                            Value = reader["Value"].ToString(),
                            Culture = reader["Culture"].ToString()
                        };
                    }

                    if (!reader.HasRows) throw new Exception(string.Format("Resource {0} for culture {1} was not found", name, culture));
                }
            }

            return resource;

        }




    }
}