using System;
using System.Linq;
using Caliburn.Micro;
using System.Windows;
using System.Threading.Tasks;
using System.Windows.Input;
using AutoMapper;
using Common;
using System.Collections.ObjectModel;
using Model;
using Helpers;
using System.Collections.Generic;
using Model.Utils;

namespace ClientApplication.ViewModels
{
    public class MainWindowViewModel : BaseViewModel
    {
        ISettingsService _settingsService;

        public MainWindowViewModel(INavigationService navigationService, IWindowManager windowManager, IDBServiceManager<IDatabaseService> dbServiceManager, ISettingsService settingsService)
            : base(navigationService, windowManager, dbServiceManager)
        { 
            //RefreshClients();
            _settingsService = settingsService;
        }

        #region bindable properties

        #region AllClients

        private BindableCollection<Client> _allClients;

        public BindableCollection<Client> AllClients
        {
            get
            {
                return _allClients;
            }
            set
            {
                if (_allClients != value)
                {
                    _allClients = value;
                    NotifyOfPropertyChange(() => AllClients);
                }
            }
        }

        #endregion

        #region Clients

        private BindableCollection<Client> _clients;

        public BindableCollection<Client> Clients
        {
            get
            {
                return _clients;
            }
            set
            {
                if (_clients != value)
                {
                    _clients = value;
                    NotifyOfPropertyChange(() => Clients);
                }
            }
        }

        #endregion

        #region SelectedClient

        private Client _selectedClient;

        public Client SelectedClient
        {
            get
            {
                return _selectedClient;
            }
            set
            {
                if (_selectedClient != value)
                {
                    _selectedClient = value;
                    if (value != null)
                    {
                        LoadLendings();
                    }
                    NotifyOfPropertyChange(() => SelectedClient);
                }
            }
        }
        #endregion

        #region SelectedLending

        private Lending _selectedLending;

        public Lending SelectedLending
        {
            get
            {
                return _selectedLending;
            }
            set
            {
                if (_selectedLending != value)
                {
                    _selectedLending = value;
                    NotifyOfPropertyChange(() => SelectedLending);
                }
            }
        }
        #endregion

        #endregion

        #region operations

        //TODO: add locks, handle exceptions
        #region Client operations

        private async Task LoadLendings()
        {
            var selectedClient = SelectedClient;

            await Task.Run(() =>
            {
                if (SelectedClient == null || selectedClient != SelectedClient)
                    return;

                using (var dbService = _dbServiceManager.GetService())
                {
                    SelectedClient.Lendings = dbService.GetLendingsOf(SelectedClient.Id);
                }
            });
        }

        private void ReplaceClient(IList<Client> clients, int clientId, Client newClient)
        {
            for (int i = 0; i < clients.Count; i++)
            {
                if (clients[i].Id == clientId)
                {
                    clients[i] = newClient;
                    break;
                }
            }
        }

        public async void RefreshSelectedClient()
        {
            var selectedClient = SelectedClient;

            await Task.Run(async () =>
            {
                if (SelectedClient == null || selectedClient != SelectedClient)
                    return;

                int id = SelectedClient.Id;
                int lendingId = SelectedLending != null ? SelectedLending.Id : -1;
                SelectedClient = null;

                using (var dbService = _dbServiceManager.GetService())
                {
                    Client newClient = dbService.Clients.GetClient(id);
                    ReplaceClient(AllClients, id, newClient);
                    ReplaceClient(Clients, id, newClient);

                    _selectedClient = newClient;
                    NotifyOfPropertyChange(() => SelectedClient);
                    await LoadLendings();

                    SelectedLending = SelectedClient.Lendings.FirstOrDefault(l => l.Id == lendingId);
                }
            });
        }

        public void RefreshClients()
        {
            Task.Run(() =>
            {
                int id = SelectedClient != null ? SelectedClient.Id : -1;
                SelectedClient = null;
                SelectedLending = null;
                AllClients = Clients = null;

                using (var dbService = _dbServiceManager.GetService())
                {
                    if (Role == null)
                        Role = (Role)dbService.GetEmployeeRole(_settingsService.Username);

                    AllClients = new BindableCollection<Client>(dbService.Clients.GetAllClients());
                    Clients = new BindableCollection<Client>(AllClients);
                    SelectedClient = Clients.FirstOrDefault(c => c.Id == id);
                }
            });
        }

        private string _currentSearchClientPhrase;
        public void SearchClient(ActionExecutionContext context, string phrase)
        {
            var keyArgs = context.EventArgs as KeyEventArgs;

            if (keyArgs != null && keyArgs.Key == Key.Enter)
            {
                _currentSearchClientPhrase = phrase;
                Task.Run(() =>
                {
                    if (phrase != _currentSearchClientPhrase)
                        return;

                    if (String.IsNullOrEmpty(phrase))
                    {
                        int id = SelectedClient != null ? SelectedClient.Id : -1;
                        Clients = new BindableCollection<Client>(AllClients);
                        SelectedClient = Clients.FirstOrDefault(c => c.Id == id);
                    }
                    else
                    {
                        Clients = new BindableCollection<Client>(AllClients.Where(c => c.FullName.ContainsAny(phrase)));
                    }
                });
            }
        }

