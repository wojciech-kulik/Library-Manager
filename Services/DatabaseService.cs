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

namespace Services
{
    public class DatabaseService : IDatabaseService
    {
        private string _connectionString;
        private string _username;

        static DatabaseService()
        {
            Mapper.CreateMap<Model.Client, DB.Client>().ForMember("Lendings", opt => opt.Ignore());
            Mapper.CreateMap<DB.Client, Model.Client>().ForMember("Lendings", opt => opt.Ignore());
            Mapper.CreateMap<Model.BookCategory, DB.BookCategory>();
            Mapper.CreateMap<DB.BookCategory, Model.BookCategory>();
            Mapper.CreateMap<Model.Book, DB.Book>();
            Mapper.CreateMap<DB.Book, Model.Book>();
            Mapper.CreateMap<DB.Employee, Model.Employee>().ForMember("Password", opt => opt.Ignore()).ForMember("Lendings", opt => opt.Ignore()).ForMember("Returns", opt => opt.Ignore());
            Mapper.CreateMap<Model.Employee, DB.Employee>().ForMember("Lendings", opt => opt.Ignore()).ForMember("Returns", opt => opt.Ignore());
            Mapper.CreateMap<Model.Lending, DB.Lending>().ForMember("Books", opt => opt.Ignore()).ForMember("Client", opt => opt.Ignore());
            Mapper.CreateMap<DB.Lending, Model.Lending>().ForMember("Books", opt => opt.Ignore()).ForMember("Client", opt => opt.Ignore());
            Mapper.CreateMap<Model.Address, DB.Address>();
            Mapper.CreateMap<DB.Address, Model.Address>();
            Mapper.CreateMap<Model.LentBook, DB.LentBook>();
            Mapper.CreateMap<DB.LentBook, Model.LentBook>();
            Mapper.CreateMap<DB.Person, Model.Person>();
            Mapper.CreateMap<Model.Person, DB.Person>();
            Mapper.CreateMap<Model.Author, DB.Author>();
            Mapper.CreateMap<DB.Author, Model.Author>();
            Mapper.CreateMap<Model.Publisher, DB.Publisher>();
            Mapper.CreateMap<DB.Publisher, Model.Publisher>();
        }

        public DatabaseService(string connectionString, string username)
        {
            _connectionString = connectionString;
            _username = username;

            Clients = new ClientEntitySet(connectionString);
            Employees = new EmployeeEntitySet(connectionString, username);
        }

        public void Dispose()
        {
            //TODO: for compatibility - will be removed
        }
        
        private LibraryDataContext GetDataContext()
        {
            var dataContext = new LibraryDataContext(_connectionString);
            dataContext.Database.CreateIfNotExists();

            return dataContext;
        }

        private DB.Employee GetCurrentEmployee(LibraryDataContext dataContext)
        {
            DB.Employee emp = dataContext.Persons.OfType<DB.Employee>().FirstOrDefault(e => e.Username == _username);

            if (emp == null)
                throw new AccessException();
            else
                return emp;
        }

        public Role GetEmployeeRole(string username)
        {
            using (var dataContext = GetDataContext())
            {
                DB.Employee e = dataContext.Persons.OfType<DB.Employee>().FirstOrDefault(emp => emp.Username == username.ToLower());
                if (e == null)
                    throw new RecordNotFoundException();

                return (Role)e.Role;
            }
        }

        public IEntitySet<Model.Client> Clients { get; private set; }

        public IEntitySet<Model.Employee> Employees { get; private set; }


        #region Lendings

        public IList<Model.Lending> GetLendingsOf(int clientId)
        {
            using (var dataContext = GetDataContext())
                return (from l in dataContext.Persons.OfType<DB.Client>().First(c => c.Id == clientId).Lendings
                        select new Model.Lending()
                        {
                            ClientId = l.ClientId,
                            EndDate = l.EndDate,
                            Id = l.Id,
                            LendingDate = l.LendingDate,
                            LendingEmployeeId = l.LendingEmployeeId,
                            LendingEmployee = new Model.Employee()
                                {
                                    Id = l.LendingEmployeeId,
                                    FirstName = l.LendingEmployee.FirstName,
                                    LastName = l.LendingEmployee.LastName
                                },
                            ReturnDate = l.ReturnDate                            
                        }).ToList();
        }

