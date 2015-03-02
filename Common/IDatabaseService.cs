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
        #region Clients
        IList<ClientDTO> GetAllClients();

        ClientDTO GetClient(int clientId);

        void DeleteClient(int clientId);

        void AddClient(ClientDTO client);

        void EditClient(ClientDTO client);
        #endregion

        #region Employees

        byte GetEmployeeRole(string username);

        IList<EmployeeDTO> GetAllEmployees();

        EmployeeDTO GetEmployee(int employeeId);

        void AddEmployee(EmployeeDTO employee);

        void EditEmployee(EmployeeDTO employee);

        void DeleteEmployee(int employeeId);

        #endregion

        #region Lendings
        IList<LendingDTO> GetLendingsOf(int clientId);

        IList<LentBookDTO> GetLentBooksOf(int lendingId);

        void ReturnAllBooks(int lendingId);

        void ReturnBooks(Dictionary<int, bool> bookIds, int lendingId);

        void AddLending(LendingDTO lending);

        void EditLending(LendingDTO lending);

        void DeleteLending(int clientId, int lendingId);
        #endregion

        #region Books

        IList<BookDTO> GetAllBooks();

        BookDTO GetBook(int bookId);

        void AddBook(BookDTO book);

        void EditBook(BookDTO book);

        void DeleteBook(int bookId);

        IList<BookCategoryDTO> GetAllBookCategories();

        IList<AuthorDTO> GetAllAuthors();

        IList<PublisherDTO> GetAllPublishers();

        #endregion

        #region Publisher

        int AddPublisher(PublisherDTO publisher);

        #endregion

        #region Authors

        int AddAuthor(AuthorDTO author);

        #endregion

        #region BookCategories

        int AddBookCategory(BookCategoryDTO bookCategory);

        #endregion

        #region Authorization

        void TestAuthorization(string username, string password);

        #endregion
    }
}
