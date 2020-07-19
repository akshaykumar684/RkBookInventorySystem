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
using System.ComponentModel;
using BILogger;

namespace BookInventorySystem.ViewModel
{
    public class CustomerViewModel : ViewModelBase, IDataErrorInfo
    {

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
                if (value != null)
                    FillAllField(value);
                GetLastPurchaseHistory(value);
                ErrorMsgVisibility = Visibility.Collapsed;
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

        public ICommand Add { get; set; }

        public ICommand Update { get; set; }


        public ICommand Delete { get; set; }


        public ICommand GetCustomer { get; set; }

        public ObservableCollection<CustomerModel> CustomerCollection { get; set; }

        public ObservableCollection<CustomerHistoryModel> PreviousBookOrderCollection { get; set; }

        public static event EventHandler CustomerUpdateEvent;

        private MessagePopUp _messagePopup;

        private ILogger _log;

        private IDataAccess<CustomerModel> _customerDataAccess;
        public CustomerViewModel(ILogger Log, IDataAccess<CustomerModel> CustomerDataAccess)
        {
            _log = Log;
            _customerDataAccess = CustomerDataAccess;
            CheckOutViewModel.CheckInOutUpdateEvent += CheckOutViewModel_CheckInOutUpdateEvent;
            InitializeProperty();
            GetAllCustomer();
            ErrorMsgVisibility = Visibility.Collapsed;
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

        public string Error
        {
            get
            {
                return null;
            }
        }

        public string this[string columnName]
        {
            get
            {
                string result = null;


                if (columnName == "CustomerName")
                {
                    if (String.IsNullOrEmpty(CustomerName))
                        result = Properties.Resources.CustomerNameErrorMsg;
                }

                if (columnName == "Address")
                {
                    if (String.IsNullOrEmpty(Address))
                        result = Properties.Resources.AddressErrorMsg;
                }

                if (columnName == "PhoneNo")
                {
                    if (String.IsNullOrEmpty(PhoneNo))
                        result = Properties.Resources.PhoneNoErrorMsg;
                }


                return result;
            }
        }


        private async void AddUser()
        {
            ErrorMsgVisibility = Visibility.Collapsed;
            if (!CheckAllInputFields())
                return;

            var _searchedCustomer = CustomerCollection.FirstOrDefault(s => s.CustomerName == CustomerName);

            // Check if there is already a book with same name and same authorname

            if (_searchedCustomer != null && _searchedCustomer.PhoneNo == PhoneNo)
            {
                ErrorMsg = Properties.Resources.UserAlreadyExistMsg;
                ErrorMsgVisibility = Visibility.Visible;
                return;
            }


            CustomerModel customer = new CustomerModel()
            {
                CustomerId = DateTime.Now.ToString().GetHashCode().ToString("x"),
                CustomerName = this.CustomerName,
                Address = this.Address,
                PhoneNo = this.PhoneNo
            };

            _log.Message("Adding New User having UserId :" + customer.CustomerId);
            await _customerDataAccess.InsertData(customer, Properties.Resources.AddUser);
            GetAllCustomer();
            ClearAllField();
            InvokeCustomerUpdateEvent();
        }



        private async void UpdateCustomer()
        {
            ErrorMsgVisibility = Visibility.Collapsed;
            if (SelectedCustomer == null || !CheckAllInputFields())
            {
                ErrorMsg = Properties.Resources.UserNotSelectedErrorMsg;
                ErrorMsgVisibility = Visibility.Visible;
                return;
            }


            var _searchedCustomer = CustomerCollection.FirstOrDefault(s => s.CustomerName == CustomerName);

            // Check if there is already a book with same name and same authorname

            if (_searchedCustomer != null && _searchedCustomer.PhoneNo == PhoneNo && _searchedCustomer.Address == Address)
            {
                ErrorMsg = Properties.Resources.UserAlreadyExistMsg;
                ErrorMsgVisibility = Visibility.Visible;
                return;
            }


            CustomerModel customer = new CustomerModel()
            {
                CustomerId = SelectedCustomer.CustomerId,
                CustomerName = this.CustomerName,
                Address = this.Address,
                PhoneNo = this.PhoneNo
            };

            _log.Message("Updating Customer Having Customer Id: " + customer.CustomerId);
            await _customerDataAccess.UpdateData(customer, Properties.Resources.UpdateCustomer);
            GetAllCustomer();
            ClearAllField();
            InvokeCustomerUpdateEvent();
        }

        private bool _canDeleteUser = true;
        private async void DeleteCustomer()
        {
            try
            {
                ErrorMsgVisibility = Visibility.Collapsed;
                if (SelectedCustomer == null)
                {
                    ErrorMsg = Properties.Resources.UserNotSelectedErrorMsg;
                    ErrorMsgVisibility = Visibility.Visible;
                    return;
                }

                _canDeleteUser = true;
                /// Check if the selected user has taken some book
                var _customerHasBook = PreviousBookOrderCollection.FirstOrDefault(t => t.HasBook == 1);
                if (_customerHasBook != null)
                {
                    ShowMessagePopup("This User has Book.");
                }
                if (_canDeleteUser)
                {
                    await _customerDataAccess.DeleteData(SelectedCustomer, Properties.Resources.DeleteCustomer);
                    GetAllCustomer();
                    ClearAllField();
                    InvokeCustomerUpdateEvent();
                    PreviousBookOrderCollection.Clear();
                    _log.Message("Deleting Customer Having CustomerID :" + SelectedCustomer.CustomerId);
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
                var t = _customerDataAccess.GetAllData(Properties.Resources.GetAllCustomer);
                return t;
            });
            _log.Message("Getting All Customer from the database");
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
                var t = DataAccess<CustomerHistoryModel, CustomerModel>.GetAllData(Properties.Resources.GetLastOrderDetails, _customer);
                return t;
            });
            PreviousBookOrderCollection.Clear();
            _log.Message("Getting all last purchase history from the database");
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
