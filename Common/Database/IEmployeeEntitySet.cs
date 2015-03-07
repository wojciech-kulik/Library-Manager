using Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common
{
    public interface IEmployeeEntitySet : IEntitySet<Model.Employee>
    {
        Role GetEmployeeRole(string username);
    }
}
