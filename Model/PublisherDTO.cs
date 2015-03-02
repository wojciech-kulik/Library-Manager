using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace Model
{
    public class PublisherDTO : DictionaryDTO
    {
        public override string ToString()
        {
            return Name;
        }
    }
}