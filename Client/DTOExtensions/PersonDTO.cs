using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClientApplication.DBService
{
    public partial class PersonDTO
    {
        public PersonDTO()
        {
            Address = new AddressDTO();
        }

        public string FullName
        {
            get
            {
                return FirstName + " " + LastName;
            }
        }
    }
}
