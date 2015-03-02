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

namespace Services
{
    public class DatabaseService : IDatabaseService
    {
        private string _connectionString;
        private string _username;
        

        private LibraryDataContext GetDataContext()
        {
            var dataContext = new LibraryDataContext(_connectionString);
            dataContext.Database.CreateIfNotExists();

            return dataContext;
        }

        private Employee GetCurrentEmployee(LibraryDataContext dataContext)
        {
            Employee emp = dataContext.Persons.OfType<Employee>().FirstOrDefault(e => e.Username == _username);

            if (emp == null)
                throw new Exception(String.Format("Employee '{0}' not found!", _username));
            else
                return emp;
        }

        public void Dispose()
        {
            //TODO: for compatibility - will be removed
        }

        static DatabaseService()
        {
            Mapper.CreateMap<ClientDTO, Client>().ForMember("Lendings", opt => opt.Ignore());
            Mapper.CreateMap<Client, ClientDTO>().ForMember("Lendings", opt => opt.Ignore());
            Mapper.CreateMap<BookCategory, BookCategoryDTO>();
            Mapper.CreateMap<BookCategoryDTO, BookCategory>();
            Mapper.CreateMap<Book, BookDTO>();
            Mapper.CreateMap<BookDTO, Book>();
            Mapper.CreateMap<Employee, EmployeeDTO>().ForMember("Password", opt => opt.Ignore()).ForMember("Lendings", opt => opt.Ignore()).ForMember("Returns", opt => opt.Ignore());
            Mapper.CreateMap<EmployeeDTO, Employee>().ForMember("Lendings", opt => opt.Ignore()).ForMember("Returns", opt => opt.Ignore());
            Mapper.CreateMap<Lending, LendingDTO>().ForMember("Books", opt => opt.Ignore()).ForMember("Client", opt => opt.Ignore());
            Mapper.CreateMap<LendingDTO, Lending>().ForMember("Books", opt => opt.Ignore()).ForMember("Client", opt => opt.Ignore());
            Mapper.CreateMap<Model.Address, DB.Address>();
            Mapper.CreateMap<DB.Address, Model.Address>();
            Mapper.CreateMap<LentBook, LentBookDTO>();
            Mapper.CreateMap<LentBookDTO, LentBook>();
            Mapper.CreateMap<Person, PersonDTO>();
            Mapper.CreateMap<PersonDTO, Person>();
            Mapper.CreateMap<Author, AuthorDTO>();
            Mapper.CreateMap<AuthorDTO, Author>();
            Mapper.CreateMap<Publisher, PublisherDTO>();
            Mapper.CreateMap<PublisherDTO, Publisher>();
        }

