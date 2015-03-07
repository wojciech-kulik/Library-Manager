using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Model
{
    public partial class LentBook : ModelBase, IIdRecord
    {
        public int Id { get; set; }

        private Nullable<DateTime> _returnDate;
        public Nullable<DateTime> ReturnDate
        {
            get
            {
                return _returnDate;
            }
            set
            {
                if (value != _returnDate)
                {
                    _returnDate = value;
                    NotifyOfPropertyChange();
                }
            }
        }

        public Employee ReturnEmployee { get; set; }

        public Nullable<int> ReturnEmployeeId { get; set; }

        private DateTime _endDate;
        public DateTime EndDate
        {
            get
            {
                return _endDate;
            }
            set
            {
                if (value != _endDate)
                {
                    _endDate = value;
                    NotifyOfPropertyChange();
                }
            }
        }

        public int BookId { get; set; }

        public int LendingId { get; set; }

        public Book Book { get; set; }

        public Lending Lending { get; set; }
    }
}
