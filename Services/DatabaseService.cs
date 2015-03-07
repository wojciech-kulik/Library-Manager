using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using DB;
using AutoMapper;
using System.Collections;
using Helpers;
using System.Diagnostics;
using AutoMapper.Mappers;
using DevOne.Security.Cryptography.BCrypt;
using Model;
using Common;
using Services.Entities;
using Common.Exceptions;
using Services.Utils;

namespace Services
{
    public class DatabaseService : IDatabaseService
    {
        private string _connectionString;
        private string _username;

        static DatabaseService()
        {
            MapperHelper.InitializeMappings();
        }

        public DatabaseService(string connectionString, string username)
        {
            _connectionString = connectionString;
            _username = username;

            Clients = new ClientEntitySet(connectionString);
            Employees = new EmployeeEntitySet(connectionString, username);
            Lendings = new LendingEntitySet(connectionString, username);

            Authors = new EntitySet<Model.Author, DB.Author>(connectionString);
            Publishers = new EntitySet<Model.Publisher, DB.Publisher>(connectionString);
            BookCategories = new EntitySet<Model.BookCategory, DB.BookCategory>(connectionString);

        }

        public void Dispose()
        {
            //TODO: for compatibility - will be removed
        }





        public IEntitySet<Model.Client> Clients { get; private set; }

        public IEmployeeEntitySet Employees { get; private set; }

        public ILendingEntitySet Lendings { get; private set; }

        public IEntitySet<Model.Author> Authors { get; private set; }

        public IEntitySet<Model.Publisher> Publishers { get; private set; }

        public IEntitySet<Model.BookCategory> BookCategories { get; private set; }


        private LibraryDataContext GetDataContext()
        {
            var dataContext = new LibraryDataContext(_connectionString);
            dataContext.Database.CreateIfNotExists();

            return dataContext;
        }

        #region Books

        public IList<Model.Book> GetAllBooks()
        {
            using (var dataContext = GetDataContext())
            {
                return (from book in dataContext.Books
                        where !book.Removed
                        select new
                        {
                            Id = book.Id,
                            Authors = book.Authors,
                            ISBN = book.ISBN,
                            PublisherId = book.PublisherId,
                            Publisher = new Model.Publisher()
                                        {
                                            Id = (book.Publisher == null) ? -1 : book.Publisher.Id,
                                            Name = (book.Publisher == null) ? "" : book.Publisher.Name
                                        },
                            HardCover = book.HardCover,
                            PublishDate = book.PublishDate,
                            Location = book.Location,
                            Quantity = book.Quantity,
                            Title = book.Title,
                            BookCategories = book.BookCategories,
                            AdditionalInfo = book.AdditionalInfo,
                            Available = dataContext.LentBooks.Where(lb => lb.ReturnDate == null && lb.BookId == book.Id).Count() < book.Quantity
                        }).AsEnumerable().Select(b => new Model.Book()
                        {                            
                            Authors = (from a in b.Authors
                                      select new Model.Author()
                                      {
                                          Id = a.Id,
                                          Name = a.Name
                                      }).ToList(),                                   
                            BookCategories = (from cat in b.BookCategories
                                                select new Model.BookCategory()
                                                {
                                                    Id = cat.Id,
                                                    Name = cat.Name
                                                }).ToList(),
                            Publisher = b.Publisher,
                            PublisherId = b.PublisherId,
                            Id = b.Id,
                            ISBN = b.ISBN,
                            HardCover = b.HardCover,
                            PublishDate = b.PublishDate,
                            Location = b.Location,
                            Quantity = b.Quantity,
                            Title = b.Title,
                            AdditionalInfo = b.AdditionalInfo,
                            Available = b.Available
                        }).ToList().ForEach<Model.Book>(bb => { if (bb.Publisher.Id == -1) bb.Publisher = null; });
            }
        }

        public Model.Book GetBook(int bookId)
        {
            using (var dataContext = GetDataContext())
            {
                var book = dataContext.Books.FirstOrDefault(b => b.Id == bookId);

                if (book == null)
                    throw new ArgumentOutOfRangeException("Nie znaleziono książki o podanym ID.", (Exception)null);
                else
                {
                    var result = Mapper.Map<Model.Book>(book);
                    result.Available = dataContext.LentBooks.Where(lb => lb.ReturnDate == null && lb.BookId == result.Id).Count() < result.Quantity;
                    return result;
                }
            }
        }

