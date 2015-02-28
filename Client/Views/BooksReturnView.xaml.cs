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
    /// Interaction logic for BooksReturnView.xaml
    /// </summary>
    public partial class BooksReturnView : Window
    {
        public BooksReturnView()
        {
            InitializeComponent();
        }

        private void OnKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Return)
                btnSave.RaiseEvent(new RoutedEventArgs(ActionButton.ClickEvent, sender));
            else if (e.Key == Key.Escape)
                btnCancel.RaiseEvent(new RoutedEventArgs(ActionButton.ClickEvent, sender));
        }
    }
}
