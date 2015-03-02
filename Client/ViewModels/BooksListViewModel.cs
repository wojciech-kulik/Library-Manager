using AutoMapper;
using Caliburn.Micro;
using Common;
using Model;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using Helpers;
using ClientApplication.Utilities;

namespace ClientApplication.ViewModels
{
    public class BooksListViewModel : BaseViewModel
    {
        public BooksListViewModel(INavigationService navigationService, IWindowManager windowManager, IDBServiceManager<IDatabaseService> dbServiceManager)
            : base(navigationService, windowManager, dbServiceManager)
        {
            RefreshAllBooks();
        }

        public delegate void BookSelectedEventHandler(BookSelectedEventArgs<BookDTO> e);

        public event BookSelectedEventHandler BookSelectEvent;

        #region navigation properties

        //if true then double click cause BookSelectEvent and closes window
        public bool BookSelectionMode { get; set; }

        #endregion

        #region bindable properties

        #region AllBooks

        private BindableCollection<BookDTO> _allBooks;

        public BindableCollection<BookDTO> AllBooks
        {
            get
            {
                return _allBooks;
            }
            set
            {
                if (_allBooks != value)
                {
                    _allBooks = value;
                    NotifyOfPropertyChange(() => AllBooks);
                }
            }
        }
        #endregion

        #region Books

        private BindableCollection<BookDTO> _books;

        public BindableCollection<BookDTO> Books
        {
            get
            {
                return _books;
            }
            set
            {
                if (_books != value)
                {
                    _books = value;
                    NotifyOfPropertyChange(() => Books);
                }
            }
        }
        #endregion

        #region SelectedBook

        private BookDTO _selectedBook;

        public BookDTO SelectedBook
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

        #region SearchPhrase

        private string _searchPhrase;

        public string SearchPhrase
        {
            get
            {
                return _searchPhrase;
            }
            set
            {
                if (_searchPhrase != value)
                {
                    _searchPhrase = value;
                    NotifyOfPropertyChange(() => SearchPhrase);
                }
            }
        }
        #endregion

        #endregion

        #region operations

        public void BookSelectCompleted()
        {
            if (BookSelectionMode)
            {
                if (BookSelectEvent != null)
                    BookSelectEvent(new BookSelectedEventArgs<BookDTO>() { Book = Mapper.Map<BookDTO>(SelectedBook) });
            }

            TryClose(true);
        }

        public void RefreshAllBooks()
        {
            using (var dbService = _dbServiceManager.GetService())
            {
                AllBooks = new BindableCollection<BookDTO>(dbService.GetAllBooks());
                Books = new BindableCollection<BookDTO>(AllBooks);
            }
        }

        public void RefreshSelectedBook()
        {
            int id = SelectedBook != null ? SelectedBook.Id : -1;
            SelectedBook = null;

            using (var dbService = _dbServiceManager.GetService())
            {
                BookDTO newBook = dbService.GetBook(id);

                for (int i = 0; i < AllBooks.Count; i++)
                    if (AllBooks[i].Id == id)
                    {
                        AllBooks[i] = newBook;
                        break;
                    }

                for (int i = 0; i < Books.Count; i++)
                    if (Books[i].Id == id)
                    {
                        Books[i] = newBook;
                        SelectedBook = Books[i];
                        break;
                    }
            }
        }

        public void SearchBook(ActionExecutionContext context, string phrase)
        {
            SearchPhrase = phrase;

            var keyArgs = context.EventArgs as KeyEventArgs;

            if (keyArgs != null && keyArgs.Key == Key.Enter)
            {
                if (String.IsNullOrEmpty(phrase))
                    Books = new BindableCollection<BookDTO>(AllBooks);
                else
                    Books = new BindableCollection<BookDTO>(AllBooks.Where(c => c.Title.ContainsAny(phrase) || (!String.IsNullOrEmpty(c.AuthorsString) && c.AuthorsString.ContainsAny(phrase))));
            }
        }

        public void AddBook()
        {
            bool result = _navigationService.GetWindow<BookDetailsViewModel>().ShowWindowModal();

            if (result)
            {
                //Adding a new books to list
                BookDTO newBook = null;
                using (var dbService = _dbServiceManager.GetService())
                {
                    var books = dbService.GetAllBooks();

                    foreach (var b in books.OrderByDescending(cli => cli.Id))
                    {
                        if (AllBooks.Any(book => book.Id == b.Id))
                            break;
                        else
                        {
                            if (newBook == null)
                                newBook = b;
                            AllBooks.Add(b);
                        }
                    }
                }

                Books = new BindableCollection<BookDTO>(AllBooks);
                SelectedBook = newBook;
            }
        }

        public void EditBook()
        {
            _navigationService.GetWindow<BookDetailsViewModel>()
                .WithParam(vm => vm.IsEditing, true)
                .WithParam(vm => vm.Book, Mapper.Map<BookDTO>(SelectedBook))
                .DoIfSuccess(() => RefreshSelectedBook())
                .ShowWindowModal();
        }

        public void DeleteBook()
        {
            if (MessageBox.Show(String.Format(App.GetString("AreYouSureRemoveBook"), SelectedBook.Title), App.GetString("Removing"),
                                MessageBoxButton.YesNoCancel, MessageBoxImage.Warning, MessageBoxResult.Cancel) != MessageBoxResult.Yes)
            {
                return;
            }

            using (var dbService = _dbServiceManager.GetService())
            {
                dbService.DeleteBook(SelectedBook.Id);
                AllBooks.Remove(SelectedBook);
                Books.Remove(SelectedBook);                
            }
        }


        public void DoubleClick()
        {
            if (BookSelectionMode)
                BookSelectCompleted();
            else
                EditBook();
        }

        #endregion
    }
}
