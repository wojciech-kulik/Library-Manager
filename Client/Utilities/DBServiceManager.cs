using Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DBService;

namespace ClientApplication.Utilities
{
    public class DBServiceManager : IDBServiceManager<IDatabaseService>
    { 
        private ISettingsService _settingsService;

        public DBServiceManager(ISettingsService settingsService)
        {
            _settingsService = settingsService;
        }

        public IDatabaseService GetService()
        {
            var service = new DatabaseService();

            return service;
        }
    }
}
