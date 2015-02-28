using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Caliburn.Micro;
using System.ComponentModel.Composition;
using System.Windows;
using System.Threading.Tasks;
using System.Timers;
using System.Windows.Input;
using ClientApplication.DBService;
using System.ComponentModel.Composition.Hosting;
using AutoMapper;
using Common;
using System.Diagnostics;
using System.Collections.ObjectModel;

namespace ClientApplication.ViewModels
{
    public class MainWindowViewModel : BaseViewModel
    {
        ISettingsService _settingsService;

        public MainWindowViewModel(INavigationService navigationService, IWindowManager windowManager, IDBServiceManager<DatabaseServiceClient> dbServiceManager, ISettingsService settingsService)
            : base(navigationService, windowManager, dbServiceManager)
        { 
            //RefreshClients();
            _settingsService = settingsService;
        }

        #region bindable properties

        #region AllClients

        private BindableCollection<ClientDTO> _allClients;

        public BindableCollection<ClientDTO> AllClients
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

        private BindableCollection<ClientDTO> _clients;

        public BindableCollection<ClientDTO> Clients
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

        private ClientDTO _selectedClient;

        public ClientDTO SelectedClient
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
                        Task.Run(() =>
                            {
                                using (var dbService = _dbServiceManager.GetService())
                                {
                                    value.Lendings = dbService.GetLendingsOf(value.Id);
                                }
                            });
                    }
                    NotifyOfPropertyChange(() => SelectedClient);
                }
            }
        }
        #endregion

        #region SelectedLending

        private LendingDTO _selectedLending;

        public LendingDTO SelectedLending
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

        #region Client operations

        public void RefreshSelectedClient()
        {
            int id = SelectedClient != null ? SelectedClient.Id : -1;
            int lendingId = SelectedLending != null ? SelectedLending.Id : -1;
            SelectedClient = null;

            using (var dbService = _dbServiceManager.GetService())
            {
                ClientDTO newClient = dbService.GetClient(id);            

                for(int i = 0; i < AllClients.Count; i++)
                    if (AllClients[i].Id == id)
                    {
                        AllClients[i] = newClient;
                        break;
                    }

                for (int i = 0; i < Clients.Count; i++)
                    if (Clients[i].Id == id)
                    {
                        Clients[i] = newClient;
                        SelectedClient = Clients[i];
                        break;
                    }

                SelectedLending = SelectedClient.Lendings.FirstOrDefault(l => l.Id == lendingId);
            }
        }

        public void RefreshClients()
        {
            Task.Factory.StartNew(() =>
                {
                    int id = SelectedClient != null ? SelectedClient.Id : -1;
                    SelectedClient = null;
                    SelectedLending = null;
                    AllClients = Clients = null;

                    using (var dbService = _dbServiceManager.GetService())
                    {
                        if (Role == null)
                            Role = (Role)dbService.GetEmployeeRole(_settingsService.Username);

                        AllClients = new BindableCollection<ClientDTO>(dbService.GetAllClients());
                        Clients = new BindableCollection<ClientDTO>(AllClients);
                        SelectedClient = Clients.FirstOrDefault(c => c.Id == id);
                    }
                });
        }

        public void SearchClient(ActionExecutionContext context, string phrase)
        {
            var keyArgs = context.EventArgs as KeyEventArgs;

            if (keyArgs != null && keyArgs.Key == Key.Enter)
            {
                if (String.IsNullOrEmpty(phrase))
                {
                    int id = SelectedClient != null ? SelectedClient.Id : -1;
                    Clients = new BindableCollection<ClientDTO>(AllClients);
                    SelectedClient = Clients.FirstOrDefault(c => c.Id == id);
                }
                else
                {
                    Clients = new BindableCollection<ClientDTO>(AllClients.Where(c => c.FullName.ContainsAny(phrase)));
                }
            }
        }

        public void AddClient()
        {
            var result = _navigationService.GetWindow<ClientDetailsViewModel>().ShowWindowModal();

            if (result)
            {
                //Adding a new clients to list
                ClientDTO newClient = null;
                using (var dbService = _dbServiceManager.GetService())
                {                    
                    var clients = dbService.GetAllClients();

                    foreach (var c in clients.OrderByDescending(cli => cli.Id))
                    {
                        if (AllClients.Any(cl => cl.Id == c.Id))
                            break;
                        else
                        {
                            if (newClient == null)
                                newClient = c;
                            AllClients.Add(c);
                        }
                    }
                }

                Clients = new BindableCollection<ClientDTO>(AllClients);
                SelectedClient = newClient;
            }
        }

        public void EditClient()
        {
            _navigationService.GetWindow<ClientDetailsViewModel>()
                .WithParam(vm => vm.IsEditing, true)
                .WithParam(vm => vm.Client, Mapper.Map<ClientDTO>(SelectedClient))
                .DoIfSuccess(() => RefreshSelectedClient())
                .ShowWindowModal();
        }

        public void DeleteClient()
        {
            if (MessageBox.Show("Czy jesteś pewien, że chcesz usunąć z bazy klienta \"" +
                                SelectedClient.FirstName + " " + SelectedClient.LastName +
                                "\" oraz całą jego historię wypożyczeń?", "Usuwanie",
                                MessageBoxButton.YesNoCancel, MessageBoxImage.Warning, MessageBoxResult.Cancel) != MessageBoxResult.Yes)
            {
                return;
            }

            using (var dbService = _dbServiceManager.GetService())
            {
                dbService.DeleteClient(SelectedClient.Id);
                AllClients.Remove(SelectedClient);
                Clients.Remove(SelectedClient);
            }            
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
                .WithParam(vm => vm.Lending, new LendingDTO() { ClientId = SelectedClient.Id, Books = new ObservableCollection<LentBookDTO>(), LendingDate = DateTime.Now, EndDate = DateTime.Now.AddDays(7) })
                .DoIfSuccess(() => RefreshSelectedClient())
                .ShowWindowModal();
        }

        public void EditLending()
        {
            LendingDTO lending = Mapper.Map<LendingDTO>(SelectedLending);
            using (var dbSerivce = _dbServiceManager.GetService())
                lending.Books = new BindableCollection<LentBookDTO>(dbSerivce.GetLentBooksOf(SelectedLending.Id));             

            _navigationService.GetWindow<LendingDetailsViewModel>()
                .WithParam(vm => vm.Lending, lending)
                .WithParam(v => v.IsEditing, true)
                .DoIfSuccess(() => RefreshSelectedClient())
                .ShowWindowModal();
        }

        public void DeleteLending()
        {
            if (MessageBox.Show("Czy jesteś pewien, że chcesz usunąć zaznaczone wypożyczenie?", "Usuwanie",
                MessageBoxButton.YesNoCancel, MessageBoxImage.Warning, MessageBoxResult.Cancel) != MessageBoxResult.Yes)
            {
                return;
            }

            using (var dbService = _dbServiceManager.GetService())
                dbService.DeleteLending(SelectedClient.Id, SelectedLending.Id);

            RefreshSelectedClient();
        }

        public void ReturnBooks()
        {
            BindableCollection<LentBookDTO> lentBooks;
            using(var dbSerivce = _dbServiceManager.GetService())
                lentBooks = new BindableCollection<LentBookDTO>(dbSerivce.GetLentBooksOf(SelectedLending.Id));

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
