using System.Collections.ObjectModel;
using DataAccessLayer;
using System.Configuration;
using System.Windows.Input;
using System.Windows;
using System.Windows.Controls;
using BILogger;
using BookInventorySystem.Model;

namespace BookInventorySystem.ViewModel
{
    public class MainWindowViewModel : ViewModelBase
    {
        public ICommand TabSwitchCommand { get; set; }

        public ICommand CloseWindow { get; set; }

        public ICommand HelpCommand { get; set; }

        private Thickness _gridMargin;

        public Thickness gridmargin
        {
            get { return _gridMargin; }

            set
            {
                _gridMargin = value;
                OnPropertyChange();

            }
        }

        private ObservableCollection<ViewModelBase> _tabItems;

        public ObservableCollection<ViewModelBase> TabItems
        {
            get { return _tabItems; }
            set
            {
                _tabItems = value;
            }
        }

        private ViewModelBase _currentScreen;

        public ViewModelBase CurrentScreen
        {
            get { return _currentScreen; }
            set
            {
                _currentScreen = value;
                OnPropertyChange();
            }
        }

        private BookViewModel _bookViewModel;

        private CustomerViewModel _customerViewModel;

        private CheckOutViewModel _checkOutViewModel;

        const string DbName = "SampleDB";

        private ILogger _log;

        private IDataAccess<BookModel, CustomerHistoryModel> _bookDataAccess;

        private IDataAccess<CustomerModel, CustomerHistoryModel> _customerDataAccess;

        private IDataAccess<CheckOutModel, CheckOutModel> _allPastOrderDataAccess;

        private IDataAccess<CustomerModel, BookModel> _allPastOrderedBookDataAcess;
        public MainWindowViewModel(ILogger Log)
        {
            _log = Log;
            _bookDataAccess = new DataAccess<BookModel, CustomerHistoryModel>();
            _customerDataAccess = new DataAccess<CustomerModel, CustomerHistoryModel>();
            _allPastOrderDataAccess = new DataAccess<CheckOutModel, CheckOutModel>();
            _allPastOrderedBookDataAcess = new DataAccess<CustomerModel, BookModel>();
            //Connection.InitiaizeDataAccessLayer(ConfigurationManager.ConnectionStrings[DbName].ConnectionString);

            InitializeViewModelClass();


            _log.Message("Application started inside mainViewModel");

            //TabItems = new ObservableCollection<ViewModelBase>();
            //TabItems.Add(new BookViewModel());
            //TabItems.Add(new CustomerViewModel());
            //TabItems.Add(new CheckOutViewModel());
        }

        private void InitializeViewModelClass()
        {
            _bookViewModel = new BookViewModel(_log, _bookDataAccess, _allPastOrderDataAccess);
            _customerViewModel = new CustomerViewModel(_log, _customerDataAccess);
            _checkOutViewModel = new CheckOutViewModel(_log, _bookDataAccess, _customerDataAccess, _allPastOrderDataAccess, _allPastOrderedBookDataAcess);
            CurrentScreen = _bookViewModel;
            TabSwitchCommand = new ApplicationCommand<object>(TabSwitch);
            CloseWindow = new ApplicationCommand<object>(CloseApplication);
            HelpCommand = new ApplicationCommand(OpenAppHelpManual);
        }

        
        private void TabSwitch(object parameter)
        {
            var _button = parameter as Button;
            if (_button != null)
            {
                string _uid = _button.Uid;
                int index = int.Parse(_uid);
                if (_uid == "0")
                {
                    gridmargin = new Thickness(10 + (150 * index), 0, 0, 0);
                    CurrentScreen = _bookViewModel;
                }

                else if (_uid == "1")
                {
                    gridmargin = new Thickness(10 + (150 * index), 0, 0, 0);
                    CurrentScreen = _customerViewModel;
                }


                else if (_uid == "2")
                {
                    gridmargin = new Thickness(10 + (150 * index), 0, 0, 0);
                    CurrentScreen = _checkOutViewModel;

                }
            }
        }

        private void OpenAppHelpManual()
        {
            System.Diagnostics.Process.Start(@"BIHelp.chm");
        }


        private void CloseApplication(object Parameter)
        {
            var _window = Parameter as Window;

            if (_window != null)
            {
                _log.Message("Shutting Down Application.....");
                _window.Close();
            }
        }
    }
}
