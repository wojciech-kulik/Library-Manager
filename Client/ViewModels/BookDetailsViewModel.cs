using Caliburn.Micro;
using Common;
using Model;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using Helpers;

namespace ClientApplication.ViewModels
{
    public class BookDetailsViewModel : BaseViewModel        
    {
        public BookDetailsViewModel(INavigationService navigationService, IWindowManager windowManager, IDBServiceManager<IDatabaseService> dbServiceManager)
            : base(navigationService, windowManager, dbServiceManager)
        {
            Book = new Book() { BookCategories = new ObservableCollection<BookCategory>(), Authors = new ObservableCollection<Author>() };
            RefreshCategoriesAuthorsAndPublishers();
        }

        #region navigation properties

        public bool IsEditing { get; set; }

        #endregion

        #region bindable properties

        #region Book

        private Book _book;

        public Book Book
        {
            get
            {
                return _book;
            }
            set
            {
                if (_book != value)
                {
                    _book = value;
                    if (Publishers != null)
                        SelectedPublisher = Publishers.FirstOrDefault(p => p.Id == _book.PublisherId);
                    NotifyOfPropertyChange(() => Book);
                }
            }
        }
        #endregion

        #region BookCategories

        private BindableCollection<BookCategory> _bookCategories;

        public BindableCollection<BookCategory> BookCategories
        {
            get
            {
                return _bookCategories;
            }
            set
            {
                if (_bookCategories != value)
                {
                    _bookCategories = value;
                    NotifyOfPropertyChange(() => BookCategories);
                }
            }
        }
        #endregion


        #region Authors

        private BindableCollection<Author> _authors;

        public BindableCollection<Author> Authors
        {
            get
            {
                return _authors;
            }
            set
            {
                if (_authors != value)
                {
                    _authors = value;
                    NotifyOfPropertyChange(() => Authors);
                }
            }
        }
        #endregion

        #region Publishers

        private BindableCollection<Publisher> _publishers;

        public BindableCollection<Publisher> Publishers
        {
            get
            {
                return _publishers;
            }
            set
            {
                if (_publishers != value)
                {
                    _publishers = value;
                    NotifyOfPropertyChange(() => Publishers);
                }
            }
        }
        #endregion


        #region SelectedBookCategory

        private BookCategory _selectedBookCategory;

        public BookCategory SelectedBookCategory
        {
            get
            {
                return _selectedBookCategory;
            }
            set
            {
                if (_selectedBookCategory != value)
                {
                    _selectedBookCategory = value;
                    if (_selectedBookCategory != null)
                        AddBookCategory();
                    NotifyOfPropertyChange(() => SelectedBookCategory);
                }
            }
        }
        #endregion

        #region SelectedAuthor

        private Author _selectedAuthor;

        public Author SelectedAuthor
        {
            get
            {
                return _selectedAuthor;
            }
            set
            {
                if (_selectedAuthor != value)
                {
                    _selectedAuthor = value;
                    if (_selectedAuthor != null)
                        AddAuthor();
                    NotifyOfPropertyChange(() => SelectedAuthor);
                }
            }
        }
        #endregion

        #region SelectedPublisher

        private Publisher _selectedPublisher;

        public Publisher SelectedPublisher
        {
            get
            {
                return _selectedPublisher;
            }
            set
            {
                if (_selectedPublisher != value)
                {
                    _selectedPublisher = value;
                    Book.Publisher = _selectedPublisher;
                    if (_selectedPublisher != null)
                        Book.PublisherId = _selectedPublisher.Id;
                    else
                        Book.PublisherId = null;
                    NotifyOfPropertyChange(() => SelectedPublisher);
                }
            }
        }
        #endregion


        #region SelectedListboxAuthor

        private Author _selectedListboxAuthor;

        public Author SelectedListboxAuthor
        {
            get
            {
                return _selectedListboxAuthor;
            }
            set
            {
                if (_selectedListboxAuthor != value)
                {
                    _selectedListboxAuthor = value;
                    NotifyOfPropertyChange(() => SelectedListboxAuthor);
                }
            }
        }
        #endregion

        #region SelectedListboxBookCategory

        private BookCategory _selectedListboxBookCategory;

        public BookCategory SelectedListboxBookCategory
        {
            get
            {
                return _selectedListboxBookCategory;
            }
            set
            {
                if (_selectedListboxBookCategory != value)
                {
                    _selectedListboxBookCategory = value;
                    NotifyOfPropertyChange(() => SelectedListboxBookCategory);
                }
            }
        }
        #endregion

        #endregion

        #region operation

        #region Publisher, Authors, BookCategories

        public void RefreshCategoriesAuthorsAndPublishers()
        {
            using (var dbService = _dbServiceManager.GetService())
            {
                BookCategories = new BindableCollection<BookCategory>(dbService.GetAllBookCategories());
                Authors = new BindableCollection<Author>(dbService.GetAllAuthors());
                Publishers = new BindableCollection<Publisher>(dbService.GetAllPublishers());
            }
        }

        public void AddBookCategory()
        {
            if (!Book.BookCategories.Any(bc => bc.Id == SelectedBookCategory.Id))
                Book.BookCategories.Add(SelectedBookCategory);
        }

