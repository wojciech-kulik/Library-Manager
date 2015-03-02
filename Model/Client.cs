using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Model
{
    public class Client : Person
    {
        public string CardNumber { get; set; }

        public string AdditionalInfo { get; set; }

        #region Lendings

        private ICollection<Lending> _lendings;

        public ICollection<Lending> Lendings
        {
            get
            {
                return _lendings;
            }
            set
            {
                if (_lendings != value)
                {
                    _lendings = value;
                    NotifyOfPropertyChange();
                }
            }
        }
        #endregion
    }
}
