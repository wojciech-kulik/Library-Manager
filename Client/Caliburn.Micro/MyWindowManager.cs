using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Caliburn.Micro
{
    public class MyWindowManager : WindowManager
    {
        protected override Window CreateWindow(object rootModel, bool isDialog, object context, IDictionary<string, object> settings)
        {
            if (settings == null)
                settings = new Dictionary<string, object>();
            settings.Add("Title", "Biblioteka 1.0 [beta]");
            return base.CreateWindow(rootModel, isDialog, context, settings);
        }
    }
}
