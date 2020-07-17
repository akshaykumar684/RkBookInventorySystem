using System.Collections.ObjectModel;
using DataAccessLayer;
using System.Configuration;
using System.Windows.Input;
using System.Windows;
using System.Windows.Controls;

namespace BookInventorySystem.ViewModel
{
    public class MainWindowViewModel : ViewModelBase
    {


        public ICommand TabSwitchCommand { get; set; }

        public ICommand CloseWindow { get; set; }

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
        public MainWindowViewModel()
        {
            Connection.InitiaizeDataAccessLayer(ConfigurationManager.ConnectionStrings[DbName].ConnectionString);



            //TabItems = new ObservableCollection<ViewModelBase>();
            //TabItems.Add(new BookViewModel());
            //TabItems.Add(new CustomerViewModel());
            //TabItems.Add(new CheckOutViewModel());


            _bookViewModel = new BookViewModel();
            _customerViewModel = new CustomerViewModel();
            _checkOutViewModel = new CheckOutViewModel();
            CurrentScreen = _bookViewModel;
            TabSwitchCommand = new ApplicationCommand<object>(TabSwitch);
            CloseWindow = new ApplicationCommand<object>(CloseApplication);
        }

        private void TabSwitch(object parameter)
        {
            var t = parameter as Button;
            if (t != null)
            {
                string x = t.Uid;
                int index = int.Parse(x);
                if (x == "0")
                {

                    gridmargin = new Thickness(10 + (150 * index), 0, 0, 0);
                }

                else if (x == "1")
                {
                    gridmargin = new Thickness(10 + (150 * index), 0, 0, 0);
                    CurrentScreen = _bookViewModel;
                }


                else if (x == "2")
                {
                    gridmargin = new Thickness(10 + (150 * index), 0, 0, 0);
                    CurrentScreen = _customerViewModel;
                }

                else if (x == "3")
                {
                    gridmargin = new Thickness(10 + (150 * index), 0, 0, 0);
                    CurrentScreen = _checkOutViewModel;
                }
            }
        }

        private void CloseApplication(object Parameter)
        {
            var _window = Parameter as Window;

            if (_window != null)
            {
                _window.Close();
            }
        }
    }
}
