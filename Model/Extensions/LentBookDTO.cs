using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Model
{
    public partial class LentBookDTO
    {
        public bool IsReturnedChanged { get; set; }

        private bool _isReturned;
        public bool IsReturned 
        {
            get
            {
                if (!IsReturnedChanged)
                    return ReturnDate != null;
                else
                    return _isReturned;
            }
            set
            {
                _isReturned = value;
                IsReturnedChanged = value != (ReturnDate != null);
            }
        }        
    }
}