        public IList<Model.LentBook> GetLentBooksOf(int lendingId)
        {
            using (var dataContext = GetDataContext())
                return (from lb in dataContext.Lendings.First(l => l.Id == lendingId).Books
                        select new Model.LentBook()
                        {
                            Id = lb.Id,
                            LendingId = lb.LendingId,
                            ReturnEmployee = lb.ReturnEmployee != null ?
                                                new Model.Employee()
                                                { 
                                                    Id = lb.ReturnEmployeeId.Value,
                                                    FirstName = lb.ReturnEmployee.FirstName,
                                                    LastName = lb.ReturnEmployee.LastName
                                                } : null,
                            ReturnEmployeeId = lb.ReturnEmployeeId,                                                  
                            EndDate = lb.EndDate,
                            ReturnDate = lb.ReturnDate,
                            BookId = lb.BookId,
                            Book = new Model.Book()
                            {                                                        
                                Id = lb.BookId,
                                Title = lb.Book.Title,
                                Location = lb.Book.Location,
                            }
                        }).ToList();
        }       
             
        public void ReturnAllBooks(int lendingId)
        {
            DateTime returnDate = DateTime.Now;

            using (var dataContext = GetDataContext())
            {
                DB.Employee currentEmployee = GetCurrentEmployee(dataContext);
                DB.Lending l = dataContext.Lendings.FirstOrDefault(lending => lending.Id == lendingId);
                if (l == null)
                    throw new InvalidOperationException("Nie znaleziono wypożyczenia o podanym ID. Możliwe, że inny użytkownik właśnie je usunął.");

                l.ReturnDate = returnDate;                

                foreach (var bookLent in l.Books)
                {
                    if (bookLent.ReturnDate == null)
                    {
                        bookLent.ReturnDate = returnDate;
                        bookLent.ReturnEmployee = currentEmployee;
                    }
                }

                dataContext.SaveChanges();
            }
        }

        public void ReturnBooks(Dictionary<int, bool> bookIds, int lendingId)
        {
            if (bookIds == null)
                throw new ArgumentNullException("Serwis bazodanowy otrzymał pustą informacje o zwracanych książkach.", (Exception)null);

            DateTime returnDate = DateTime.Now;            

            using (var dataContext = GetDataContext())
            {
                DB.Employee currentEmployee = GetCurrentEmployee(dataContext);

                //set ReturnDate and ReturnEmployee according to argument
                dataContext.LentBooks
                    .Where(l => bookIds.Keys.Contains(l.Id))
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
                    throw new InvalidOperationException("Nie znaleziono wypożyczenia o podanym ID. Możliwe, że inny użytkownik właśnie je usunął.");

                if (lending.Books.All(len => len.ReturnDate != null))
                    lending.ReturnDate = returnDate;
                else
                    lending.ReturnDate = null;

                dataContext.SaveChanges();
            }
        }

        public void AddLending(Model.Lending lending)
        {
            if (lending == null)
                throw new ArgumentNullException("Serwis bazodanowy otrzymał pustą informacje o wypożyczeniu.", (Exception)null);
            if (lending.ClientId == 0)
                throw new ArgumentNullException("Serwis bazodanowy nie otrzymał informacji do którego klienta ma zostać dodane wypożyczenie.", (Exception)null);
            if (lending.Books == null || lending.Books.Count() == 0)
                throw new ArgumentNullException("Serwis bazodanowy nie otrzymał informacji o wypożyczanych książkach.", (Exception)null);


            using (var dataContext = GetDataContext())
            {
                DB.Lending newLending = new DB.Lending()
                {
                    ClientId = lending.ClientId,
                    Books = new List<DB.LentBook>(),
                    EndDate = lending.EndDate,
                    Id = lending.Id,
                    ReturnDate = lending.ReturnDate,
                    LendingDate = lending.LendingDate,
                    LendingEmployeeId = GetCurrentEmployee(dataContext).Id
                };

                foreach (var lentBook in lending.Books)
                {
                    DB.LentBook lb = new DB.LentBook()
                    {        
                        Lending = newLending,
                        BookId = lentBook.BookId,
                        EndDate = lentBook.EndDate,                        
                        ReturnDate = lentBook.ReturnDate,    
                        ReturnEmployeeId = (lentBook.ReturnDate != null) ? new int?(newLending.LendingEmployeeId) : null
                    };
                    newLending.Books.Add(lb);
                }

                dataContext.Lendings.Add(newLending);
                dataContext.SaveChanges();
            }
        }

