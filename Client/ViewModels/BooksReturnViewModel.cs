using Caliburn.Micro;
using Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace ClientApplication.ViewModels
{
    public class BooksReturnViewModel : BaseViewModel
    {
        public BooksReturnViewModel(INavigationService navigationService, IWindowManager windowManager, IDBServiceManager<IDatabaseService> dbServiceManager)
            : base(navigationService, windowManager, dbServiceManager)
        {
        }

        #region bindable properties

        #region LentBooks

        private BindableCollection<LentBookDTO> _lentBooks;

        public BindableCollection<LentBookDTO> LentBooks
        {
            get
            {
                return _lentBooks;
            }
            set
            {
                if (_lentBooks != value)
                {
                    _lentBooks = value;
                    NotifyOfPropertyChange(() => LentBooks);
                }
            }
        }
        #endregion

        #region SelectedLentBook

        private LentBookDTO _selectedLentBook;

        public LentBookDTO SelectedLentBook
        {
            get
            {
                return _selectedLentBook;
            }
            set
            {
                if (_selectedLentBook != value)
                {
                    _selectedLentBook = value;
                    NotifyOfPropertyChange(() => SelectedLentBook);
                }
            }
        }
        #endregion

        #endregion

        #region operations

        public void Save()
        {
            using (var dbService = _dbServiceManager.GetService())
            {
                //update only that lendings, which have changed
                Dictionary<int, bool> ids = new Dictionary<int,bool>();
                LentBooks.Where(b => b.IsReturnedChanged).ForEach(l => ids.Add(l.Id, l.IsReturned));

                if (ids.Count == 0)
                    TryClose(false);
                else
                {
                    dbService.ReturnBooks(ids, LentBooks[0].LendingId);
                    TryClose(true);
                }
            }
        }

        public void SelectDeselectAll()
        {
            bool allSelected = LentBooks.All(b => b.IsReturned);
            LentBooks.ForEach(lb => lb.IsReturned = !allSelected);
            RefreshCheckboxes();            
        }

        public void ReverseSelection()
        {
            if (SelectedLentBook != null)
            {
                SelectedLentBook.IsReturned = !SelectedLentBook.IsReturned;
                RefreshCheckboxes();
            }
        }

        private void RefreshCheckboxes()
        {
            LentBookDTO selection = SelectedLentBook;
            BindableCollection<LentBookDTO> tmp = LentBooks;

            LentBooks = null;
            LentBooks = tmp;
            SelectedLentBook = selection;
        }

        #endregion
    }
}
