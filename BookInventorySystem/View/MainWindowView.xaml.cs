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
        public MainWindowView()
        {
            InitializeComponent();
            ILogger _log = new Logger();
            this.DataContext = new MainWindowViewModel(_log);
        }
    }
}
