using System.Collections.ObjectModel;
using DataAccessLayer;
using System.Configuration;

namespace BookInventorySystem.ViewModel
{
    public class MainWindowViewModel : ViewModelBase
    {

        private ObservableCollection<ViewModelBase> _tabItems;

        public ObservableCollection<ViewModelBase> TabItems
        {
            get { return _tabItems; }
            set
            {
                _tabItems = value;
            }
        }

        const string DbName = "SampleDB";
        public MainWindowViewModel()
        {
            Connection.InitiaizeDataAccessLayer(ConfigurationManager.ConnectionStrings[DbName].ConnectionString);
            TabItems = new ObservableCollection<ViewModelBase>();
            TabItems.Add(new BookViewModel());
            TabItems.Add(new CustomerViewModel());
            TabItems.Add(new CheckOutViewModel());
        }
    }
}
