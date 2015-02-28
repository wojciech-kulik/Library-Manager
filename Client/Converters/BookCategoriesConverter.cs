using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;
using System.Globalization;
using ClientApplication.DBService;

namespace ClientApplication.Converters
{
    class BookCategoriesConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string result = "";
            
            foreach (var cat in (IEnumerable<BookCategoryDTO>)value)
            {
                if (result != "")
                    result += ", ";
                result += cat.Name;
            }

            return result;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
