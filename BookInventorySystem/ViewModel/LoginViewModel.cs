using BookInventorySystem.Model;
using BookInventorySystem.View;
using DataAccessLayer;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace BookInventorySystem.ViewModel
{
    public class LoginViewModel : ViewModelBase
    {
        private string _userName;

        public string UserName
        {
            get { return _userName; }

            set
            {
                _userName = value;
                OnPropertyChange();
            }
        }

        private Visibility _errorMsgVisibility;

        public Visibility ErrorMsgVisibility
        {
            get { return _errorMsgVisibility; }

            set
            {
                _errorMsgVisibility = value;
                OnPropertyChange();
            }
        }
        const string DbName = "SampleDB";
        public ICommand Login { get; set; }

        private List<AdminModel> AdminList;
        public LoginViewModel()
        {
            Connection.InitiaizeDataAccessLayer(ConfigurationManager.ConnectionStrings[DbName].ConnectionString);
            Login = new ApplicationCommand<object>(LoginValidation);
            AdminList = new List<AdminModel>();
            ErrorMsgVisibility = Visibility.Hidden;
        }

        private async void LoginValidation(object parameter)
        {
            ErrorMsgVisibility = Visibility.Hidden;
            var win = parameter as Window;
            await GetBooks();
            if(AdminList.Count==0)
                ErrorMsgVisibility = Visibility.Visible;
            else
            {
                MainWindowView _mainWindow = new MainWindowView();
                _mainWindow.Show();
                if(win!=null)
                    win.Close();
            }
        }


        private async Task GetBooks()
        {
            AdminList.Clear();

            var _admin = new AdminModel()
            {
                UserName = this.UserName,
                Password = App.pswd
            };

            Task<List<AdminModel>> task = Task.Run<List<AdminModel>>(() => {
                var t = DataAccess<AdminModel,AdminModel>.GetAllData(Properties.Resources.GetAdminList,_admin);
                return t;
            });
            var adminCollection = await task;
            adminCollection.ForEach(t => AdminList.Add(t));
        }
    }
}
