using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Model
{
    public partial class Employee : Person
    {
        public byte Role { get; set; }

        public string Username { get; set; }

        public string Password { get; set; }

        public ICollection<Lending> Lendings { get; set; }

        public ICollection<LentBook> Returns { get; set; }

        public bool Removed { get; set; }
    }
}
