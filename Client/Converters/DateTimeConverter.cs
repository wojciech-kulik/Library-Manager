using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;
using System.Globalization;

namespace ClientApplication.Converters
{
    class DateTimeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
                return "(brak)";

            bool with_time = false;
            if (parameter != null) 
                with_time = Boolean.Parse((string)parameter);

            return ((DateTime)value).ToShortDateString() + (with_time ? " " + ((DateTime)value).ToShortTimeString() : "");
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException("DateTimeConverter not implemented yet.");
        }
    }
}
