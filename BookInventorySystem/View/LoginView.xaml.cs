using BookInventorySystem.Model;
using BookInventorySystem.ViewModel;
using System.Windows;
using DataAccessLayer;
using BILogger;

namespace BookInventorySystem.View
{
    /// <summary>
    /// Interaction logic for LoginView.xaml
    /// </summary>
    public partial class LoginView : Window
    {
        public LoginView()
        {
            InitializeComponent();
            this.DataContext = new LoginViewModel(new Logger(),new DataAccess<AdminModel,AdminModel>());
            
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            App.pswd = Pswd.Password;
            //this.Close();
        }
    }
}
