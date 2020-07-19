﻿using BookInventorySystem.Model;
using BookInventorySystem.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using DataAccessLayer;
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
            this.DataContext = new LoginViewModel(new DataAccess<AdminModel,AdminModel>());
            
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            App.pswd = Pswd.Password;
            //this.Close();
        }
    }
}
