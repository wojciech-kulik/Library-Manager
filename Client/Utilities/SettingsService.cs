using Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClientApplication.Utilities
{
    public class SettingsService : ISettingsService
    {
        #region saved to file

        #endregion

        #region only in memory

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



        static string Password { get; set; } //hash

        string ISettingsService.Password
        {
            get
            {
                return SettingsService.Password;
            }
            set
            {
                SettingsService.Password = value;
            }
        }

        #endregion
    }
}
