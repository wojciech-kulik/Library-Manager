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
        IEntitySet<Client> Clients { get; }

        IEmployeeEntitySet Employees { get; }

        IEntitySet<Author> Authors { get; }

        IEntitySet<Publisher> Publishers { get; }

        IEntitySet<BookCategory> BookCategories { get; }

        ILendingEntitySet Lendings { get; }

        IEntitySet<Book> Books { get; }

        #region Authentication

        bool IsFirstLogIn();

        bool Authenticate(string username, string password);

        #endregion
    }
}
