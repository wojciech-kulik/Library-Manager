using ClientApplication.Controls;
using Common;
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
    /// Interaction logic for LendingDetailsView.xaml
    /// </summary>
    public partial class LendingDetailsView : Window
    {
        public LendingDetailsView()
        {
            InitializeComponent();
        }

        private void OnKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
                btnCancel.RaiseEvent(new RoutedEventArgs(ActionButton.ClickEvent, sender));
        }

        private void DatePicker_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            if (dgLentBooks.Items.Count > 0 && (sender as DatePicker).SelectedDate.HasValue && 
                MessageBox.Show("Czy ustawić ten termin zwrotu dla wszystkich książek?", "Termin zwrotu",
                                MessageBoxButton.YesNo, MessageBoxImage.Question, MessageBoxResult.Yes) == MessageBoxResult.Yes)
            {
                foreach (var lentBook in dgLentBooks.Items)
                    (lentBook as LentBookDTO).EndDate = (sender as DatePicker).SelectedDate.Value;
            }
            
        }
    }
}
