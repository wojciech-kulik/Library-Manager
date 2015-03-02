using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Model
{
    public partial class LentBookDTO
    {
        public int Id { get; set; }

        public Nullable<DateTime> ReturnDate { get; set; }

        public EmployeeDTO ReturnEmployee { get; set; }

        public Nullable<int> ReturnEmployeeId { get; set; }

        public DateTime EndDate { get; set; }

        public int BookId { get; set; }

        public int LendingId { get; set; }

        public BookDTO Book { get; set; }

        public LendingDTO Lending { get; set; }
    }
}
