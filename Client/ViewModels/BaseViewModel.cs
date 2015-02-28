using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Caliburn.Micro;
using System.ComponentModel.Composition;
using Common;

namespace ClientApplication.ViewModels
{
    public class BaseViewModel : Screen
    {
        protected IWindowManager _windowManager;
        protected IDBServiceManager<IDatabaseService> _dbServiceManager;
        protected INavigationService _navigationService;

        public BaseViewModel(INavigationService navigationService, IWindowManager windowManager, IDBServiceManager<IDatabaseService> dbServiceManager)
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
