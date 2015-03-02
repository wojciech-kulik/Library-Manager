using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model
{
    public partial class EmployeeDTO : PersonDTO
    {
        public string RoleString
        {
            get
            {
                switch (Role)
                {
                    case 0:
                        return "Pracownik";

                    case 1:
                        return "Administrator";
                }

                return "(brak danych)";
            }
        }
    }
}
