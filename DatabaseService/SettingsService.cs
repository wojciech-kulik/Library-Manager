using Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services
{
    public class SettingsService : ISettingsService
    {
        static string Username { get; set; }

        string ISettingsService.Username
        {
            get
            {
                return SettingsService.Username;
            }
            set
            {
                SettingsService.Username = value;
            }
        }
    }
}
