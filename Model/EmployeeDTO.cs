using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Model
{
    public partial class EmployeeDTO : PersonDTO
    {
        public byte Role { get; set; }

        public string Username { get; set; }

        public string Password { get; set; }

        public ICollection<LendingDTO> Lendings { get; set; }

        public ICollection<LentBookDTO> Returns { get; set; }

        public bool Removed { get; set; }
    }
}
