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
            Books = new BookEntitySet(connectionString);

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

        public IEntitySet<Model.Book> Books { get; private set; }
        
        public IEntitySet<Model.Author> Authors { get; private set; }

        public IEntitySet<Model.Publisher> Publishers { get; private set; }

        public IEntitySet<Model.BookCategory> BookCategories { get; private set; }


        #region Authentication

        private LibraryDataContext GetDataContext()
        {
            var dataContext = new LibraryDataContext(_connectionString);
            dataContext.Database.CreateIfNotExists();

            return dataContext;
        }

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
