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
        public string TabName { get; set; }



        private BookModel _selectedBook;

        public BookModel SelectedBook
        {
            get { return _selectedBook; }
            set
            {
                _selectedBook = value;
                OnPropertyChange();
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
            }
        }

        public ICommand CheckOut { get; set; }

        public ICommand CheckIn { get; set; }


        public ObservableCollection<BookModel> BookCollection { get; set; }
        public ObservableCollection<BookModel> BookCollectionBelongingToSelectedCustomer { get; set; }

        public ObservableCollection<CustomerModel> CustomerCollection { get; set; }

        public ObservableCollection<CustomerModel> CustomerCollectionHavingBook { get; set; }

        public List<CheckOutModel> AllOrderData { get; set; }
        

        public CheckOutViewModel()
        {
            TabName = "CheckOutDetails";
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
        }

        private void BookViewModel_BookUpdateEvent(object sender, EventArgs e)
        {
            GetBooks();
        }

        private async void CheckInBook()
        {
            if (SelectedBookOfSelectedCustomer != null && SelectedCustomerHavingBook != null)
            {
                var obj = new CheckOutModel()
                {
                    BookId = SelectedBookOfSelectedCustomer.BookId,
                    CustomerId = SelectedCustomerHavingBook.CustomerId,
                    Quantity = SelectedBookOfSelectedCustomer.Quantity + 1
                };

                await CheckOutIn<CheckOutModel>.CheckOutBook(obj, Properties.Resources.CheckInBook);
                GetAllOrders();
                GetBooks();
                GetAllCustomerHavingBook();
                BookCollectionBelongingToSelectedCustomer.Clear();
            }
            else
            {
                MessageBox.Show("Please Select All field");
            }
        }

        private async void CheckOutBook()
        {
            if (SelectedBook != null && SelectedCustomer != null)
            {
                if (SelectedBook.Quantity <= 0)
                {
                    MessageBox.Show("Book Out of Stock");
                    return;
                }
                ///Check if the user has already taken the same book previously
                var tx = AllOrderData.Where(t => t.BookId == SelectedBook.BookId && t.CustomerId == SelectedCustomer.CustomerId && t.HasBook == 1).ToList();
                if (tx.Count!=0)
                {
                    MessageBox.Show("This User has this book");
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

                await CheckOutIn<CheckOutModel>.CheckOutBook(_checkOutModel, Properties.Resources.CheckOutBook);
                GetAllOrders();
                GetBooks();
                GetAllCustomerHavingBook();
            }
            else
            {
                MessageBox.Show("Please Select All field");
            }
        }

        private async void GetBooks()
        {
            Task<List<BookModel>> task = Task.Run<List<BookModel>>(() => {
                var t = DataAccess<BookModel>.GetAllData(Properties.Resources.GetAllBooks);
                return t;
            });
            BookCollection.Clear();
            var booksCollection = await task;
            booksCollection.ForEach(t => BookCollection.Add(t));
        }

        private async void GetAllCustomer()
        {
            Task<List<CustomerModel>> task = Task.Run<List<CustomerModel>>(() => {
                var t = DataAccess<CustomerModel>.GetAllData(Properties.Resources.GetAllCustomer);
                return t;
            });
            CustomerCollection.Clear();
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
                var t = DataAccess<BookModel,CustomerModel>.GetAllData(Properties.Resources.GetAllBooksOfSelectedCustomer, _customerModel);
                return t;
            });
            BookCollectionBelongingToSelectedCustomer.Clear();
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
                var t = DataAccess<CustomerModel>.GetAllData(Properties.Resources.GetAllCustomerHavingBook);
                return t;
            });
            
            var _customerCollectionHavingBook = await task;
            _customerCollectionHavingBook.ForEach(t => CustomerCollectionHavingBook.Add(t));
        }



        private async void GetAllOrders()
        {
            Task<List<CheckOutModel>> task = Task.Run<List<CheckOutModel>>(() => {
                var t = CheckOutIn<CheckOutModel>.GetAllOrderData(Properties.Resources.GetAllOrders); ;
                return t;
            });
            var orderCollection = await task;
            AllOrderData.Clear();
            orderCollection.ForEach(_order => AllOrderData.Add(_order));
        }
    }
}
