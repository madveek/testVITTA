using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using testVITTA.MVVM.Model;
using testVITTA.MVVM.ViewModel;

namespace testVITTA
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void IncomeList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            
            var listView = sender as ListView;
            if (listView != null && DataContext is MainViewModel viewModel)
            {
                viewModel.SelectedIncomes.Clear();
                foreach (Income income in listView.SelectedItems)
                {
                    viewModel.SelectedIncomes.Add(income);
                    
                }
                viewModel.CalculatePayments();
                
            }
        }

        private void OrderList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            IncomeList.UnselectAll();
        }
    }
}