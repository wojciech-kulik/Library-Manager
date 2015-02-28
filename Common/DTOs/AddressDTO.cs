using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Common
{
    public class AddressDTO
    {
        public string Street { get; set; }

        public string HouseNumber { get; set; }

        public string ApartmentNumber { get; set; }

        public string PostalCode { get; set; }

        public string City { get; set; }
    }
}
