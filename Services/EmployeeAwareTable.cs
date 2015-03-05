using DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services
{
    public class EmployeeAwareTable : DBTable
    {
        private string _username;

        public EmployeeAwareTable(string connectionString, string username)
            : base(connectionString)
        {
            _username = username;
        }

        public DB.Employee GetCurrentEmployee(LibraryDataContext dataContext)
        {
            DB.Employee emp = dataContext.Persons.OfType<DB.Employee>().FirstOrDefault(e => e.Username == _username);

            if (emp == null)
                throw new Exception(String.Format("Employee '{0}' not found!", _username));
            else
                return emp;
        }
    }
}
