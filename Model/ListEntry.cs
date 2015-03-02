using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace Model
{
    public class ListEntry : ModelBase
    {
        public int Id { get; set; }

        public string Name { get; set; }
    }
}