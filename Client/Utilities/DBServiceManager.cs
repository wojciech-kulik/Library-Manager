using Common;
using ClientApplication.DBService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClientApplication.Utilities
{
    public class DBServiceManager : IDBServiceManager<DatabaseServiceClient>
    { 
        private ISettingsService _settingsService;

        public DBServiceManager(ISettingsService settingsService)
        {
            _settingsService = settingsService;
        }

        public DatabaseServiceClient GetService()
        {
            var service = new DatabaseServiceClient();
            service.ClientCredentials.UserName.UserName = _settingsService.Username;
            service.ClientCredentials.UserName.Password = _settingsService.Password;

            return service;
        }
    }
}
