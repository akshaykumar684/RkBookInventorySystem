using BILogger;
using BookInventorySystem.ViewModel;
using System.Windows;

namespace BookInventorySystem.View
{
    /// <summary>
    /// Interaction logic for MainWindowView.xaml
    /// </summary>
    public partial class MainWindowView : Window
    {
        private ILogger _log;
        public MainWindowView(ILogger Log)
        {
            InitializeComponent();
            _log = Log;
            this.DataContext = new MainWindowViewModel(_log);
        }
    }
}
