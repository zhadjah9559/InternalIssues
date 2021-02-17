using Microsoft.Extensions.Configuration;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace InternalIssues.Utilities
{
    public static class DataUtility
    {
        public static string GetConnectionString(IConfiguration configuration)
        {
            //default connection string will come from appSettings like usual
            var connectionString = configuration.GetConnectionString("DefaultConnection");

            //it will automatically overwritten if we are running on heroku
            var databaseUrl = Environment.GetEnvironmentVariable("DATABASE_URL");
            return string.IsNullOrEmpty(databaseUrl) ? connectionString : BuildConnectionString(databaseUrl);
        }

        public static string BuildConnectionString(string databaseUrl)
        {
            //Provides an object representation of a uniform resource identifier (URI) and easy access to the parts of the URI.
            var databaseUri = new Uri(databaseUrl);
            var userInfo = databaseUri.UserInfo.Split(':');

            //Provides a simple way to create and maage the contents of connection strings used by the NpgsqlConnection class
            var builder = new NpgsqlConnectionStringBuilder
            {
                Host = databaseUri.Host,
                Port = databaseUri.Port,
                Username = userInfo[0],
                Password = userInfo[1],
                Database = databaseUri.LocalPath.TrimStart('/'),
                SslMode = SslMode.Prefer,
                TrustServerCertificate = true
            };

            return builder.ToString();
        }


    }
}
