using AutoMapper;
using Common.Exceptions;
using DevOne.Security.Cryptography.BCrypt;
using Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Entities
{
    public class EmployeeEntity : DBUserAwareEntity<Model.Employee, DB.Employee>
    {
        public EmployeeEntity(string connectionString, string username)
            : base(connectionString, username)
        {
        }

        public override IList<Model.Employee> GetAll()
        {
            return base.GetAll().Where(x => !x.Removed).ToList();
        }

        public override void Delete(int id)
        {
            using (var dataContext = GetDataContext())
            {
                DB.Employee employee = dataContext.Persons.OfType<DB.Employee>().FirstOrDefault(x => x.Id == id);

                if (employee.Username == GetCurrentEmployee(dataContext).Username)
                    throw new RemoveYourselfException();
                if (employee == null)
                    throw new RecordNotFoundException();

                if (employee.Lendings.Count() > 0 || employee.Returns.Count() > 0)
                    employee.Removed = true;
                else
                    dataContext.Persons.Remove(employee);

                dataContext.SaveChanges();
            }
        }

        public override void Add(Model.Employee entity)
        {
            if (entity == null)
                throw new ArgumentNullException("entity");

            using (var dataContext = GetDataContext())
            {
                entity.Password = BCryptHelper.HashPassword(entity.Password, BCryptHelper.GenerateSalt(10));
                entity.Username = entity.Username.ToLower();

                if (dataContext.Persons.OfType<DB.Employee>().Any(emp => !emp.Removed && emp.Username == entity.Username))
                    throw new UsernameTakenException();

                dataContext.Persons.Add(Mapper.Map<DB.Employee>(entity));
                dataContext.SaveChanges();
            }
        }

        public override void Update(Model.Employee entity)
        {
            if (entity == null)
                throw new ArgumentNullException("entity");

            using (var dataContext = GetDataContext())
            {
                DB.Employee toEdit = dataContext.Persons.OfType<DB.Employee>().FirstOrDefault(x => x.Id == entity.Id);
                if (toEdit == null)
                    throw new RecordNotFoundException();

                string oldPass = toEdit.Password;
                entity.Username = entity.Username.ToLower();

                if (dataContext.Persons.OfType<DB.Employee>().Any(x => !x.Removed && x.Id != entity.Id && x.Username == entity.Username))
                    throw new UsernameTakenException();
                if (entity.Username == GetCurrentEmployee(dataContext).Username && (Role)entity.Role != Role.Admin)
                    throw new RemoveYourselfException();

                Mapper.Map<Model.Employee, DB.Employee>(entity, toEdit);

                if (!String.IsNullOrWhiteSpace(toEdit.Password))
                    toEdit.Password = BCryptHelper.HashPassword(toEdit.Password, BCryptHelper.GenerateSalt(10));
                else
                    toEdit.Password = oldPass;

                dataContext.SaveChanges();
            }
        }
    }
}
