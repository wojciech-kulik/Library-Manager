using ClientApplication.Controls;
using ClientApplication.ViewModels;
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
    /// Interaction logic for UserDetailsView.xaml
    /// </summary>
    public partial class UserDetailsView : Window
    {
        public UserDetailsView()
        {
            InitializeComponent();
            Loaded += ClientDetailsView_Loaded;
        }

        void ClientDetailsView_Loaded(object sender, RoutedEventArgs e)
        {
            tbFirstName.Focus();
        }

        private void OnKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Return)
                btnSave.RaiseEvent(new RoutedEventArgs(ActionButton.ClickEvent, sender));
            else if (e.Key == Key.Escape)
                btnCancel.RaiseEvent(new RoutedEventArgs(ActionButton.ClickEvent, sender));
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            (DataContext as UserDetailsViewModel).User.Password = pbPassword.Password;
            (DataContext as UserDetailsViewModel).Save();
        }
    }
}
