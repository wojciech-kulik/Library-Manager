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
    public class ClientDTO : PersonDTO, INotifyPropertyChanged
    {
        public string CardNumber { get; set; }

        public string AdditionalInfo { get; set; }

        #region Lendings

        private ICollection<LendingDTO> _lendings;

        public ICollection<LendingDTO> Lendings
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
                    NotifyPropertyChanged("Lendings");
                }
            }
        }
        #endregion

        #region INotifyPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
        #endregion
    }
}
