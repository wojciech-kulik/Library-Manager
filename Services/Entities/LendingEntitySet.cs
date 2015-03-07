using AutoMapper;
using Common.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Helpers;
using Common;
using DB;

namespace Services.Entities
{
    public class LendingEntitySet : UserAwareEntitySet<Model.Lending, DB.Lending>, ILendingEntitySet
    {
        public LendingEntitySet(string connectionString, string username)
            : base(connectionString, username)
        {
        }

        public IList<Model.Lending> GetLendingsOf(int clientId)
        {
            using (var dataContext = GetDataContext())
            {
                var client = dataContext.Persons.OfType<DB.Client>().FirstOrDefault(x => x.Id == clientId);
                if (client == null)
                    throw new RecordNotFoundException();

                return Mapper.Map<IList<Model.Lending>>(client.Lendings);
            }
        }

        public IList<Model.LentBook> GetLentBooksOf(int lendingId)
        {
            using (var dataContext = GetDataContext())
            {
                var lending = dataContext.Lendings.FirstOrDefault(x => x.Id == lendingId);
                if (lending == null)
                    throw new RecordNotFoundException();

                return Mapper.Map<IList<Model.LentBook>>(lending.Books);
            }
        }

        public void ReturnBooks(Dictionary<int, bool> bookIds, int lendingId)
        {
            if (bookIds == null)
                throw new ArgumentNullException("bookIds");

            DateTime returnDate = DateTime.Now;

            using (var dataContext = GetDataContext())
            {
                DB.Employee currentEmployee = GetCurrentEmployee(dataContext);

                //set ReturnDate and ReturnEmployee according to argument
                dataContext.LentBooks
                    .Where(x => bookIds.Keys.Contains(x.Id))
                    .ForEach(lent =>
                    {
                        if (bookIds[lent.Id])
                        {
                            lent.ReturnDate = returnDate;
                            lent.ReturnEmployee = currentEmployee;
                        }
                        else
                        {
                            lent.ReturnDate = null;
                            lent.ReturnEmployee = null;
                        }
                    });

                //if all books returned, set ReturnDate of whole Lending
                var lending = dataContext.Lendings.FirstOrDefault(l => l.Id == lendingId);
                if (lending == null)
                    throw new RecordNotFoundException();

                if (lending.Books.All(x => x.ReturnDate.HasValue))
                    lending.ReturnDate = returnDate;
                else
                    lending.ReturnDate = null;

                dataContext.SaveChanges();
            }
        }

        public override int Add(Model.Lending entity)
        {
            if (entity == null)
                throw new ArgumentNullException("entity");
            if (entity.ClientId == 0)
                throw new ArgumentNullException("clientId");
            if (entity.Books == null || entity.Books.Count() == 0)
                throw new ArgumentNullException("books");


            using (var dataContext = GetDataContext())
            {
                DB.Lending newLending = Mapper.Map<DB.Lending>(entity);
                newLending.Books = new List<DB.LentBook>();
                newLending.LendingEmployeeId = GetCurrentEmployee(dataContext).Id;

                foreach (var lentBook in entity.Books)
                {
                    DB.LentBook book = Mapper.Map<DB.LentBook>(lentBook);
                    book.Lending = newLending;
                    book.ReturnEmployeeId = (lentBook.ReturnDate != null) ? new int?(newLending.LendingEmployeeId) : null;

                    newLending.Books.Add(book);
                }

                var newRecord = dataContext.Lendings.Add(newLending);
                dataContext.SaveChanges();

                return newRecord.Id;
            }
        }

        private void UpdateSingleLentBook(DB.LentBook dbEntity, Model.LentBook modelEntity, DB.Employee currentEmployee)
        {
            dbEntity.EndDate = modelEntity.EndDate;
            dbEntity.ReturnDate = modelEntity.ReturnDate;

            if (modelEntity.ReturnDate.HasValue)
            {
                if (modelEntity.ReturnEmployeeId != null)
                    dbEntity.ReturnEmployeeId = modelEntity.ReturnEmployeeId;
                else
                    dbEntity.ReturnEmployee = currentEmployee;
            }
            else
                dbEntity.ReturnEmployee = null;
        }

        private void UpdateLentBooks(LibraryDataContext dataContext, DB.Lending currentEntity, Model.Lending modifiedEntity)
        {
            DB.Employee currentEmployee = GetCurrentEmployee(dataContext);
            var newListOfBooks = modifiedEntity.Books.ToList();

            foreach (DB.LentBook book in currentEntity.Books.ToList())
            {
                newListOfBooks.RemoveFirst(x => x.Id == book.Id);
                Model.LentBook newBook = modifiedEntity.Books.FirstOrDefault(x => x.Id == book.Id);

                if (newBook == null) //removed
                {
                    currentEntity.Books.Remove(book);
                    dataContext.LentBooks.Remove(book);
                }
                else //modified
                {
                    UpdateSingleLentBook(book, newBook, currentEmployee);
                }
            }

            //add new LentBooks
            foreach (Model.LentBook book in newListOfBooks)
            {
                DB.LentBook toAdd = Mapper.Map<DB.LentBook>(book);
                toAdd.Lending = currentEntity;
                toAdd.ReturnEmployee = (book.ReturnDate != null) ? currentEmployee : null;
                currentEntity.Books.Add(toAdd);
            }
        }

        public override void Update(Model.Lending entity)
        {
            if (entity == null)
                throw new ArgumentNullException("entity");

            using (var dataContext = GetDataContext())
            {
                var current = dataContext.Lendings.FirstOrDefault(x => x.Id == entity.Id);
                if (current == null)
                    throw new RecordNotFoundException();

                //update Lending object
                current.LendingDate = entity.LendingDate;
                current.ReturnDate = entity.ReturnDate;
                current.EndDate = entity.EndDate;

                //update LentBooks
                UpdateLentBooks(dataContext, current, entity);

                dataContext.SaveChanges();
            }
        }

        public override void Delete(int id)
        {
            using (var dataContext = GetDataContext())
            {
                DB.Lending lending = dataContext.Lendings.FirstOrDefault(l => l.Id == id);
                if (lending == null)
                    throw new RecordNotFoundException();

                foreach (var lentBook in lending.Books.ToList())
                    dataContext.LentBooks.Remove(lentBook);

                dataContext.Lendings.Remove(lending);
                dataContext.SaveChanges();
            }
        }
    }
}
