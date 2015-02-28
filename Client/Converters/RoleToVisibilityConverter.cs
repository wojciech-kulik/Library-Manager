using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;
using System.Globalization;
using ClientApplication.DBService;
using Common;
using System.Windows;

namespace ClientApplication.Converters
{
    class RoleToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            Nullable<Role> role = value != null ? new Nullable<Role>((Role)value) : null;
            Role required = (Role)Byte.Parse((string)parameter);

            return role == required ? Visibility.Visible : Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