        public void AddAuthor()
        {
            if (!Book.Authors.Any(a => a.Id == SelectedAuthor.Id))
                Book.Authors.Add(SelectedAuthor);
        }

        public void DeleteAuthor(ActionExecutionContext context)
        {
            if ((context.EventArgs as KeyEventArgs).Key == Key.Delete && SelectedListboxAuthor != null)
                Book.Authors.RemoveFirst(a => a.Id == SelectedListboxAuthor.Id);
        }

        public void DeleteBookCategory(ActionExecutionContext context)
        {
            if ((context.EventArgs as KeyEventArgs).Key == Key.Delete && SelectedListboxBookCategory != null)
                Book.BookCategories.RemoveFirst(bc => bc.Id == SelectedListboxBookCategory.Id);
        }

        public void AddNewPublisher(string publisherName)
        {
            Publisher publisher = Publishers.FirstOrDefault(p => p.Name.ToLower() == publisherName.ToLower());
            if (publisher != null)
            {
                SelectedPublisher = publisher;
                return;
            }

            if (MessageBox.Show(String.Format(App.GetString("AreYouSureAddPublisher"), publisherName), App.GetString("NewPublisher"),
                                MessageBoxButton.YesNo, MessageBoxImage.Question, MessageBoxResult.Yes) == MessageBoxResult.Yes)
            {
                using (var dbService = _dbServiceManager.GetService())
                {
                    Publisher newPub = new Publisher() { Name = publisherName };
                    newPub.Id = dbService.AddPublisher(newPub);

                    Publishers.Add(newPub);
                    SelectedPublisher = newPub;
                }
            }
            else
                SelectedPublisher = null;
        }

        public void AddNewPublisher(ActionExecutionContext context, string publisherName)
        {
            if ((context.EventArgs as KeyEventArgs).Key == Key.Return && !String.IsNullOrWhiteSpace(publisherName))
            {
                (context.EventArgs as KeyEventArgs).Handled = true;
                AddNewPublisher(publisherName);
            }
        }

        public void AddNewAuthor(ActionExecutionContext context, string authorName)
        {
            if ((context.EventArgs as KeyEventArgs).Key == Key.Return && !String.IsNullOrWhiteSpace(authorName))
            {
                (context.EventArgs as KeyEventArgs).Handled = true;

                Author author = Authors.FirstOrDefault(p => p.Name.ToLower() == authorName.ToLower());
                if (author != null)
                {
                    SelectedAuthor = author;
                    return;
                }

                if (MessageBox.Show(String.Format(App.GetString("AreYouSureAddAuthor"), authorName), App.GetString("NewAuthor"),
                                    MessageBoxButton.YesNo, MessageBoxImage.Question, MessageBoxResult.Yes) == MessageBoxResult.Yes)
                {
                    using (var dbService = _dbServiceManager.GetService())
                    {
                        Author newAuthor = new Author() { Name = authorName };
                        newAuthor.Id = dbService.AddAuthor(newAuthor);

                        Authors.Add(newAuthor);
                        SelectedAuthor = newAuthor;
                    }
                }
                else
                    SelectedAuthor = null;
            }
        }

        public void AddNewBookCategory(ActionExecutionContext context, string bookCategoryName)
        {
            if ((context.EventArgs as KeyEventArgs).Key == Key.Return && !String.IsNullOrWhiteSpace(bookCategoryName))
            {
                (context.EventArgs as KeyEventArgs).Handled = true;

                BookCategory bookCat = BookCategories.FirstOrDefault(p => p.Name.ToLower() == bookCategoryName.ToLower());
                if (bookCat != null)
                {
                    SelectedBookCategory = bookCat;
                    return;
                }

                if (MessageBox.Show(String.Format(App.GetString("AreYouSureAddCategory"), bookCategoryName), App.GetString("NewCategory"),
                                    MessageBoxButton.YesNo, MessageBoxImage.Question, MessageBoxResult.Yes) == MessageBoxResult.Yes)
                {
                    using (var dbService = _dbServiceManager.GetService())
                    {
                        BookCategory newBookCat = new BookCategory() { Name = bookCategoryName };
                        newBookCat.Id = dbService.AddBookCategory(newBookCat);

                        BookCategories.Add(newBookCat);
                        SelectedBookCategory = newBookCat;
                    }
                }
                else
                    SelectedBookCategory = null;
            }
        }

        #endregion

        #region saving

        private void UpdateBook()
        {
            using (var dbService = _dbServiceManager.GetService())
            {
                dbService.EditBook(Book);
            }
            TryClose(true);
        }

        private void AddBook()
        {
            using (var dbService = _dbServiceManager.GetService())
            {
                dbService.AddBook(Book);
            }
            TryClose(true);
        }

        public void Save()
        {
            if (String.IsNullOrWhiteSpace(Book.Title))
            {
                MessageBox.Show(App.GetString("TitleIsRequired"), App.GetString("FillRequiredFields"), MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            Book.Publisher = null;

            if (IsEditing)
                UpdateBook();
            else
                AddBook();
        }

        #endregion

        #endregion

        //todo usuwanie kategorii itd
    }
}
