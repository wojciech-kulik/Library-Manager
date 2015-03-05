using DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services
{
    public class DBTable
    {
        private string _connectionString;

        public DBTable(string connectionString)
        {
            _connectionString = connectionString;
            CreateDatabaseIfNotExists();
        }

        protected void CreateDatabaseIfNotExists()
        {
            using (var dataContext = new LibraryDataContext(_connectionString))
            {
                dataContext.Database.CreateIfNotExists();
            }
        }

        protected LibraryDataContext GetDataContext()
        {
            return new LibraryDataContext(_connectionString);
        }
    }
}
