using System;
using System.Windows.Data;
using System.Globalization;
using System.Windows;
using Model;

namespace ClientApplication.Converters
{
    class RoleToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (byte)value == 1 ? App.GetString("Administrator") : App.GetString("Employee");
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
