using Caliburn.Micro;
using Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace ClientApplication.ViewModels
{
    public class LoginViewModel : BaseViewModel
    {
        private ISettingsService _settingsService;

        public LoginViewModel(INavigationService navigationService, IWindowManager windowManager, IDBServiceManager<IDatabaseService> dbServiceManager, ISettingsService settingsService)
            : base(navigationService, windowManager, dbServiceManager)
        {
            _settingsService = settingsService;
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

        #endregion

        #region operations

        public void Login()
        {
            if (String.IsNullOrWhiteSpace(Username) || String.IsNullOrWhiteSpace(Password))
            {
                MessageBox.Show("Nie podano nazwy użytkownika i/lub hasła.", "Brak danych", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            _settingsService.Username = Username;
            _settingsService.Password = Password;

            var dbService = _dbServiceManager.GetService();              
            try
            {
                dbService.TestAuthorization();
                TryClose(true);
            }
            catch (System.ServiceModel.Security.MessageSecurityException)
            {
                MessageBox.Show("Podany użytkownik nie został znaleziony lub hasło jest nieprawidłowe.", "Nieprawidłowe dane", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        #endregion
    }
}
