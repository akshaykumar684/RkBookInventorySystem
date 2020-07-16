using System.Collections.Generic;
using System.Threading.Tasks;
using BookInventorySystem.Model;
using System.Collections.ObjectModel;
using System.Windows.Input;
using DataAccessLayer;
using System.Text.RegularExpressions;
using System;
using System.Linq;
using System.Windows;
using BookInventorySystem.View;

namespace BookInventorySystem.ViewModel
{
    public class CustomerViewModel : ViewModelBase
    {
        public string TabName { get; set; }

        private string _customerId;

        public string CustomerId
        {
            get { return _customerId; }

            set
            {
                _customerId = value;
                OnPropertyChange();
            }
        }

        private string _customerName;

        public string CustomerName
        {
            get { return _customerName; }

            set
            {
                _customerName = value;
                OnPropertyChange();
            }
        }

        private string _address;

        public string Address
        {
            get { return _address; }

            set
            {
                _address = value;
                OnPropertyChange();
            }
        }


        private string _phoneNo;

        public string PhoneNo
        {
            get { return _phoneNo; }

            set
            {
                value = Regex.Replace(value, @"[^0-9]+", "");
                _phoneNo = value;
                OnPropertyChange();
            }
        }

        private CustomerModel _selectedCustomer;

        public CustomerModel SelectedCustomer
        {
            get { return _selectedCustomer; }

            set
            {
                _selectedCustomer = value;
                if(value!=null)
                    FillAllField(value);
                GetLastPurchaseHistory(value);
            }
        }

        

        public ICommand Add { get; set; }

        public ICommand Update { get; set; }


        public ICommand Delete { get; set; }


        public ICommand GetCustomer { get; set; }

        public ObservableCollection<CustomerModel> CustomerCollection { get; set; }

        public ObservableCollection<CustomerHistoryModel> PreviousBookOrderCollection { get; set; }

        public static event EventHandler CustomerUpdateEvent;

        private MessagePopUp _messagePopup;
        public CustomerViewModel()
        {
            TabName = "CustomerDetails";
            CheckOutViewModel.CheckInOutUpdateEvent += CheckOutViewModel_CheckInOutUpdateEvent;
            InitializeProperty();
            GetAllCustomer();
        }

       

        private void InitializeProperty()
        {
            CustomerCollection = new ObservableCollection<CustomerModel>();
            PreviousBookOrderCollection = new ObservableCollection<CustomerHistoryModel>();
            Add = new ApplicationCommand(AddUser);
            Update = new ApplicationCommand(UpdateCustomer);
            Delete = new ApplicationCommand(DeleteCustomer);
            GetCustomer = new ApplicationCommand(GetAllCustomer);
        }

        private void CheckOutViewModel_CheckInOutUpdateEvent(object sender, EventArgs e)
        {
            SelectedCustomer = null;
            ClearAllField();
        }


        private async void AddUser()
        {
            if (!CheckAllInputFields())
                return;

            var _searchedCustomer = CustomerCollection.FirstOrDefault(s => s.CustomerName == CustomerName);

            // Check if there is already a book with same name and same authorname

            if (_searchedCustomer != null && _searchedCustomer.PhoneNo == PhoneNo)
            {
                MessageBox.Show("Duplicate");
                return;
            }


            CustomerModel customer = new CustomerModel()
            {
                CustomerId = DateTime.Now.ToString().GetHashCode().ToString("x"),
                CustomerName = this.CustomerName,
                Address = this.Address,
                PhoneNo = this.PhoneNo
            };

            await DataAccess<CustomerModel>.InsertData(customer, Properties.Resources.AddUser);
            GetAllCustomer();
            ClearAllField();
            InvokeCustomerUpdateEvent();
        }

        

