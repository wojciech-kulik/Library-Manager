using Caliburn.Micro;
using ClientApplication.DBService;
using Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace ClientApplication.ViewModels
{
    public class UserDetailsViewModel : BaseViewModel        
    {
        ISettingsService _settingsService;

        public UserDetailsViewModel(INavigationService navigationService, IWindowManager windowManager, IDBServiceManager<DatabaseServiceClient> dbServiceManager, ISettingsService settingsService)
            : base(navigationService, windowManager, dbServiceManager)
        {
            User = new EmployeeDTO();
            _settingsService = settingsService;
        }

        #region navigation properties

        public bool IsEditing { get; set; }

        #endregion

        #region bindable properties

        #region User

        private EmployeeDTO _user;

        public EmployeeDTO User
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
                    MessageBox.Show("Użytkownik o podanej nazwie istnieje już w systemie. Wybierz inną nazwę użytkownika.", "Nazwa zajęta", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                var oldUser = dbService.GetEmployee(User.Id);
                if (_settingsService.Username == oldUser.Username && (Role)User.Role != Role.Admin)
                {
                    MessageBox.Show("Nie możesz sam sobie odebrać uprawnień administracyjnych.", "Błąd", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }
                dbService.EditEmployee(User);
                if (_settingsService.Username == oldUser.Username)
                {
                    _settingsService.Username = User.Username;
                    if (!String.IsNullOrWhiteSpace(User.Password))
                        _settingsService.Password = User.Password;
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
                    MessageBox.Show("Użytkownik o podanej nazwie istnieje już w systemie. Wybierz inną nazwę użytkownika.", "Nazwa zajęta", MessageBoxButton.OK, MessageBoxImage.Warning);
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
                MessageBox.Show("Imię i nazwisko jest wymagane.", "Brak danych", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            if (String.IsNullOrWhiteSpace(User.Username))
            {
                MessageBox.Show("Nazwa użytkownika jest wymagana.", "Brak danych", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            if (!IsEditing && String.IsNullOrWhiteSpace(User.Password))
            {
                MessageBox.Show("Podanie hasła jest wymagane w przypadku tworzenia nowego użytkownika.", "Brak danych", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            if ((String.IsNullOrWhiteSpace(User.Address.City) || String.IsNullOrWhiteSpace(User.Address.Street)) &&
                (!String.IsNullOrWhiteSpace(User.Address.City) || !String.IsNullOrWhiteSpace(User.Address.Street) ||
                !String.IsNullOrWhiteSpace(User.Address.HouseNumber) || !String.IsNullOrWhiteSpace(User.Address.ApartmentNumber) ||
                !String.IsNullOrWhiteSpace(User.Address.PostalCode)))
            {
                MessageBox.Show("Jeżeli podajesz adres, to należ uzupełnić minimum ulicę i miasto.", "Błąd", MessageBoxButton.OK, MessageBoxImage.Warning);
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
