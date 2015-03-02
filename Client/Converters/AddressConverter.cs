using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;
using System.Globalization;
using Common;
using Model;

namespace ClientApplication.Converters
{
    class AddressConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is Address)
            {
                Address addr = (Address)value;

                if (String.IsNullOrEmpty(addr.Street) && String.IsNullOrEmpty(addr.City))
                    return App.GetString("NotAvailable");

                string nr = addr.HouseNumber + (String.IsNullOrEmpty(addr.ApartmentNumber) ? "" : " " + App.GetString("Apt") + " " + addr.ApartmentNumber);
                return (addr.Street + " " + nr).Trim() + "\n" + (addr.PostalCode + " " + addr.City).Trim();
            } 
            else
                return "";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