        private async void UpdateCustomer()
        {
            if (SelectedCustomer == null || !CheckAllInputFields())
                return;

            var _searchedCustomer = CustomerCollection.FirstOrDefault(s => s.CustomerName == CustomerName);

            // Check if there is already a book with same name and same authorname

            if (_searchedCustomer != null && _searchedCustomer.PhoneNo == PhoneNo && _searchedCustomer.Address == Address)
            {
                MessageBox.Show("Duplicate");
                return;
            }


            CustomerModel customer = new CustomerModel()
            {
                CustomerId = SelectedCustomer.CustomerId,
                CustomerName = this.CustomerName,
                Address = this.Address,
                PhoneNo = this.PhoneNo
            };

            await DataAccess<CustomerModel>.UpdateData(customer, Properties.Resources.UpdateCustomer);
            GetAllCustomer();
            ClearAllField();
            InvokeCustomerUpdateEvent();
        }

        private bool _canDeleteUser = true;
        private async void DeleteCustomer()
        {
            try
            {
                if (SelectedCustomer == null)
                {
                    MessageBox.Show("Select Customer");
                    return;
                }
                    
                _canDeleteUser = true;
                /// Check if the selected user has taken some book
                var _customerHasBook = PreviousBookOrderCollection.FirstOrDefault(t => t.HasBook == 1);
                if(_customerHasBook!=null)
                {
                    //MessageBox.Show("This User has Book. Do you want to delete it?");
                    ShowMessagePopup("This User has Book.");
                }
                if (_canDeleteUser)
                {
                    await DataAccess<CustomerModel>.DeleteData(SelectedCustomer, Properties.Resources.DeleteCustomer);
                    GetAllCustomer();
                    ClearAllField();
                    InvokeCustomerUpdateEvent();
                    PreviousBookOrderCollection.Clear();
                }
            }
            catch (System.Data.SqlClient.SqlException ex)
            {
                MessageBox.Show(ex.Message);
            }
        }


        private void ShowMessagePopup(string Message)
        {
            _messagePopup = new MessagePopUp();
            _messagePopup.msg_Label.Content = Message;
            _messagePopup.OkButton.Click += (a, b) => { _messagePopup.Close(); };

            _messagePopup.CancelButton.Click += (a, b) =>
            {
                _canDeleteUser = false;
                _messagePopup.Close();
            };
           

             _messagePopup.ShowDialog();
        }


        private async void GetAllCustomer()
        {
            CustomerCollection.Clear();

            Task<List<CustomerModel>> task = Task.Run<List<CustomerModel>>(() => {
                var t = DataAccess<CustomerModel>.GetAllData(Properties.Resources.GetAllCustomer);
                return t;
            });
            var _customerCollection = await task;
            _customerCollection.ForEach(t => CustomerCollection.Add(t));
        }

        private bool CheckAllInputFields()
        {
            bool _isValid = true;

            if (string.IsNullOrEmpty(CustomerName) || string.IsNullOrEmpty(Address) || string.IsNullOrEmpty(PhoneNo))
                _isValid = false;

            return _isValid;
        }

        private void FillAllField(CustomerModel _customer)
        {
            CustomerName = _customer.CustomerName;
            Address = _customer.Address;
            PhoneNo = _customer.PhoneNo;
        }

        private async void GetLastPurchaseHistory(CustomerModel _customer)
        {
            Task<List<CustomerHistoryModel>> task = Task.Run<List<CustomerHistoryModel>>(() => {
                var t = DataAccess<CustomerHistoryModel,CustomerModel>.GetAllData(Properties.Resources.GetLastOrderDetails,_customer);
                return t;
            });
            PreviousBookOrderCollection.Clear();
            var _lastHistoryCollection = await task;
            _lastHistoryCollection.ForEach(t => PreviousBookOrderCollection.Add(t));
        }

        private void ClearAllField()
        {
            CustomerName = string.Empty;
            Address = string.Empty;
            PhoneNo = string.Empty;
        }

        private void InvokeCustomerUpdateEvent()
        {
            CustomerUpdateEvent?.Invoke(new Object(), EventArgs.Empty);
        }

    }
}
