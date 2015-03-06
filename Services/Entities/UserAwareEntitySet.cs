using Common.Exceptions;
using DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Entities
{
    public class UserAwareEntitySet<TModel, TDBModel> : EntitySet<TModel, TDBModel>
        where TModel : class, Model.IIdRecord
        where TDBModel : class, DB.IIdRecord
    {
        protected readonly string _username;

        public UserAwareEntitySet(string connectionString, string username)
            : base(connectionString)
        {
            _username = username;
        }

        protected DB.Employee GetCurrentEmployee(LibraryDataContext dataContext)
        {
            DB.Employee emp = dataContext.Persons.OfType<DB.Employee>().FirstOrDefault(e => e.Username == _username);

            if (emp == null)
                throw new AccessException();
            else
                return emp;
        }
    }
}