        public void AddClient()
        {
            var result = _navigationService.GetWindow<ClientDetailsViewModel>().ShowWindowModal();

            if (result)
            {
                Task.Run(() =>
                {
                    using (var dbService = _dbServiceManager.GetService())
                    {
                        var clients = dbService.Clients.GetAllClients();
                        var newClients = clients.Except(AllClients, new IdRecordComparator<Client>()).ToList();

                        AllClients.AddRange(newClients);
                        Clients = new BindableCollection<Client>(AllClients);
                        SelectedClient = newClients.FirstOrDefault();
                    }
                });
            }
        }

        public void DeleteClient()
        {
            if (MessageBox.Show(String.Format(App.GetString("AreYouSureRemoveClientAndHistory"), SelectedClient.FullName), App.GetString("Removing"),
                                MessageBoxButton.YesNoCancel, MessageBoxImage.Warning, MessageBoxResult.Cancel) != MessageBoxResult.Yes)
            {
                return;
            }

            var toRemove = SelectedClient;
            Task.Run(() =>
            {
                if (toRemove == null || SelectedClient != toRemove)
                    return;

                using (var dbService = _dbServiceManager.GetService())
                {
                    dbService.Clients.DeleteClient(toRemove.Id);
                    AllClients.Remove(toRemove);
                    Clients.Remove(toRemove);
                }
            });
        }

        public void EditClient()
        {
            _navigationService.GetWindow<ClientDetailsViewModel>()
                .WithParam(vm => vm.IsEditing, true)
                .WithParam(vm => vm.Client, Mapper.Map<Client>(SelectedClient))
                .DoIfSuccess(() => RefreshSelectedClient())
                .ShowWindowModal();
        }

        #endregion

        #region Users operations

        public void ShowUsersList()
        {
            _navigationService.GetWindow<UsersListViewModel>().ShowWindowModal();
        }

        #endregion

        #region Lending operations

        public void LendBooks()
        {
            //todo: termin zwrotu
            _navigationService.GetWindow<LendingDetailsViewModel>()
                .WithParam(vm => vm.Lending, new Lending() { ClientId = SelectedClient.Id, Books = new ObservableCollection<LentBook>(), LendingDate = DateTime.Now, EndDate = DateTime.Now.AddDays(7) })
                .DoIfSuccess(() => RefreshSelectedClient())
                .ShowWindowModal();
        }

        public void EditLending()
        {
            Lending lending = Mapper.Map<Lending>(SelectedLending);
            using (var dbSerivce = _dbServiceManager.GetService())
                lending.Books = new BindableCollection<LentBook>(dbSerivce.GetLentBooksOf(SelectedLending.Id));             

            _navigationService.GetWindow<LendingDetailsViewModel>()
                .WithParam(vm => vm.Lending, lending)
                .WithParam(v => v.IsEditing, true)
                .DoIfSuccess(() => RefreshSelectedClient())
                .ShowWindowModal();
        }

        public async void DeleteLending()
        {
            if (MessageBox.Show(App.GetString("AreYouSureRemoveSelectedLending"), App.GetString("Removing"),
                MessageBoxButton.YesNoCancel, MessageBoxImage.Warning, MessageBoxResult.Cancel) != MessageBoxResult.Yes)
            {
                return;
            }

            using (var dbService = _dbServiceManager.GetService())
                dbService.DeleteLending(SelectedClient.Id, SelectedLending.Id);

            await LoadLendings();
        }

        public void ReturnBooks()
        {
            BindableCollection<LentBook> lentBooks;
            using(var dbSerivce = _dbServiceManager.GetService())
                lentBooks = new BindableCollection<LentBook>(dbSerivce.GetLentBooksOf(SelectedLending.Id));

            _navigationService.GetWindow<BooksReturnViewModel>()
                .WithParam(w => w.LentBooks, lentBooks)
                .DoIfSuccess(() => RefreshSelectedClient())
                .ShowWindowModal();        
        }

        #endregion

        #region Books operations

        public void ShowBooksList()
        {
            _navigationService.GetWindow<BooksListViewModel>().ShowWindowModal();
        }

        public void SearchBook(ActionExecutionContext context, string phrase)
        {
            var keyArgs = context.EventArgs as KeyEventArgs;

            if (keyArgs != null && keyArgs.Key == Key.Return && !String.IsNullOrEmpty(phrase))
            {
                _navigationService.GetWindow<BooksListViewModel>()
                    .DoBeforeShow(vm => vm.SearchBook(context, phrase))
                    .ShowWindowModal();
            }
        }

        #endregion

        #region Role

        private Nullable<Role> _role;

        public Nullable<Role> Role
        {
            get
            {
                return _role;
            }
            set
            {
                if (_role != value)
                {
                    _role = value;
                    NotifyOfPropertyChange(() => Role);
                }
            }
        }
        #endregion

        #endregion

    }
}
