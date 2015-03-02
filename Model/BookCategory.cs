using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Model
{
    public class BookCategory : ModelBase
    {
        public int Id { get; set; }

        public string Name { get; set; }
    }
}
