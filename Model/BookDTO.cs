using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Model
{
    public partial class BookDTO
    {
        public int Id { get; set; }

        public string Title { get; set; }

        public string ISBN { get; set; }

        public bool HardCover { get; set; }

        public Nullable<DateTime> PublishDate { get; set; }

        public short Quantity { get; set; }

        public string Location { get; set; }

        public ICollection<BookCategoryDTO> BookCategories { get; set; }

        public PublisherDTO Publisher { get; set; }

        public Nullable<int> PublisherId { get; set; }

        public ICollection<AuthorDTO> Authors { get; set; }

        public string AdditionalInfo { get; set; }

        public bool Removed { get; set; }

        public bool Available { get; set; }
    }
}
