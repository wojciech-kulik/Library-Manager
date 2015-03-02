using Caliburn.Micro;
using ClientApplication.ConfigurationModels;
using ClientApplication.Views;
using Common;
using System;
using System.Configuration;
using System.Windows;

namespace ClientApplication.ViewModels
{
    public class LoginViewModel : BaseViewModel
    {
        private const string ProtectionProvider = "DataProtectionConfigurationProvider";
        private const string DatabaseSettingsNode = "Database";
        private const string ConnectionStringNode = "LibraryDataContext";
        private ISettingsService _settingsService;

        public LoginViewModel(INavigationService navigationService, IWindowManager windowManager, IDBServiceManager<IDatabaseService> dbServiceManager, ISettingsService settingsService)
            : base(navigationService, windowManager, dbServiceManager)
        {
            _settingsService = settingsService;
            LoadDBSettings();
        }

        #region bindable properties

        #region Username

        private string _username;

        public string Username
        {
            get
            {
                return _username;
            }
            set
            {
                if (_username != value)
                {
                    _username = value;
                    NotifyOfPropertyChange(() => Username);
                }
            }
        }
        #endregion

        #region Password

        private string _password;

        public string Password
        {
            get
            {
                return _password;
            }
            set
            {
                if (_password != value)
                {
                    _password = value;
                    NotifyOfPropertyChange(() => Password);
                }
            }
        }
        #endregion

        #region DBUsername

        private string _dbusername;

        public string DBUsername
        {
            get
            {
                return _dbusername;
            }
            set
            {
                if (_dbusername != value)
                {
                    _dbusername = value;
                    NotifyOfPropertyChange(() => DBUsername);
                }
            }
        }
        #endregion

        #region DBPassword

        private string _dbpassword;

        public string DBPassword
        {
            get
            {
                return _dbpassword;
            }
            set
            {
                if (_dbpassword != value)
                {
                    _dbpassword = value;
                    NotifyOfPropertyChange(() => DBPassword);
                }
            }
        }
        #endregion

        #region ServerAddress

        private string _serverAddress;

        public string ServerAddress
        {
            get
            {
                return _serverAddress;
            }
            set
            {
                if (_serverAddress != value)
                {
                    _serverAddress = value;
                    NotifyOfPropertyChange(() => ServerAddress);
                }
            }
        }
        #endregion

        #region DatabaseName

        private string _databaseName;

        public string DatabaseName
        {
            get
            {
                return _databaseName;
            }
            set
            {
                if (_databaseName != value)
                {
                    _databaseName = value;
                    NotifyOfPropertyChange(() => DatabaseName);
                }
            }
        }
        #endregion

        #region UseWindowsAuthentication

        private bool _useWindowsAuthentication;

        public bool UseWindowsAuthentication
        {
            get
            {
                return _useWindowsAuthentication;
            }
            set
            {
                if (_useWindowsAuthentication != value)
                {
                    _useWindowsAuthentication = value;
                    NotifyOfPropertyChange(() => UseWindowsAuthentication);
                }
            }
        }
        #endregion

        #region RememberPassword

        private bool _rememberPassword;

        public bool RememberPassword
        {
            get
            {
                return _rememberPassword;
            }
            set
            {
                if (_rememberPassword != value)
                {
                    _rememberPassword = value;
                    NotifyOfPropertyChange(() => RememberPassword);
                }
            }
        }
        #endregion

        #region DatabaseSettingsVisible

        private bool _databaseSettingsVisible;

        public bool DatabaseSettingsVisible
        {
            get
            {
                return _databaseSettingsVisible;
            }
            set
            {
                if (_databaseSettingsVisible != value)
                {
                    _databaseSettingsVisible = value;
                    NotifyOfPropertyChange(() => DatabaseSettingsVisible);
                }
            }
        }
        #endregion

        #endregion

        #region Lifecycle
        protected override void OnViewLoaded(object view)
        {
            LoadDBSettings();
            if (RememberPassword)
            {
                (view as LoginView).pbDBPassword.Password = DBPassword;
            }
        }
        #endregion

