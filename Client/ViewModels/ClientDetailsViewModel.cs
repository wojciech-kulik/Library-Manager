using Caliburn.Micro;
using Common;
using Model;
using System;
using System.Windows;

namespace ClientApplication.ViewModels
{
    public class ClientDetailsViewModel : BaseViewModel        
    {
        public ClientDetailsViewModel(INavigationService navigationService, IWindowManager windowManager, IDBServiceManager<IDatabaseService> dbServiceManager)
            : base(navigationService, windowManager, dbServiceManager)
        {
            Client = new ClientDTO();
        }



        #region navigation properties

        public bool IsEditing { get; set; }

        #endregion

        #region bindable properties

        #region Client

        private ClientDTO _client;

        public ClientDTO Client
        {
            get
            {
                return _client;
            }
            set
            {
                if (_client != value)
                {
                    _client = value;
                    NotifyOfPropertyChange(() => Client);
                }
            }
        }
        #endregion

        #endregion

        #region operation

        private void UpdateClient()
        {
            using (var dbService = _dbServiceManager.GetService())
            {
                dbService.EditClient(Client);
            }
            TryClose(true);
        }

        private void AddClient()
        {
            using (var dbService = _dbServiceManager.GetService())
            {
                dbService.AddClient(Client);
            }
            TryClose(true);
        }

        public void Save()
        {
            if (String.IsNullOrWhiteSpace(Client.FirstName) || String.IsNullOrWhiteSpace(Client.LastName))
            {
                MessageBox.Show(App.GetString("FirstNameLastNameRequired"), App.GetString("FillRequiredFields"), MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            if ((String.IsNullOrWhiteSpace(Client.Address.City) || String.IsNullOrWhiteSpace(Client.Address.Street)) &&
                (!String.IsNullOrWhiteSpace(Client.Address.City) || !String.IsNullOrWhiteSpace(Client.Address.Street) || 
                !String.IsNullOrWhiteSpace(Client.Address.HouseNumber) || !String.IsNullOrWhiteSpace(Client.Address.ApartmentNumber) || 
                !String.IsNullOrWhiteSpace(Client.Address.PostalCode)))
            {
                MessageBox.Show(App.GetString("FillStreetAndCity"), App.GetString("FillRequiredFields"), MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            //to be sure, that we won't sent unnecessary data
            Client.Lendings = null;

            if (IsEditing)
                UpdateClient();
            else
                AddClient();            
        }

        #endregion
    }
}
