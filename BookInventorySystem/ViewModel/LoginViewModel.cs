using BILogger;
using BookInventorySystem.Model;
using BookInventorySystem.View;
using DataAccessLayer;
using System.Collections.Generic;
using System.Configuration;
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

        private string _errorMsg;

        public string ErrorMsg
        {
            get { return _errorMsg; }

            set
            {
                _errorMsg = value;
                OnPropertyChange();
            }
        }


        const string DbName = "SampleDB";
        public ICommand Login { get; set; }

        private List<AdminModel> AdminList;
        
        private IDataAccess<AdminModel, AdminModel> _dataAccess;

        private ILogger _log;
        public LoginViewModel(ILogger Log,IDataAccess<AdminModel, AdminModel> DataAccess)
        {
            Connection.InitiaizeDataAccessLayer(ConfigurationManager.ConnectionStrings[DbName].ConnectionString);
            _log = Log;
            _dataAccess = DataAccess;
            Login = new ApplicationCommand<object>(LoginValidation);
            AdminList = new List<AdminModel>();
            ErrorMsgVisibility = Visibility.Hidden;
        }

        private async void LoginValidation(object parameter)
        {
            try
            {
                ErrorMsgVisibility = Visibility.Hidden;
                var win = parameter as Window;
                await AuthenticateUser();
                if (AdminList.Count == 0)
                {
                    ErrorMsg = Properties.Resources.AuthenticationFailedMsg;
                    ErrorMsgVisibility = Visibility.Visible;
                }

                else
                {
                    MainWindowView _mainWindow = new MainWindowView(_log);
                    _mainWindow.Show();
                    if (win != null)
                        win.Close();
                    _log.Message("User Authenticated");
                }
            }
            catch (System.Data.SqlClient.SqlException ex)
            {
                _log.Error(ex);
                _log.Message("Unable to connect to database");
                ErrorMsg = Properties.Resources.DatabaseConnFailedMsg;
                ErrorMsgVisibility = Visibility.Visible;
            }
        }


        private async Task AuthenticateUser()
        {
            AdminList.Clear();

            var _admin = new AdminModel()
            {
                UserName = this.UserName,
                Password = App.pswd
            };

            Task<List<AdminModel>> task = Task.Run<List<AdminModel>>(() => {
                var t = _dataAccess.GetAllData(Properties.Resources.GetAdminList, _admin);
                return t;
            });
            var adminCollection = await task;
            adminCollection.ForEach(t => AdminList.Add(t));
        }
    }
}
