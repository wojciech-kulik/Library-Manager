using Caliburn.Micro;
using Common;
using Model;
using System;
using System.Linq;
using System.Windows;

namespace ClientApplication.ViewModels
{
    public class UserDetailsViewModel : BaseViewModel        
    {
        ISettingsService _settingsService;

        public UserDetailsViewModel(INavigationService navigationService, IWindowManager windowManager, IDBServiceManager<IDatabaseService> dbServiceManager, ISettingsService settingsService)
            : base(navigationService, windowManager, dbServiceManager)
        {
            User = new Employee();
            _settingsService = settingsService;
        }

        #region navigation properties

        public bool IsEditing { get; set; }

        #endregion

        #region bindable properties

        #region User

        private Employee _user;

        public Employee User
        {
            get
            {
                return _user;
            }
            set
            {
                if (_user != value)
                {
                    _user = value;
                    NotifyOfPropertyChange(() => User);
                }
            }
        }
        #endregion

        #endregion

        #region operation

        private void UpdateUser()
        {
            using (var dbService = _dbServiceManager.GetService())
            {
                if (dbService.GetAllEmployees().Any(emp => !emp.Removed && emp.Id != User.Id && emp.Username == User.Username.ToLower()))
                {
                    MessageBox.Show(App.GetString("UsernameIsTaken"), App.GetString("UsernameIsTakenCaption"), MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                var oldUser = dbService.GetEmployee(User.Id);
                if (_settingsService.Username == oldUser.Username && (Role)User.Role != Role.Admin)
                {
                    MessageBox.Show(App.GetString("CantRemoveAdminYourself"), App.GetString("Error"), MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }
                dbService.EditEmployee(User);
                if (_settingsService.Username == oldUser.Username)
                {
                    _settingsService.Username = User.Username;
                }
            }
            TryClose(true);
        }

        private void AddUser()
        {
            using (var dbService = _dbServiceManager.GetService())
            {
                if (dbService.GetAllEmployees().Any(emp => !emp.Removed && emp.Username == User.Username.ToLower()))
                {
                    MessageBox.Show(App.GetString("UsernameIsTaken"), App.GetString("UsernameIsTakenCaption"), MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }
                dbService.AddEmployee(User);
            }
            TryClose(true);
        }

        public void Save()
        {
            if (String.IsNullOrWhiteSpace(User.FirstName) || String.IsNullOrWhiteSpace(User.LastName))
            {
                MessageBox.Show(App.GetString("FirstNameLastNameRequired"), App.GetString("FillRequiredFields"), MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            if (String.IsNullOrWhiteSpace(User.Username))
            {
                MessageBox.Show(App.GetString("UsernameIsRequired"), App.GetString("FillRequiredFields"), MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            if (!IsEditing && String.IsNullOrWhiteSpace(User.Password))
            {
                MessageBox.Show(App.GetString("PasswordIsRequired"), App.GetString("FillRequiredFields"), MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            if ((String.IsNullOrWhiteSpace(User.Address.City) || String.IsNullOrWhiteSpace(User.Address.Street)) &&
                (!String.IsNullOrWhiteSpace(User.Address.City) || !String.IsNullOrWhiteSpace(User.Address.Street) ||
                !String.IsNullOrWhiteSpace(User.Address.HouseNumber) || !String.IsNullOrWhiteSpace(User.Address.ApartmentNumber) ||
                !String.IsNullOrWhiteSpace(User.Address.PostalCode)))
            {
                MessageBox.Show(App.GetString("FillStreetAndCity"), App.GetString("Error"), MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            //to be sure, that we won't sent unnecessary data
            User.Lendings = null;
            User.Returns = null;

            if (IsEditing)
                UpdateUser();
            else
                AddUser();            
        }

        #endregion
    }
}
