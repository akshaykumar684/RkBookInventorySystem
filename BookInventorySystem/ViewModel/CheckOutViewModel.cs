using BILogger;
using BookInventorySystem.Model;
using DataAccessLayer;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace BookInventorySystem.ViewModel
{
    public class CheckOutViewModel : ViewModelBase
    {

        private BookModel _selectedBook;

        public BookModel SelectedBook
        {
            get { return _selectedBook; }
            set
            {
                _selectedBook = value;
                OnPropertyChange();
                VisibilityCollapser();
            }
        }

        private BookModel _selectedBookOfSelectedCustomer;

        public BookModel SelectedBookOfSelectedCustomer
        {
            get { return _selectedBookOfSelectedCustomer; }
            set
            {
                _selectedBookOfSelectedCustomer = value;
                OnPropertyChange();
                VisibilityCollapser();
            }
        }


        private CustomerModel _selectedCustomer;

        public CustomerModel SelectedCustomer
        {
            get { return _selectedCustomer; }
            set
            {
                _selectedCustomer = value;
                OnPropertyChange();
                VisibilityCollapser();
            }
        }


        private CustomerModel _selectedCustomerHavingBook;

        public CustomerModel SelectedCustomerHavingBook
        {
            get { return _selectedCustomerHavingBook; }
            set
            {
                _selectedCustomerHavingBook = value;
                GetAllBooksOfSelectedCustomer();
                OnPropertyChange();
                VisibilityCollapser();
            }
        }

        private string _checkOutErrorMessage;

        public string CheckOutErrorMessage
        {
            get { return _checkOutErrorMessage; }
            set
            {
                _checkOutErrorMessage = value;
                OnPropertyChange();
            }
        }

        private string _checkInErrorMessage;

        public string CheckInErrorMessage
        {
            get { return _checkInErrorMessage; }
            set
            {
                _checkInErrorMessage = value;
                OnPropertyChange();
            }
        }

        private Visibility _checkOutErrorMessageVisibility;

        public Visibility CheckOutErrorMessageVisibility
        {
            get { return _checkOutErrorMessageVisibility; }

            set
            {
                _checkOutErrorMessageVisibility = value;
                OnPropertyChange();
            }
        }

        private Visibility _checkInErrorMessageVisibility;

        public Visibility CheckInErrorMessageVisibility
        {
            get { return _checkInErrorMessageVisibility; }
            set
            {
                _checkInErrorMessageVisibility = value;
                OnPropertyChange();
            }
        }

        public ICommand CheckOut { get; set; }

        public ICommand CheckIn { get; set; }


        public ObservableCollection<BookModel> BookCollection { get; set; }
        public ObservableCollection<BookModel> BookCollectionBelongingToSelectedCustomer { get; set; }

        public ObservableCollection<CustomerModel> CustomerCollection { get; set; }

        public ObservableCollection<CustomerModel> CustomerCollectionHavingBook { get; set; }

        public List<CheckOutModel> AllOrderData { get; set; }

        public static event EventHandler CheckInOutUpdateEvent;

        private ILogger _log;

        private IDataAccess<BookModel> _bookDataAccess;

        private IDataAccess<CustomerModel> _customerDataAccess;

        private IDataAccess<CheckOutModel> _allCheckInoutOrderDataAccess;

        public CheckOutViewModel(ILogger Log, IDataAccess<BookModel> BookDataAccess, IDataAccess<CustomerModel> CustomerDataAccess, IDataAccess<CheckOutModel> AllPastOrderDataAccess)
        {
            _log = Log;
            _bookDataAccess = BookDataAccess;
            _customerDataAccess = CustomerDataAccess;
            _allCheckInoutOrderDataAccess = AllPastOrderDataAccess;
            BookViewModel.BookUpdateEvent += BookViewModel_BookUpdateEvent;
            CustomerViewModel.CustomerUpdateEvent += CustomerViewModel_CustomerUpdateEvent;
            InitializePrperty();

        }

        private void InitializePrperty()
        {
            BookCollection = new ObservableCollection<BookModel>();
            BookCollectionBelongingToSelectedCustomer = new ObservableCollection<BookModel>();
            CustomerCollection = new ObservableCollection<CustomerModel>();
            CustomerCollectionHavingBook = new ObservableCollection<CustomerModel>();

            AllOrderData = new List<CheckOutModel>();
            CheckOut = new ApplicationCommand(CheckOutBook);
            CheckIn = new ApplicationCommand(CheckInBook);
            GetBooks();
            GetAllCustomer();
            GetAllOrders();
            GetAllCustomerHavingBook();
        }

        private void CustomerViewModel_CustomerUpdateEvent(object sender, EventArgs e)
        {
            GetAllCustomer();
            GetAllOrders();
            GetBooks();
            GetAllCustomerHavingBook();
            BookCollectionBelongingToSelectedCustomer.Clear();
        }

        private void BookViewModel_BookUpdateEvent(object sender, EventArgs e)
        {
            GetBooks();
        }

        private async void CheckInBook()
        {
            VisibilityCollapser();
            if (SelectedBookOfSelectedCustomer != null && SelectedCustomerHavingBook != null)
            {
                var obj = new CheckOutModel()
                {
                    BookId = SelectedBookOfSelectedCustomer.BookId,
                    CustomerId = SelectedCustomerHavingBook.CustomerId,
                    Quantity = SelectedBookOfSelectedCustomer.Quantity + 1
                };

                await _allCheckInoutOrderDataAccess.CheckOutBook(obj, Properties.Resources.CheckInBook);
                _log.Message("Checkin book ");
                RaiseCheckInOutEvent();
                GetAllOrders();
                GetBooks();
                GetAllCustomerHavingBook();
                BookCollectionBelongingToSelectedCustomer.Clear();

            }
            else
            {
                CheckInErrorMessage = Properties.Resources.EmptyFieldErrorMsg;
                CheckInErrorMessageVisibility = Visibility.Visible;
            }
        }

        private async void CheckOutBook()
        {
            VisibilityCollapser();
            if (SelectedBook != null && SelectedCustomer != null)
            {
                if (SelectedBook.Quantity <= 0)
                {
                    CheckOutErrorMessage = Properties.Resources.BookOutOfStockMsg;
                    CheckOutErrorMessageVisibility = Visibility.Visible;
                    return;
                }
                ///Check if the user has already taken the same book previously
                var tx = AllOrderData.Where(t => t.BookId == SelectedBook.BookId && t.CustomerId == SelectedCustomer.CustomerId && t.HasBook == 1).ToList();
                if (tx.Count != 0)
                {
                    //MessageBox.Show("This User has this book");
                    CheckOutErrorMessage = Properties.Resources.UserAlreadyHasBookMsg;
                    CheckOutErrorMessageVisibility = Visibility.Visible;
                    return;
                }


                CheckOutModel _checkOutModel = new CheckOutModel()
                {
                    OrderId = DateTime.Now.ToString().GetHashCode().ToString("x"),
                    BookId = SelectedBook.BookId,
                    CustomerId = SelectedCustomer.CustomerId,
                    DateTime = DateTime.Now,
                    HasBook = 1,
                    Quantity = SelectedBook.Quantity - 1
                };

                await _allCheckInoutOrderDataAccess.CheckOutBook(_checkOutModel, Properties.Resources.CheckOutBook);
                _log.Message("CheckOut book ");
                GetAllOrders();
                GetBooks();
                GetAllCustomerHavingBook();
                RaiseCheckInOutEvent();
            }
            else
            {
                CheckOutErrorMessage = Properties.Resources.EmptyFieldErrorMsg;
                CheckOutErrorMessageVisibility = Visibility.Visible;
            }
        }

        private async void GetBooks()
        {
            Task<List<BookModel>> task = Task.Run<List<BookModel>>(() => {
                var t = _bookDataAccess.GetAllData(Properties.Resources.GetAllBooks);
                return t;
            });
            BookCollection.Clear();
            _log.Message("Getting All books from the database");
            var booksCollection = await task;
            booksCollection.ForEach(t => BookCollection.Add(t));
        }

        private async void GetAllCustomer()
        {
            Task<List<CustomerModel>> task = Task.Run<List<CustomerModel>>(() => {
                var t = _customerDataAccess.GetAllData(Properties.Resources.GetAllCustomer);
                return t;
            });
            CustomerCollection.Clear();
            _log.Message("Getting All Customer from the database");
            var _customerCollection = await task;
            _customerCollection.ForEach(t => CustomerCollection.Add(t));
        }

        /// <summary>
        /// This method gives all the books of Currently selected user
        /// </summary>
        private async void GetAllBooksOfSelectedCustomer()
        {
            var _customerModel = new CustomerModel()
            {
                CustomerId = SelectedCustomerHavingBook.CustomerId
            };
            Task<List<BookModel>> task = Task.Run<List<BookModel>>(() => {
                var t = DataAccess<BookModel, CustomerModel>.GetAllData(Properties.Resources.GetAllBooksOfSelectedCustomer, _customerModel);
                return t;
            });
            BookCollectionBelongingToSelectedCustomer.Clear();
            _log.Message("Getting All books of selected user from the database");
            var booksCollectionOfSelectedUser = await task;
            booksCollectionOfSelectedUser.ForEach(t => BookCollectionBelongingToSelectedCustomer.Add(t));
        }


        /// <summary>
        /// This method gives all the customer who has currently owing some book
        /// </summary>
        private async void GetAllCustomerHavingBook()
        {
            CustomerCollectionHavingBook.Clear();
            Task<List<CustomerModel>> task = Task.Run<List<CustomerModel>>(() => {
                var t = _customerDataAccess.GetAllData(Properties.Resources.GetAllCustomerHavingBook);
                return t;
            });
            _log.Message("Getting All customer from the database");
            var _customerCollectionHavingBook = await task;
            _customerCollectionHavingBook.ForEach(t => CustomerCollectionHavingBook.Add(t));
        }



        private async void GetAllOrders()
        {
            Task<List<CheckOutModel>> task = Task.Run<List<CheckOutModel>>(() => {
                var t = _allCheckInoutOrderDataAccess.GetAllOrderData(Properties.Resources.GetAllOrders);
                return t;
            });
            _log.Message("Getting all order details");
            var orderCollection = await task;
            AllOrderData.Clear();
            orderCollection.ForEach(_order => AllOrderData.Add(_order));
        }

        /// <summary>
        /// This method raise event for checkIn and checkOut of book so that BookViewModel and CustomerViewModel get notified and make necessary
        /// Api calls
        /// </summary>

        private void RaiseCheckInOutEvent()
        {
            CheckInOutUpdateEvent?.Invoke(new object(), EventArgs.Empty);
        }

        private void VisibilityCollapser()
        {
            CheckInErrorMessageVisibility = Visibility.Hidden;
            CheckOutErrorMessageVisibility = Visibility.Hidden;
        }
    }
}