        public void EditLending(Model.Lending lending)
        {
            if (lending == null)
                throw new ArgumentNullException("Serwis bazodanowy otrzymał pustą informacje o wypożyczeniu.", (Exception)null);

            using (var dataContext = GetDataContext())
            {
                var current = dataContext.Lendings.FirstOrDefault(l => l.Id == lending.Id);
                if (current == null)
                    throw new InvalidOperationException("Nie znaleziono danego wypożyczenia w bazie. Możliwe, że inny użytkownik właśnie je usunął.");

                DB.Employee currentEmployee = GetCurrentEmployee(dataContext);

                //update Lending object
                current.LendingDate = lending.LendingDate;
                current.ReturnDate = lending.ReturnDate;
                current.EndDate = lending.EndDate;

                //update LentBooks in Lending object
                var listOfNewBooks = lending.Books.ToList();
                foreach (DB.LentBook book in current.Books.ToList())
                {
                    listOfNewBooks.RemoveFirst(lb => lb.Id == book.Id); //to see which books have been added

                    Model.LentBook newBook = lending.Books.FirstOrDefault(b => b.Id == book.Id);

                    if (newBook == null) //lentBook removed
                    {
                        current.Books.Remove(book);
                        dataContext.LentBooks.Remove(book);
                    }
                    else //lentBook modified
                    {
                        book.EndDate = newBook.EndDate;
                        book.ReturnDate = newBook.ReturnDate;
                        if (newBook.ReturnDate != null)
                        {
                            if (newBook.ReturnEmployeeId != null)
                                book.ReturnEmployeeId = newBook.ReturnEmployeeId;
                            else
                                book.ReturnEmployee = currentEmployee;
                        }
                        else
                            book.ReturnEmployee = null;
                    }                 
                }


                //add new LentBooks to Lending object
                foreach (Model.LentBook book in listOfNewBooks)
                {
                    DB.LentBook toAdd = new DB.LentBook()
                    {
                        Lending = current,
                        BookId = book.BookId,                        
                        ReturnEmployee = (book.ReturnDate != null) ? currentEmployee : null,
                        EndDate = book.EndDate,                        
                        ReturnDate = book.ReturnDate                        
                    };
                    current.Books.Add(toAdd);
                }

                dataContext.SaveChanges();
            }
        }

        public void DeleteLending(int clientId, int lendingId)
        {
            using (var dataContext = GetDataContext())
            {
                DB.Client client = dataContext.Persons.OfType<DB.Client>().FirstOrDefault(p => p.Id == clientId);
                DB.Lending lending = dataContext.Lendings.FirstOrDefault(l => l.Id == lendingId);
                if (client == null)
                    throw new InvalidOperationException("Nie znaleziono klienta o podanym ID. Możliwe, że inny użytkownik właśnie go usunął");
                if (lending == null)
                    throw new InvalidOperationException("Nie znaleziono wypożyczenia o podanym ID. Możliwe, że inny użytkownik właśnie je usunął");

                foreach (var lentBook in lending.Books.ToList())
                    dataContext.LentBooks.Remove(lentBook);

                dataContext.Lendings.Remove(lending);
                client.Lendings.Remove(lending);
                dataContext.SaveChanges();
            }
        }

        #endregion

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

        #region Publisher

        public IList<Model.Publisher> GetAllPublishers()
        {
            using (var dataContext = GetDataContext())
            {
                return Mapper.Map<List<Model.Publisher>>(dataContext.Publishers);
            }
        }

        public int AddPublisher(Model.Publisher publisher)
        {
            if (publisher == null)
                throw new ArgumentNullException("Serwis bazodanowy otrzymał pustą informacje o wydawcy.", (Exception)null);

            using (var dataContext = GetDataContext())
            {
                DB.Publisher newPub = new DB.Publisher() { Name = publisher.Name };
                dataContext.Publishers.Add(newPub);
                dataContext.SaveChanges();

                return newPub.Id;
            }
        }

        #endregion

        #region BookCategories

        public IList<Model.BookCategory> GetAllBookCategories()
        {
            using (var dataContext = GetDataContext())
            {
                return Mapper.Map<List<Model.BookCategory>>(dataContext.BookCategories);
            }
        }

        public int AddBookCategory(Model.BookCategory bookCategory)
        {
            if (bookCategory == null)
                throw new ArgumentNullException("Serwis bazodanowy otrzymał pustą informacje o kategorii.", (Exception)null);

            using (var dataContext = GetDataContext())
            {
                DB.BookCategory newBookCat = new DB.BookCategory() { Name = bookCategory.Name };
                dataContext.BookCategories.Add(newBookCat);
                dataContext.SaveChanges();

                return newBookCat.Id;
            }
        }

        #endregion

        #region Authors

        public IList<Model.Author> GetAllAuthors()
        {
            using (var dataContext = GetDataContext())
            {
                return Mapper.Map<List<Model.Author>>(dataContext.Authors);
            }
        }

        public int AddAuthor(Model.Author author)
        {
            if (author == null)
                throw new ArgumentNullException("Serwis bazodanowy otrzymał pustą informacje o autorze.", (Exception)null);

            using (var dataContext = GetDataContext())
            {
                DB.Author newAuthor = new DB.Author() { Name = author.Name };
                dataContext.Authors.Add(newAuthor);
                dataContext.SaveChanges();

                return newAuthor.Id;
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
