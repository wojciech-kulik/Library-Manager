using AutoMapper;
using Caliburn.Micro;
using Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace ClientApplication.ViewModels
{
    public class UsersListViewModel : BaseViewModel
    {
        ISettingsService _settingsService;

        public UsersListViewModel(INavigationService navigationService, IWindowManager windowManager, IDBServiceManager<IDatabaseService> dbServiceManager, ISettingsService settingsService)
            : base(navigationService, windowManager, dbServiceManager)
        {
            RefreshUsers();
            _settingsService = settingsService;
        }

        #region bindable properties

        #region Users

        private BindableCollection<EmployeeDTO> _users;

        public BindableCollection<EmployeeDTO> Users
        {
            get
            {
                return _users;
            }
            set
            {
                if (_users != value)
                {
                    _users = value;
                    NotifyOfPropertyChange(() => Users);
                }
            }
        }
        #endregion

        #region SelectedUser

        private EmployeeDTO _selectedUser;

        public EmployeeDTO SelectedUser
        {
            get
            {
                return _selectedUser;
            }
            set
            {
                if (_selectedUser != value)
                {
                    _selectedUser = value;
                    NotifyOfPropertyChange(() => SelectedUser);
                }
            }
        }
        #endregion

        #endregion

        #region operations

        public void RefreshUsers()
        {
            using (var dbService = _dbServiceManager.GetService())
            {
                Users = new BindableCollection<EmployeeDTO>(dbService.GetAllEmployees());
            }
        }

        public void RefreshSelectedUser()
        {
            int id = SelectedUser != null ? SelectedUser.Id : -1;
            SelectedUser = null;

            using (var dbService = _dbServiceManager.GetService())
            {
                EmployeeDTO newUser = dbService.GetEmployee(id);

                for (int i = 0; i < Users.Count; i++)
                    if (Users[i].Id == id)
                    {
                        Users[i] = newUser;
                        SelectedUser = Users[i];
                        break;
                    }
            }
        }

        public void AddUser()
        {
            bool result = _navigationService.GetWindow<UserDetailsViewModel>().ShowWindowModal();

            if (result)
            {
                //Adding a new user to list
                EmployeeDTO newUser = null;
                using (var dbService = _dbServiceManager.GetService())
                {
                    var users = dbService.GetAllEmployees();

                    foreach (var u in users.OrderByDescending(cli => cli.Id))
                    {
                        if (Users.Any(user => user.Id == u.Id))
                            break;
                        else
                        {
                            if (newUser == null)
                                newUser = u;
                            Users.Add(u);
                        }
                    }
                }

                SelectedUser = newUser;
            }
        }

        public void EditUser()
        {
            _navigationService.GetWindow<UserDetailsViewModel>()
                .WithParam(vm => vm.IsEditing, true)
                .WithParam(vm => vm.User, Mapper.Map<EmployeeDTO>(SelectedUser))
                .DoIfSuccess(() => RefreshSelectedUser())
                .ShowWindowModal();
        }

        public void DeleteUser()
        {
            if (SelectedUser.Username.ToLower() == _settingsService.Username.ToLower())
            {
                MessageBox.Show(App.GetString("CantRemoveYourself"), App.GetString("Error"), MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            if (MessageBox.Show(String.Format(App.GetString("AreYouSureRemoveUser"), SelectedUser.FullName), App.GetString("Removing"),
                                MessageBoxButton.YesNoCancel, MessageBoxImage.Warning, MessageBoxResult.Cancel) != MessageBoxResult.Yes)
            {
                return;
            }

            using (var dbService = _dbServiceManager.GetService())
            {
                dbService.DeleteEmployee(SelectedUser.Id);
                Users.Remove(SelectedUser);
            }
        }

        #endregion
    }
}
