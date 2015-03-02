using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace Common
{
    public class PublisherDTO : DictionaryDTO
    {
        public override string ToString()
        {
            return Name;
        }
    }
}