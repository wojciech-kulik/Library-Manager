using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Security.Permissions;
using System.Text;
using Model;

namespace Common
{
    public interface IDatabaseService : IDisposable
    {
        IEntity<Client> Clients { get; }

        IEntity<Employee> Employees { get; }

        Role GetEmployeeRole(string username);

        #region Lendings
        IList<Lending> GetLendingsOf(int clientId);

        IList<LentBook> GetLentBooksOf(int lendingId);

        void ReturnAllBooks(int lendingId);

        void ReturnBooks(Dictionary<int, bool> bookIds, int lendingId);

        void AddLending(Lending lending);

        void EditLending(Lending lending);

        void DeleteLending(int clientId, int lendingId);
        #endregion

        #region Books

        IList<Book> GetAllBooks();

        Book GetBook(int bookId);

        void AddBook(Book book);

        void EditBook(Book book);

        void DeleteBook(int bookId);

        IList<BookCategory> GetAllBookCategories();

        IList<Author> GetAllAuthors();

        IList<Publisher> GetAllPublishers();

        #endregion

        #region Publisher

        int AddPublisher(Publisher publisher);

        #endregion

        #region Authors

        int AddAuthor(Author author);

        #endregion

        #region BookCategories

        int AddBookCategory(BookCategory bookCategory);

        #endregion

        #region Authentication

        bool IsFirstLogIn();

        bool Authenticate(string username, string password);

        #endregion
    }
}
