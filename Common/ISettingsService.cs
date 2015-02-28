using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common
{
    public interface ISettingsService
    {
        string Username { get; set; }

        string Password { get; set; }
    }
}
