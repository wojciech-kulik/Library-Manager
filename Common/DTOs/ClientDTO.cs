using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Common
{
    public class ClientDTO : PersonDTO
    {
        public string CardNumber { get; set; }

        public string AdditionalInfo { get; set; }

        public ICollection<LendingDTO> Lendings { get; set; }
    }
}