        public DatabaseService(string connectionString, string username)
        {
            _connectionString = connectionString;
            _username = username;
            return;
            #region test creation
            try
            {
                var _dataContext = GetDataContext();

                Client c = new Client() { FirstName = "Jacek", LastName = "Kowalski", Phone = "6431231", IdNumber = "ARR21231", Email = "mail@o2.pl", CardNumber = "112312" };
                c.Address.City = "Miasteczko";
                c.Address.HouseNumber = "25";
                c.Address.Street = "Uliczka";
                c.Address.PostalCode = "50-500";

                Employee emp = new Employee() { FirstName = "Jacek", LastName = "Kowalski", Phone = "6431231", IdNumber = "ARR21231", Username = "Login", Password = "pass" };
                emp.Address.City = "Miasteczko";
                emp.Address.HouseNumber = "25";
                emp.Address.Street = "Uliczka";
                emp.Address.PostalCode = "50-500";

                BookCategory bc = new BookCategory() { Name = "Fantasy" };
                BookCategory bc2 = new BookCategory() { Name = "Przygodowa" };

                Author author = new Author() { Name = "Tolkien" };
                Author author2 = new Author() { Name = "Stephen King" };

                Publisher publisher = new Publisher() { Name = "Helion" };

                Book b = new Book() { Authors = new List<Author>() { { author }, { author2 } }, Title = "Władca Pierścieni", Location = "2AB", BookCategories = new List<BookCategory>() { { bc }, { bc2 } } };
                Book b2 = new Book() { Authors = new List<Author>() { { author2 } }, Title = "Ręka mistrza", Location = "AA2", ISBN = "123132131231", PublishDate = DateTime.Now.AddYears(-1), Publisher = publisher, BookCategories = new List<BookCategory>() { {bc} } };              

                LentBook lb = new LentBook() { Book = b, EndDate = DateTime.Now.AddDays(7) };
                LentBook lb2 = new LentBook() { Book = b2, EndDate = DateTime.Now.AddDays(7) };

                Lending l = new Lending() { LendingDate = DateTime.Now, EndDate = DateTime.Now.AddDays(7), LendingEmployee = emp, Client = c };
                l.Books.Add(lb);
                l.Books.Add(lb2);

                c.Lendings.Add(l);

                _dataContext.Persons.Add(emp);
                _dataContext.Persons.Add(c);
                _dataContext.SaveChanges();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            #endregion
        }


        #region Client

        public IList<ClientDTO> GetAllClients()
        {
            using (var dataContext = GetDataContext())
                return Mapper.Map<List<ClientDTO>>(dataContext.Persons.OfType<Client>());            
        }

        public ClientDTO GetClient(int clientId)
        {
            using (var dataContext = GetDataContext())
            {
                var client = dataContext.Persons.OfType<Client>().FirstOrDefault(c => c.Id == clientId);

                if (client == null)
                    throw new ArgumentOutOfRangeException("Nie znaleziono klienta o podanym ID.", (Exception)null);
                else
                    return Mapper.Map<ClientDTO>(client);
            }
        }

        public void DeleteClient(int clientId)
        {
            using (var dataContext = GetDataContext())
            {
                Client client = dataContext.Persons.OfType<Client>().FirstOrDefault(p => p.Id == clientId);
                if (client == null)
                    throw new InvalidOperationException("Podany klient nie został znaleziony w bazie danych. Możliwe, że inny użytkownik właśnie go usunął.");

                foreach (var lending in client.Lendings.ToList())
                {
                    foreach (var lentBook in lending.Books.ToList())
                        dataContext.LentBooks.Remove(lentBook);

                    dataContext.Lendings.Remove(lending);
                }
                dataContext.Persons.Remove(client);
                dataContext.SaveChanges();
            }
        }

        public void AddClient(ClientDTO client)
        {
            if (client == null)
                throw new ArgumentNullException("Serwis bazodanowy otrzymał pustą informacje o kliencie.", (Exception)null);

            using (var dataContext = GetDataContext())
            {
                dataContext.Persons.Add(Mapper.Map<Client>(client));
                dataContext.SaveChanges();
            }
        }

        public void EditClient(ClientDTO client)
        {
            if (client == null)
                throw new ArgumentNullException("Serwis bazodanowy otrzymał pustą informacje o kliencie.", (Exception)null);

            using (var dataContext = GetDataContext())
            {
                Client c = dataContext.Persons.OfType<Client>().FirstOrDefault(cl => cl.Id == client.Id);
                if (c == null)
                    throw new InvalidOperationException("Podany klient nie został znaleziony w bazie danych. Możliwe, że inny użytkownik właśnie go usunął.");

                Mapper.Map<ClientDTO, Client>(client, c);
                dataContext.SaveChanges();
            }
        }

        #endregion

        #region Employee

        public byte GetEmployeeRole(string username)
        {
            using (var dataContext = GetDataContext())
            {
                Employee e = dataContext.Persons.OfType<Employee>().FirstOrDefault(emp => emp.Username == username.ToLower());
                if (e == null)
                    return (byte)Role.Admin; 
                    //throw new ArgumentOutOfRangeException("Nie znaleziono pracownika o danej nazwie.");

                return e.Role;
            }
        }

        public IList<EmployeeDTO> GetAllEmployees()
        {
            using (var dataContext = GetDataContext())
                return Mapper.Map<List<EmployeeDTO>>(dataContext.Persons.OfType<Employee>().Where(emp => !emp.Removed));
        }

        public EmployeeDTO GetEmployee(int employeeId)
        {
            using (var dataContext = GetDataContext())
            {
                var employee = dataContext.Persons.OfType<Employee>().FirstOrDefault(e => e.Id == employeeId);

                if (employee == null)
                    throw new ArgumentOutOfRangeException("Nie znaleziono pracownika o podanym ID.", (Exception)null);
                else
                    return Mapper.Map<EmployeeDTO>(employee);
            }
        }

        public void DeleteEmployee(int employeeId)
        {
            using (var dataContext = GetDataContext())
            {
                Employee employee = dataContext.Persons.OfType<Employee>().FirstOrDefault(e => e.Id == employeeId);
                if (employee.Username == GetCurrentEmployee(dataContext).Username)
                    throw new InvalidOperationException("Nie można usunąć samego siebie.");
                if (employee == null)
                    throw new InvalidOperationException("Podany pracownik nie został znaleziony w bazie danych. Możliwe, że inny użytkownik właśnie go usunął.");

                if (employee.Lendings.Count() > 0 || employee.Returns.Count() > 0)
                    employee.Removed = true;
                else
                    dataContext.Persons.Remove(employee);

                dataContext.SaveChanges();
            }
        }

        public void AddEmployee(EmployeeDTO employee)
        {
            if (employee == null)
                throw new ArgumentNullException("Serwis bazodanowy otrzymał pustą informacje o pracowniku.", (Exception)null);

            using (var dataContext = GetDataContext())
            {
                employee.Password = BCryptHelper.HashPassword(employee.Password, BCryptHelper.GenerateSalt(10));
                employee.Username = employee.Username.ToLower();

                if (dataContext.Persons.OfType<Employee>().Any(emp => !emp.Removed && emp.Username == employee.Username))
                    throw new InvalidOperationException("Użytkownik o podanej nazwie istnieje już w systemie.");

                dataContext.Persons.Add(Mapper.Map<Employee>(employee));
                dataContext.SaveChanges();
            }
        }

        public void EditEmployee(EmployeeDTO employee)
        {
            if (employee == null)
                throw new ArgumentNullException("Serwis bazodanowy otrzymał pustą informacje o pracowniku.", (Exception)null);

            using (var dataContext = GetDataContext())
            {
                Employee toEdit = dataContext.Persons.OfType<Employee>().FirstOrDefault(e => e.Id == employee.Id);
                if (toEdit == null)
                    throw new InvalidOperationException("Podany pracownik nie został znaleziony w bazie danych. Możliwe, że inny użytkownik właśnie go usunął.");

                string oldPass = toEdit.Password;
                employee.Username = employee.Username.ToLower();

                if (dataContext.Persons.OfType<Employee>().Any(emp => !emp.Removed && emp.Id != employee.Id && emp.Username == employee.Username))
                    throw new InvalidOperationException("Użytkownik o podanej nazwie istnieje już w systemie.");
                if (employee.Username == GetCurrentEmployee(dataContext).Username && (Role)employee.Role != Role.Admin)
                    throw new InvalidOperationException("Nie możesz sam sobie odebrać administratora.");

                Mapper.Map<EmployeeDTO, Employee>(employee, toEdit);

                if (!String.IsNullOrWhiteSpace(toEdit.Password))
                    toEdit.Password = BCryptHelper.HashPassword(toEdit.Password, BCryptHelper.GenerateSalt(10));
                else
                    toEdit.Password = oldPass;

                dataContext.SaveChanges();
            }
        }

        #endregion

        #region Lendings

        public IList<LendingDTO> GetLendingsOf(int clientId)
        {
            using (var dataContext = GetDataContext())
                return (from l in dataContext.Persons.OfType<Client>().First(c => c.Id == clientId).Lendings
                        select new LendingDTO()
                        {
                            ClientId = l.ClientId,
                            EndDate = l.EndDate,
                            Id = l.Id,
                            LendingDate = l.LendingDate,
                            LendingEmployeeId = l.LendingEmployeeId,
                            LendingEmployee = new EmployeeDTO()
                                {
                                    Id = l.LendingEmployeeId,
                                    FirstName = l.LendingEmployee.FirstName,
                                    LastName = l.LendingEmployee.LastName
                                },
                            ReturnDate = l.ReturnDate                            
                        }).ToList();
        }

        public IList<LentBookDTO> GetLentBooksOf(int lendingId)
        {
            using (var dataContext = GetDataContext())
                return (from lb in dataContext.Lendings.First(l => l.Id == lendingId).Books
                        select new LentBookDTO()
                        {
                            Id = lb.Id,
                            LendingId = lb.LendingId,
                            ReturnEmployee = lb.ReturnEmployee != null ? 
                                                new EmployeeDTO()
                                                { 
                                                    Id = lb.ReturnEmployeeId.Value,
                                                    FirstName = lb.ReturnEmployee.FirstName,
                                                    LastName = lb.ReturnEmployee.LastName
                                                } : null,
                            ReturnEmployeeId = lb.ReturnEmployeeId,                                                  
                            EndDate = lb.EndDate,
                            ReturnDate = lb.ReturnDate,
                            BookId = lb.BookId,
                            Book = new BookDTO()
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
                Employee currentEmployee = GetCurrentEmployee(dataContext);
                Lending l = dataContext.Lendings.FirstOrDefault(lending => lending.Id == lendingId);
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
                Employee currentEmployee = GetCurrentEmployee(dataContext);

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

        public void AddLending(LendingDTO lending)
        {
            if (lending == null)
                throw new ArgumentNullException("Serwis bazodanowy otrzymał pustą informacje o wypożyczeniu.", (Exception)null);
            if (lending.ClientId == 0)
                throw new ArgumentNullException("Serwis bazodanowy nie otrzymał informacji do którego klienta ma zostać dodane wypożyczenie.", (Exception)null);
            if (lending.Books == null || lending.Books.Count() == 0)
                throw new ArgumentNullException("Serwis bazodanowy nie otrzymał informacji o wypożyczanych książkach.", (Exception)null);


            using (var dataContext = GetDataContext())
            {
                Lending newLending = new Lending()
                {
                    ClientId = lending.ClientId,
                    Books = new List<LentBook>(),
                    EndDate = lending.EndDate,
                    Id = lending.Id,
                    ReturnDate = lending.ReturnDate,
                    LendingDate = lending.LendingDate,
                    LendingEmployeeId = GetCurrentEmployee(dataContext).Id
                };

                foreach (var lentBook in lending.Books)
                {
                    LentBook lb = new LentBook()
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

        public void EditLending(LendingDTO lending)
        {
            if (lending == null)
                throw new ArgumentNullException("Serwis bazodanowy otrzymał pustą informacje o wypożyczeniu.", (Exception)null);

            using (var dataContext = GetDataContext())
            {
                var current = dataContext.Lendings.FirstOrDefault(l => l.Id == lending.Id);
                if (current == null)
                    throw new InvalidOperationException("Nie znaleziono danego wypożyczenia w bazie. Możliwe, że inny użytkownik właśnie je usunął.");

                Employee currentEmployee = GetCurrentEmployee(dataContext);

                //update Lending object
                current.LendingDate = lending.LendingDate;
                current.ReturnDate = lending.ReturnDate;
                current.EndDate = lending.EndDate;

                //update LentBooks in Lending object
                var listOfNewBooks = lending.Books.ToList();
                foreach (LentBook book in current.Books.ToList())
                {
                    listOfNewBooks.RemoveFirst(lb => lb.Id == book.Id); //to see which books have been added

                    LentBookDTO newBook = lending.Books.FirstOrDefault(b => b.Id == book.Id);

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
                foreach (LentBookDTO book in listOfNewBooks)
                {
                    LentBook toAdd = new LentBook()
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
                Client client = dataContext.Persons.OfType<Client>().FirstOrDefault(p => p.Id == clientId);
                Lending lending = dataContext.Lendings.FirstOrDefault(l => l.Id == lendingId);
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

        public IList<BookDTO> GetAllBooks()
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
                            Publisher = new PublisherDTO()
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
                        }).AsEnumerable().Select(b => new BookDTO()
                        {                            
                            Authors = (from a in b.Authors
                                      select new AuthorDTO()
                                      {
                                          Id = a.Id,
                                          Name = a.Name
                                      }).ToList(),                                   
                            BookCategories = (from cat in b.BookCategories
                                                select new BookCategoryDTO()
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
                        }).ToList().ForEach<BookDTO>(bb => { if (bb.Publisher.Id == -1) bb.Publisher = null; });
            }
        }

        public BookDTO GetBook(int bookId)
        {
            using (var dataContext = GetDataContext())
            {
                var book = dataContext.Books.FirstOrDefault(b => b.Id == bookId);

                if (book == null)
                    throw new ArgumentOutOfRangeException("Nie znaleziono książki o podanym ID.", (Exception)null);
                else
                {
                    var result = Mapper.Map<BookDTO>(book);
                    result.Available = dataContext.LentBooks.Where(lb => lb.ReturnDate == null && lb.BookId == result.Id).Count() < result.Quantity;
                    return result;
                }
            }
        }

        public void AddBook(BookDTO book)
        {
            if (book == null)
                throw new ArgumentNullException("Serwis bazodanowy otrzymał pustą informacje o książce.", (Exception)null);

            using (var dataContext = GetDataContext())
            {
                Book newBook = Mapper.Map<Book>(book);

                //copy authors
                List<AuthorDTO> authorsDTO = book.Authors.ToList();
                newBook.Authors = new List<Author>();
                foreach (var author in authorsDTO)
                {
                    Author a = dataContext.Authors.FirstOrDefault(aa => aa.Id == author.Id);
                    if (a != null)
                        newBook.Authors.Add(a);
                    else
                        newBook.Authors.Add(new Author() { Name = author.Name });
                }

                //copy bookCategories
                List<BookCategoryDTO> bookCategoriesDTO = book.BookCategories.ToList();
                newBook.BookCategories = new List<BookCategory>();
                foreach (var bookCat in bookCategoriesDTO)
                {
                    BookCategory b = dataContext.BookCategories.FirstOrDefault(bc => bc.Id == bookCat.Id);
                    if (b != null)
                        newBook.BookCategories.Add(b);
                    else
                        newBook.BookCategories.Add(new BookCategory() { Name = bookCat.Name });
                }

                dataContext.Books.Add(newBook);
                dataContext.SaveChanges();
            }
        }

        public void EditBook(BookDTO book)
        {
            if (book == null)
                throw new ArgumentNullException("Serwis bazodanowy otrzymał pustą informacje o książce.", (Exception)null);

            using (var dataContext = GetDataContext())
            {
                Book toEdit = dataContext.Books.FirstOrDefault(b => b.Id == book.Id);
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
                toEdit.Title = toEdit.Title;

                #region updating authors

                var newListOfAuthors = book.Authors.ToList();
                foreach (var oldAuthor in toEdit.Authors.ToList())
                {
                    AuthorDTO newAuthor = newListOfAuthors.FirstOrDefault(a => a.Id == oldAuthor.Id);

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
                    BookCategoryDTO newCat = newListOfBookCategories.FirstOrDefault(bc => bc.Id == oldCat.Id);

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
                Book toDel = dataContext.Books.FirstOrDefault(b => b.Id == bookId);
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

        public IList<PublisherDTO> GetAllPublishers()
        {
            using (var dataContext = GetDataContext())
            {
                return Mapper.Map<List<PublisherDTO>>(dataContext.Publishers);
            }
        }

        public int AddPublisher(PublisherDTO publisher)
        {
            if (publisher == null)
                throw new ArgumentNullException("Serwis bazodanowy otrzymał pustą informacje o wydawcy.", (Exception)null);

            using (var dataContext = GetDataContext())
            {
                Publisher newPub = new Publisher() { Name = publisher.Name };
                dataContext.Publishers.Add(newPub);
                dataContext.SaveChanges();

                return newPub.Id;
            }
        }

        #endregion

        #region BookCategories

        public IList<BookCategoryDTO> GetAllBookCategories()
        {
            using (var dataContext = GetDataContext())
            {
                return Mapper.Map<List<BookCategoryDTO>>(dataContext.BookCategories);
            }
        }

        public int AddBookCategory(BookCategoryDTO bookCategory)
        {
            if (bookCategory == null)
                throw new ArgumentNullException("Serwis bazodanowy otrzymał pustą informacje o kategorii.", (Exception)null);

            using (var dataContext = GetDataContext())
            {
                BookCategory newBookCat = new BookCategory() { Name = bookCategory.Name };
                dataContext.BookCategories.Add(newBookCat);
                dataContext.SaveChanges();

                return newBookCat.Id;
            }
        }

        #endregion

        #region Authors

        public IList<AuthorDTO> GetAllAuthors()
        {
            using (var dataContext = GetDataContext())
            {
                return Mapper.Map<List<AuthorDTO>>(dataContext.Authors);
            }
        }

        public int AddAuthor(AuthorDTO author)
        {
            if (author == null)
                throw new ArgumentNullException("Serwis bazodanowy otrzymał pustą informacje o autorze.", (Exception)null);

            using (var dataContext = GetDataContext())
            {
                Author newAuthor = new Author() { Name = author.Name };
                dataContext.Authors.Add(newAuthor);
                dataContext.SaveChanges();

                return newAuthor.Id;
            }
        }

        #endregion

        #region Authorization

        public void TestAuthorization(string username, string password)
        {
            using (var dataContext = GetDataContext())
            {
                var user = dataContext.Persons.OfType<Employee>().First(x => x.Username == username);
                if (!BCryptHelper.CheckPassword(password, user.Password))
                {
                    throw new InvalidOperationException("Password is incorrect!");
                }
            }
        }

        #endregion
    }
}
