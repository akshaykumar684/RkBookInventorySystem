using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataAccessLayer;
using System.Configuration;
using BookInventorySystem.Model;

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
