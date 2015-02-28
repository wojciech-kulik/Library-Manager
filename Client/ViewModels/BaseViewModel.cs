using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Caliburn.Micro;
using System.ComponentModel.Composition;
using ClientApplication.DBService;
using Common;

namespace ClientApplication.ViewModels
{
    public class BaseViewModel : Screen
    {
        protected IWindowManager _windowManager;
        protected IDBServiceManager<DatabaseServiceClient> _dbServiceManager;
        protected INavigationService _navigationService;

        public BaseViewModel(INavigationService navigationService, IWindowManager windowManager, IDBServiceManager<DatabaseServiceClient> dbServiceManager)
        {           
            _windowManager = windowManager;
            _dbServiceManager = dbServiceManager;
            _navigationService = navigationService;
        }

        public virtual void Cancel()
        {    
            TryClose();            
        }
    }
}
