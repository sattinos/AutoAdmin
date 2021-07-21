using System;
using System.IO;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Dapper;
using MySql.Data.MySqlClient;
using AutoAdmin.Config;
using AutoAdmin.Injection.Attributes;
using AutoAdmin.Services.Dapper;


namespace AutoAdmin.Services {
    [InjectAs(ServiceLifetime.Singleton)]
    public class DbContext {
        public DbConfigurations DbConfigurations { get; }

        public DbContext(IConfiguration configuration) {
            DbConfigurations = configuration.GetSection("Db").Get<DbConfigurations>();
            EstablishConnection(DbConfigurations.ConnectionString);
            GuidMapper.Setup();
        }
        private void EstablishConnection(string connectionString) {
            Connection = new MySqlConnection(connectionString);
        }
        public MySqlConnection Connection { get; private set; }
        public static void SetupDb(IConfiguration configuration)
        {
            try
            {
                var dbConfigurations = configuration.GetSection("Db").Get<DbConfigurations>();
                if (dbConfigurations.Setup)
                {
                    Console.WriteLine("setup database");
                    Console.WriteLine("==============");
                    dbConfigurations.Process();
                    var connectionStringWithoutDb = $"server={dbConfigurations.Server};port={dbConfigurations.Port};user={dbConfigurations.User};password={dbConfigurations.Password}";
                    var connection = new MySqlConnection(connectionStringWithoutDb);
                    var assemblyFolder = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
                    var paths = Path.Combine(assemblyFolder, dbConfigurations.ScriptsFolder);
                    var scriptsFullPath = Path.GetFullPath(paths);
                    if (Directory.Exists(scriptsFullPath))
                    {
                        var scripts = Directory.GetFiles( scriptsFullPath, "*.sql");
                        Array.Sort(scripts);
                        foreach (var scriptFileName in scripts)
                        {
                            Console.WriteLine($"executing {scriptFileName}");
                            var sql = File.ReadAllText(scriptFileName);
                            connection.Execute(sql);
                        }
                    }
                    Console.WriteLine("==============");
                }
            }
            catch (Exception e)
            {
                Console.WriteLine($"an exception while setting up the database: {e.Message}");
                throw;
            }
        }
    }
}
