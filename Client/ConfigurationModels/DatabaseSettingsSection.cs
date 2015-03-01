using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClientApplication.ConfigurationModels
{
    public class DatabaseSettingsSection : ConfigurationSection
    {
        private const string SectionName = "DatabaseSettings";

        [ConfigurationProperty(SectionName)]
        public DatabaseSettingsElem DatabaseSettings
        { 
            get 
            {
                return (DatabaseSettingsElem)base[SectionName]; 
            } 
        }
    }

    public class DatabaseSettingsElem : ConfigurationElement
    {
        [ConfigurationProperty("databaseName", IsRequired = true)]
        public string DatabaseName
        {
            get { return (string)this["databaseName"]; }
            set { this["databaseName"] = value; }
        }

        [ConfigurationProperty("serverAddress", IsRequired = true)]
        public string ServerAddress
        {
            get { return (string)this["serverAddress"]; }
            set { this["serverAddress"] = value; }
        }

        [ConfigurationProperty("username", IsRequired = true)]
        public string Username
        {
            get { return (string)this["username"]; }
            set { this["username"] = value; }
        }

        [ConfigurationProperty("password", IsRequired = true)]
        public string Password
        {
            get { return (string)this["password"]; }
            set { this["password"] = value; }
        }

        [ConfigurationProperty("useWindowsAuthentication", IsRequired = false, DefaultValue = false)]
        public bool UseWindowsAuthentication
        {
            get { return (bool)this["useWindowsAuthentication"]; }
            set { this["useWindowsAuthentication"] = value; }
        }

        [ConfigurationProperty("rememberPassword", IsRequired = false, DefaultValue = false)]
        public bool RememberPassword
        {
            get { return (bool)this["rememberPassword"]; }
            set { this["rememberPassword"] = value; }
        }
    }
}
