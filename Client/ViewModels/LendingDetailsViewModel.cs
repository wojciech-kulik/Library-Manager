using Caliburn.Micro;
using ClientApplication.Utilities;
using Common;
using Model;
using System;
using System.Linq;
using System.Windows;

namespace ClientApplication.ViewModels
{
    public class LendingDetailsViewModel : BaseViewModel
    {
        public LendingDetailsViewModel(INavigationService navigationService, IWindowManager windowManager, IDBServiceManager<IDatabaseService> dbServiceManager)
            : base(navigationService, windowManager, dbServiceManager)
        {
        }

        #region navigation properties

        public bool IsEditing { get; set; }

        #endregion

        #region bindable properties

        #region Lending

        private Lending _lending;

        public Lending Lending
        {
            get
            {
                return _lending;
            }
            set
            {
                if (_lending != value)
                {
                    _lending = value;
                    NotifyOfPropertyChange(() => Lending);
                }
            }
        }
        #endregion

        #region SelectedBook

        private LentBook _selectedBook;

        public LentBook SelectedBook
        {
            get
            {
                return _selectedBook;
            }
            set
            {
                if (_selectedBook != value)
                {
                    _selectedBook = value;
                    NotifyOfPropertyChange(() => SelectedBook);
                }
            }
        }
        #endregion

        #endregion

        #region operations

        public void Save()
        {
            if (Lending.Books.Count == 0)
            {
                MessageBox.Show(App.GetString("BooksAreRequired"), App.GetString("FillRequiredFields"), MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            foreach (var lentBook in Lending.Books)
                lentBook.Book = null;

            if (IsEditing)
            {
                using (var dbService = _dbServiceManager.GetService())
                {
                    dbService.EditLending(Lending);
                }                
            }
            else
            {
                using (var dbService = _dbServiceManager.GetService())
                {
                    dbService.AddLending(Lending);
                }   
            }

            TryClose(true);
        }

        public void DeleteBook()
        {
            var tmp = SelectedBook;
            SelectedBook = null;
            Lending.Books.Remove(tmp);
        }

        public void AddBook()
        {
            _navigationService.GetWindow<BooksListViewModel>()
                .WithParam(vm => vm.BookSelectionMode, true)
                .DoBeforeShow(vm => vm.BookSelectEvent += LendingDetailsViewModel_BookSelectedEvent)
                .ShowWindowModal();
        }

        void LendingDetailsViewModel_BookSelectedEvent(BookSelectedEventArgs<Book> e)
        {
            if (Lending.Books.Any(lb => lb.BookId == e.Book.Id))
            {
                MessageBox.Show(String.Format(App.GetString("BookAlreadyAdded"), e.Book.Title), App.GetString("Error"), MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            Lending.Books.Add(new LentBook() 
            { 
                Book = e.Book, 
                BookId = e.Book.Id, 
                LendingId = this.Lending.Id, 
                EndDate = this.Lending.EndDate 
            });
        }

        #endregion
    }
}
