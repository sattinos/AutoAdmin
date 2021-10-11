using System;
using System.IO;
using System.Threading;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Dapper;
using MySql.Data.MySqlClient;
using AutoAdmin.Config;
using AutoAdmin.Injection.Attributes;
using AutoAdmin.Services.Dapper;

namespace AutoAdmin.Services
{
    [InjectAs(ServiceLifetime.Singleton)]
    public class DbContext
    {
        public DbConfigurations DbConfigurations { get; }

        public DbContext(IConfiguration configuration)
        {
            DbConfigurations = configuration.GetSection("Db").Get<DbConfigurations>();
            EstablishConnection(DbConfigurations.ConnectionString);
            GuidMapper.Setup();
        }

        private void EstablishConnection(string connectionString)
        {
            Connection = new MySqlConnection(connectionString);
        }

        public MySqlConnection Connection { get; private set; }

        private static void SetupDb(DbConfigurations dbConfigurations)
        {
            if (dbConfigurations.Setup)
            {
                Console.WriteLine("==============");
                Console.WriteLine("setup database");
                Console.WriteLine("==============");
                var connection = new MySqlConnection(dbConfigurations.ConnectionString);
                var assemblyFolder =
                    Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
                var paths = Path.Combine(assemblyFolder, dbConfigurations.ScriptsFolder);
                var scriptsFullPath = Path.GetFullPath(paths);
                if (Directory.Exists(scriptsFullPath))
                {
                    var scripts = Directory.GetFiles(scriptsFullPath, "*.sql");
                    if (scripts.Length == 0)
                    {
                        Console.WriteLine("no scripts detected.");
                    }

                    Array.Sort(scripts);
                    foreach (var scriptFileName in scripts)
                    {
                        Console.WriteLine($"executing {scriptFileName}");
                        var sql = File.ReadAllText(scriptFileName);
                        connection.Execute(sql);
                    }
                }
                else
                {
                    Console.WriteLine($"Directory {scriptsFullPath} doesn't exist");
                }
                Console.WriteLine("==============");
            }
            else
            {
                Console.WriteLine("Setup DB is turned off");
            }
        }

        public static void SetupDbUntilSucceed(IConfiguration configuration)
        {
            var dbConfigurations = configuration.GetSection("Db").Get<DbConfigurations>();
            var connected = false;
            while (!connected)
            {
                try
                {
                    SetupDb(dbConfigurations);
                    connected = true;
                }
                catch (Exception e)
                {
                    Console.Error.WriteLine($"an exception while setting up the database: {e.Message}");
                    Thread.Sleep(dbConfigurations.Interval * 1000);
                    Console.WriteLine($"re-trying after sleep: {dbConfigurations.Interval} seconds");
                }
            }
        }
    }
}