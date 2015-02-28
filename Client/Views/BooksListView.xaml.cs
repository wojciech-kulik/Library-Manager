using ClientApplication.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace ClientApplication.Views
{
    /// <summary>
    /// Interaction logic for BooksListView.xaml
    /// </summary>
    public partial class BooksListView : Window
    {
        public BooksListView()
        {
            InitializeComponent();
            Loaded += BooksListView_Loaded;
        }

        void BooksListView_Loaded(object sender, RoutedEventArgs e)
        {
            tbSearchPhrase.Focus();
            tbSearchPhrase.CaretIndex = tbSearchPhrase.Text.Length;
        }

        private void OnKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
                btnCancel.RaiseEvent(new RoutedEventArgs(ActionButton.ClickEvent, sender));
        }
    }
}
