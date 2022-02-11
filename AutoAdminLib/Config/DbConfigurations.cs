using System;
using System.Collections.Generic;
using AutoAdminLib.Injection.Attributes;

namespace AutoAdminLib.Config
{
    [ConfigurationSection(KeyName)]
    public class DbConfigurations
    {
        /// <summary>
        /// Stands for the major key of inside the settings file
        /// </summary>
        public const string KeyName = "Db";
        private readonly Dictionary<string, Action<DbConfigurations, string>> _configurationKeysMap = new()
        {
            ["server"] = (dbConf, value) => dbConf.Server = value,
            ["port"] = (dbConf, value) => dbConf.Port = value,
            ["database"] = (dbConf, value) => dbConf.DbName = value,
            ["user"] = (dbConf, value) => dbConf.User = value,
            ["password"] = (dbConf, value) => dbConf.Password = value
        };

        public string ConnectionString { get; set; }
        public bool Setup { get; set; }
        public string ScriptsFolder { get; set; }
        public string DbName { get; set; }
        public string Server { get; set; }
        public string Port { get; set; }
        public string User { get; set; }
        public string Password { get; set; }
        public int Interval { get; set; }
        public void Process()
        {
            if (!string.IsNullOrWhiteSpace(ConnectionString))
            {
                var tokens = ConnectionString.Split(";");
                foreach (var token in tokens)
                {
                    var keyvalue = token.Trim().Split("=");
                    if (_configurationKeysMap.ContainsKey(keyvalue[0]))
                    {
                        _configurationKeysMap[keyvalue[0]](this, keyvalue[1]);
                    }
                }
            }
        }
    }
}