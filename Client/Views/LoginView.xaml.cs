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
    /// Interaction logic for LoginView.xaml
    /// </summary>
    public partial class LoginView : Window
    {
        public LoginView()
        {
            InitializeComponent();
            Loaded += LoginView_Loaded;
        }

        void LoginView_Loaded(object sender, RoutedEventArgs e)
        {
            tbLogin.Focus();
        }

        private void OnKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Return)
                btnLogin.RaiseEvent(new RoutedEventArgs(ActionButton.ClickEvent, sender));
        }

        private void btnLogin_Click(object sender, RoutedEventArgs e)
        {
            (DataContext as LoginViewModel).Password = pbPassword.Password;
            (DataContext as LoginViewModel).Login();
        }
    }
}