        #region operations

        private void AddConnectionString()
        {
            var section = App.Config.ConnectionStrings;
            var connectionString = String.Format(
                    "metadata=res://*/LibraryManager.csdl|res://*/LibraryManager.ssdl|res://*/LibraryManager.msl;provider=System.Data.SqlClient;provider connection string=\"" +
                    "data source={0};initial catalog={1};{2};MultipleActiveResultSets=True;App=EntityFramework\"",
                    ServerAddress, 
                    DatabaseName,
                    UseWindowsAuthentication ? "integrated security=True" : String.Format("User Id={0};Password={1}", DBUsername, DBPassword));

            if (section.ConnectionStrings[ConnectionStringNode] != null)
            {
                section.ConnectionStrings.Remove(ConnectionStringNode);
            }
            section.ConnectionStrings.Add(new ConnectionStringSettings(ConnectionStringNode, connectionString, "System.Data.EntityClient"));
            if (!section.SectionInformation.IsProtected)
                section.SectionInformation.ProtectSection(ProtectionProvider);

            App.Config.Save();
        }

        private void SaveDBSettings()
        {
            var section = new DatabaseSettingsSection();            
            section.DatabaseSettings.DatabaseName = DatabaseName;
            section.DatabaseSettings.ServerAddress = ServerAddress;
            section.DatabaseSettings.Username = DBUsername;
            section.DatabaseSettings.Password = RememberPassword ? DBPassword : null;
            section.DatabaseSettings.RememberPassword = RememberPassword;
            section.DatabaseSettings.UseWindowsAuthentication = UseWindowsAuthentication;

            if (App.Config.GetSection(DatabaseSettingsNode) != null)
            {
                App.Config.Sections.Remove(DatabaseSettingsNode);
            }
            App.Config.Sections.Add(DatabaseSettingsNode, section);
            if (!section.SectionInformation.IsProtected)
                section.SectionInformation.ProtectSection(ProtectionProvider);

            App.Config.Save();
        }

        private void LoadDBSettings()
        {
            var section = App.Config.GetSection(DatabaseSettingsNode) as DatabaseSettingsSection;
            if (section == null)
            {
                DatabaseSettingsVisible = true;
                return;
            }

            DatabaseName = section.DatabaseSettings.DatabaseName;
            ServerAddress = section.DatabaseSettings.ServerAddress;
            DBUsername = section.DatabaseSettings.Username;
            DBPassword = section.DatabaseSettings.Password;
            RememberPassword = section.DatabaseSettings.RememberPassword;
            UseWindowsAuthentication = section.DatabaseSettings.UseWindowsAuthentication;
        }

        private bool TestConnection()
        {
            //TODO: implement it
            return true;
        }

        private bool ValidateDatabaseSettings()
        {
            if (String.IsNullOrWhiteSpace(ServerAddress) || String.IsNullOrWhiteSpace(DatabaseName) || (!UseWindowsAuthentication && String.IsNullOrWhiteSpace(DBUsername)))
            {
                MessageBox.Show(App.GetString("FillDatabaseSettings"), App.GetString("FillRequiredFields"), MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            return TestConnection();
        }

        private bool AuthenticateUser()
        {
            var dbService = _dbServiceManager.GetService();
            try
            {
                dbService.TestAuthorization(Username, Password);
                return true;
            }
            catch
            {
                MessageBox.Show(App.GetString("UserNotFoundPasswordWrong"), App.GetString("Error"), MessageBoxButton.OK, MessageBoxImage.Warning);
            }

            return false;
        }

        public void Login()
        {
            if (!ValidateDatabaseSettings())
            {
                return;
            }
            AddConnectionString();
            SaveDBSettings();

            if (String.IsNullOrWhiteSpace(Username) || String.IsNullOrWhiteSpace(Password))
            {
                MessageBox.Show(App.GetString("FillUsernameAndPassword"), App.GetString("FillRequiredFields"), MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            _settingsService.Username = Username;
           
            if (AuthenticateUser())
            {
                TryClose(true);
            }
        }

        #endregion
    }
}
