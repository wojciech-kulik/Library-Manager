using Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Services;
using System.Configuration;

namespace ClientApplication.Utilities
{
    public class DBServiceManager : IDBServiceManager<IDatabaseService>
    {
        private readonly ISettingsService _settingsService;

        public DBServiceManager(ISettingsService settingsService)
        {
            _settingsService = settingsService;
        }

        public IDatabaseService GetService()
        {
            var connectionString = App.Config.ConnectionStrings.ConnectionStrings["LibraryDataContext"].ConnectionString;
            return new DatabaseService(connectionString, _settingsService.Username);
        }
    }
}
