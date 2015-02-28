using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Common
{
    public class LendingDTO
    {
        public int Id { get; set; }

        public int ClientId { get; set; }

        public DateTime EndDate { get; set; }

        public DateTime LendingDate { get; set; }

        public Nullable<DateTime> ReturnDate { get; set; }

        public ClientDTO Client { get; set; }

        public ICollection<LentBookDTO> Books { get; set; }

        public EmployeeDTO LendingEmployee { get; set; }

        public int LendingEmployeeId { get; set; }        
    }
}
