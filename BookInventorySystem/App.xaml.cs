using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;

namespace BookInventorySystem
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public App()
        {
            this.Dispatcher.UnhandledException += OnDispatcherUnhandledException;
        }

        private void OnDispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            
            //string errorMessage = string.Format("An unhandled exception occurred: {0}", e.Exception.Message);
            //MessageBox.TabSwitch(errorMessage, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            e.Handled = true;
        }
    }
}
