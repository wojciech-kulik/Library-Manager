using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Helpers;
using Common;
using DB;
using Common.Exceptions;
using AutoMapper;

namespace Services.Entities
{
    public class BookEntitySet : EntitySet<Model.Book, DB.Book>
    {
        public BookEntitySet(string connectionString)
            : base(connectionString)
        {
        }

        private void UpdateAvailability(LibraryDataContext dataContext, Model.Book book)
        {
            book.Available = dataContext.LentBooks.Count(y => y.ReturnDate == null && y.BookId == book.Id) < book.Quantity;
        }

        public override IList<Model.Book> GetAll()
        {
            var books = base.GetAll();
            using (var dataContext = GetDataContext())
            {
                return books.ForEach(x =>
                {
                    UpdateAvailability(dataContext, x);
                });
            }
        }

        public override Model.Book Get(int id)
        {
            using (var dataContext = GetDataContext())
            {
                var book = base.Get(id);
                UpdateAvailability(dataContext, book);
                return book;
            }
        }

        protected override void BeforeAdd(LibraryDataContext dataContext, Model.Book entity, DB.Book record)
        {
            MapCollection<Model.Author, DB.Author>(dataContext, entity.Authors, record.Authors);
            MapCollection<Model.BookCategory, DB.BookCategory>(dataContext, entity.BookCategories, record.BookCategories);
            record.Publisher = entity.PublisherId.HasValue ? dataContext.Publishers.FirstOrDefault(x => x.Id == entity.PublisherId) : null;
        }

        protected override void BeforeUpdate(LibraryDataContext dataContext, Model.Book entity, DB.Book record)
        {
            UpdateCollection<Model.Author, DB.Author>(dataContext, entity.Authors, record.Authors);
            UpdateCollection<Model.BookCategory, DB.BookCategory>(dataContext, entity.BookCategories, record.BookCategories);
            record.Publisher = entity.PublisherId.HasValue ? dataContext.Publishers.FirstOrDefault(x => x.Id == entity.PublisherId) : null;
        }

        public override void Delete(int id)
        {
            using (var dataContext = GetDataContext())
            {
                DB.Book toDelete = dataContext.Books.FirstOrDefault(x => x.Id == id);
                if (toDelete == null)
                    throw new RecordNotFoundException();

                if (dataContext.LentBooks.Any(x => x.BookId == toDelete.Id))
                {
                    toDelete.Removed = true;
                }
                else
                {
                    toDelete.BookCategories.Clear();
                    toDelete.Authors.Clear();
                    dataContext.Books.Remove(toDelete);
                }

                dataContext.SaveChanges();
            }
        }

    }
}