        public void AddBook(Model.Book book)
        {
            if (book == null)
                throw new ArgumentNullException("Serwis bazodanowy otrzymał pustą informacje o książce.", (Exception)null);

            using (var dataContext = GetDataContext())
            {
                DB.Book newBook = Mapper.Map<DB.Book>(book);

                //copy authors
                List<Model.Author> authorsModel = book.Authors.ToList();
                newBook.Authors = new List<DB.Author>();
                foreach (var author in authorsModel)
                {
                    DB.Author a = dataContext.Authors.FirstOrDefault(aa => aa.Id == author.Id);
                    if (a != null)
                        newBook.Authors.Add(a);
                    else
                        newBook.Authors.Add(new DB.Author() { Name = author.Name });
                }

                //copy bookCategories
                List<Model.BookCategory> bookCategoriesModel = book.BookCategories.ToList();
                newBook.BookCategories = new List<DB.BookCategory>();
                foreach (var bookCat in bookCategoriesModel)
                {
                    DB.BookCategory b = dataContext.BookCategories.FirstOrDefault(bc => bc.Id == bookCat.Id);
                    if (b != null)
                        newBook.BookCategories.Add(b);
                    else
                        newBook.BookCategories.Add(new DB.BookCategory() { Name = bookCat.Name });
                }

                dataContext.Books.Add(newBook);
                dataContext.SaveChanges();
            }
        }

        public void EditBook(Model.Book book)
        {
            if (book == null)
                throw new ArgumentNullException("Serwis bazodanowy otrzymał pustą informacje o książce.", (Exception)null);

            using (var dataContext = GetDataContext())
            {
                DB.Book toEdit = dataContext.Books.FirstOrDefault(b => b.Id == book.Id);
                if (toEdit == null)
                    throw new InvalidOperationException("Podana książka nie została znaleziona w bazie danych. Możliwe, że inny użytkownik właśnie ją usunął.");

                //updating info
                toEdit.AdditionalInfo = book.AdditionalInfo;
                toEdit.HardCover = book.HardCover;
                toEdit.ISBN = book.ISBN;
                toEdit.Location = book.Location;
                toEdit.PublishDate = book.PublishDate;
                toEdit.Publisher = null;
                toEdit.PublisherId = book.PublisherId;
                toEdit.Quantity = book.Quantity;
                toEdit.Removed = book.Removed;
                toEdit.Title = book.Title;

                #region updating authors

                var newListOfAuthors = book.Authors.ToList();
                foreach (var oldAuthor in toEdit.Authors.ToList())
                {
                    Model.Author newAuthor = newListOfAuthors.FirstOrDefault(a => a.Id == oldAuthor.Id);

                    //removed author
                    if (newAuthor == null)
                        toEdit.Authors.Remove(oldAuthor);
                    else
                        newListOfAuthors.Remove(newAuthor);
                }
                //add new authors
                foreach (var newAuthor in newListOfAuthors)
                    toEdit.Authors.Add(dataContext.Authors.FirstOrDefault(a => a.Id == newAuthor.Id));

                #endregion

                #region updating bookCategories

                var newListOfBookCategories = book.BookCategories.ToList();
                foreach (var oldCat in toEdit.BookCategories.ToList())
                {
                    Model.BookCategory newCat = newListOfBookCategories.FirstOrDefault(bc => bc.Id == oldCat.Id);

                    //removed category
                    if (newCat == null)
                        toEdit.BookCategories.Remove(oldCat);
                    else
                        newListOfBookCategories.Remove(newCat);
                }
                //add remained categories
                foreach (var newCat in newListOfBookCategories)
                    toEdit.BookCategories.Add(dataContext.BookCategories.FirstOrDefault(bc => bc.Id == newCat.Id));

                #endregion

                dataContext.SaveChanges();
            }
        }

        public void DeleteBook(int bookId)
        {
            using (var dataContext = GetDataContext())
            {
                DB.Book toDel = dataContext.Books.FirstOrDefault(b => b.Id == bookId);
                if (toDel == null)
                    throw new InvalidOperationException("Podana książka nie została znaleziona w bazie danych. Możliwe, że inny użytkownik właśnie ją usunął.");

                if (dataContext.LentBooks.Any(lb => lb.BookId == toDel.Id))
                    toDel.Removed = true;
                else
                {
                    foreach (var author in toDel.Authors.ToList())
                        toDel.Authors.Remove(author);
                    foreach (var bookCat in toDel.BookCategories.ToList())
                        toDel.BookCategories.Remove(bookCat);

                    dataContext.Books.Remove(toDel);
                }

                dataContext.SaveChanges();
            }
        }

        #endregion

        #region Authentication

        public bool Authenticate(string username, string password)
        {
            try
            {
                if (IsFirstLogIn())
                {
                    Employees.Add(new Model.Employee()
                    {
                        Username = username,
                        Password = password,
                        FirstName = "Admin",
                        LastName = "Admin",
                        Role = (byte)Role.Admin
                    });
                    return true;
                }

                using (var dataContext = GetDataContext())
                {
                    var user = dataContext.Persons.OfType<DB.Employee>().First(x => x.Username == username);
                    if (BCryptHelper.CheckPassword(password, user.Password))
                    {
                        return true;
                    }
                }
            }
            catch { }

            return false;
        }

        public bool IsFirstLogIn()
        {
            using(var dataContext = GetDataContext())
            {
                return dataContext.Persons.OfType<DB.Employee>().Count() == 0;
            }
        }

        #endregion
    }
}
