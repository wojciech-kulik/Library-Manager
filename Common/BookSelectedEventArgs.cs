using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common
{
    public class BookSelectedEventArgs<TBook> : EventArgs
        where TBook : class
    {
        public TBook Book { get; set; }
    }
}
