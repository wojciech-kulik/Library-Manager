using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Model
{
    public class Lending
    {
        public int Id { get; set; }

        public int ClientId { get; set; }

        public DateTime EndDate { get; set; }

        public DateTime LendingDate { get; set; }

        public Nullable<DateTime> ReturnDate { get; set; }

        public Client Client { get; set; }

        public ICollection<LentBook> Books { get; set; }

        public Employee LendingEmployee { get; set; }

        public int LendingEmployeeId { get; set; }        
    }
}
