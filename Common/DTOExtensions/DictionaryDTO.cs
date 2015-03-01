using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common;

namespace Common
{
    public partial class BookDTO
    {
        private string _bookCategoriesString;
        public string BookCategoriesString
        {
            get
            {
                if (_bookCategoriesString == null)                   
                    _bookCategoriesString = BookCategories.AggregateString(b => b.Name);
                return _bookCategoriesString.NullIfEmpty();
            }
        }

        private string _authorsString;
        public string AuthorsString
        {
            get
            {
                if (_authorsString == null)
                    _authorsString = Authors.AggregateString(a => a.Name);
                return _authorsString.NullIfEmpty();
            }
        }
    }
}
