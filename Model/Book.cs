using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Model
{
    public partial class Book : ModelBase, IIdRecord
    {
        public int Id { get; set; }

        public string Title { get; set; }

        public string ISBN { get; set; }

        public bool HardCover { get; set; }

        public Nullable<DateTime> PublishDate { get; set; }

        public short Quantity { get; set; }

        public string Location { get; set; }

        public ICollection<BookCategory> BookCategories { get; set; }

        public Publisher Publisher { get; set; }

        public Nullable<int> PublisherId { get; set; }

        public ICollection<Author> Authors { get; set; }

        public string AdditionalInfo { get; set; }

        public bool Removed { get; set; }

        public bool Available { get; set; }
    }
}
