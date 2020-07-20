using BookInventorySystem.Model;
using DataAccessLayer;
using System.Collections.ObjectModel;
using System.Windows.Input;
using System;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.ComponentModel;
using BILogger;

namespace BookInventorySystem.ViewModel
{
    public class BookViewModel : ViewModelBase, IDataErrorInfo
    {
        public ICommand GetBook { get; set; }

        public ICommand AddBook { get; set; }

        public ICommand Update { get; set; }

        public ICommand Delete { get; set; }


        private string _bookId;

        public string BookId
        {
            get { return _bookId; }

            set
            {
                _bookId = value;
                OnPropertyChange();
            }
        }


        private string _bookName;

        public string BookName
        {
            get { return _bookName; }
            set
            {
                _bookName = value;
                OnPropertyChange();
            }
        }


        private string _authorName;

        public string AuthorName
        {
            get { return _authorName; }

            set
            {
                _authorName = value;
                OnPropertyChange();
            }
        }

        private string _quantity;

        public string Quantity
        {
            get { return _quantity; }

            set
            {
                value = Regex.Replace(value, @"[^0-9]+", "");
                _quantity = value;
                OnPropertyChange();
            }
        }

        private BookModel _selectedBook;

        public BookModel SelectedBook
        {
            get { return _selectedBook; }

            set
            {
                _selectedBook = value;
                if (value != null)
                    FillAllField(value);
                OnPropertyChange();
                GetLastPurchaseHistory(value);
                GetAllOrders();
                ErrorMsgVisibility = Visibility.Collapsed;
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

        public ObservableCollection<BookModel> BookCollection { get; set; }
        public List<CheckOutModel> AllOrderData { get; set; }

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


                if (columnName == "BookName")
                {
                    if (String.IsNullOrEmpty(BookName))
                        result = Properties.Resources.BookNameErrorMsg;
                }

                if (columnName == "AuthorName")
                {
                    if (String.IsNullOrEmpty(AuthorName))
                        result = Properties.Resources.AuthorNameErrorMsg;
                }

                if (columnName == "Quantity")
                {
                    if (String.IsNullOrEmpty(Quantity))
                        result = Properties.Resources.QuantityErrorMsg;
                }


                return result;
            }
        }

        public static event EventHandler BookUpdateEvent;

        public ObservableCollection<CustomerHistoryModel> PreviousBookOrderCollection { get; set; }

        private ILogger _log;
        private IDataAccess<BookModel, CustomerHistoryModel> _dataAccess;
        private IDataAccess<CheckOutModel, CheckOutModel> _allPastOrderDataAccess;
        public BookViewModel(ILogger Log, IDataAccess<BookModel, CustomerHistoryModel> DataAccess, IDataAccess<CheckOutModel, CheckOutModel> AllPastOrderDataAccess)
        {
            _log = Log;
            _dataAccess = DataAccess;
            _allPastOrderDataAccess = AllPastOrderDataAccess;
            InitializeProperty();
            CheckOutViewModel.CheckInOutUpdateEvent += CheckOutViewModel_CheckInOutUpdateEvent;
            GetBooks();
            GetAllOrders();
        }



        private void InitializeProperty()
        {
            GetBook = new ApplicationCommand(GetBooks);
            AddBook = new ApplicationCommand(InsertBook);
            Update = new ApplicationCommand(UpdateBook);
            Delete = new ApplicationCommand(DeleteBook);
            BookCollection = new ObservableCollection<BookModel>();
            PreviousBookOrderCollection = new ObservableCollection<CustomerHistoryModel>();
            AllOrderData = new List<CheckOutModel>();
            ErrorMsgVisibility = Visibility.Collapsed;
        }

        /// <summary>
        /// this method is called by event,which will raise if the user has done some check in and checkout of book.
        /// so that the quantity of books will get auto updated
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CheckOutViewModel_CheckInOutUpdateEvent(object sender, EventArgs e)
        {
            GetBooks();
            SelectedBook = null;
            ClearAllField();
            //GetAllOrders();
        }


        private async void UpdateBook()
        {
            ErrorMsgVisibility = Visibility.Collapsed;
            if (SelectedBook == null || !CheckAllInputFields())
            {
                ErrorMsg = Properties.Resources.SelectBookErrorMsg;
                ErrorMsgVisibility = Visibility.Visible;
                return;
            }

            var _searchedBook = BookCollection.FirstOrDefault(s => s.AuthorName == AuthorName);

            // Check if there is already a book with same name and same authorname

            if (_searchedBook != null && _searchedBook.BookName == BookName && Convert.ToInt32(Quantity) == _searchedBook.Quantity)
            {
                ErrorMsg = Properties.Resources.SameBookExistMsg;
                ErrorMsgVisibility = Visibility.Visible;
                return;
            }

            BookModel book = new BookModel()
            {
                BookId = SelectedBook.BookId,
                BookName = this.BookName,
                AuthorName = this.AuthorName,
                Quantity = Convert.ToInt32(this.Quantity)
            };

            _log.Message("Updating Book");
            await _dataAccess.UpdateData(book, Properties.Resources.UpdateBook);
            GetBooks();
            InvokeBookUpdate();
            ClearAllField();
        }

        private async void GetBooks()
        {
            ErrorMsgVisibility = Visibility.Collapsed;
            BookCollection.Clear();

            Task<List<BookModel>> task = Task.Run<List<BookModel>>(() => {
                var t = _dataAccess.GetAllData(Properties.Resources.GetAllBooks);
                return t;
            });
            _log.Message("Getting All the bok from Database");
            var booksCollection = await task;
            booksCollection.ForEach(t => BookCollection.Add(t));
        }


        private async void InsertBook()
        {
            ErrorMsgVisibility = Visibility.Collapsed;
            if (!CheckAllInputFields())
                return;

            var _searchedBook = BookCollection.FirstOrDefault(s => s.AuthorName == AuthorName);

            // Check if there is already a book with same name and same authorname

            if (_searchedBook != null && _searchedBook.BookName == BookName)
            {
                ErrorMsg = Properties.Resources.SameBookExistMsg;
                ErrorMsgVisibility = Visibility.Visible;
                return;
            }

            BookModel book = new BookModel()
            {
                BookId = DateTime.Now.ToString().GetHashCode().ToString("x"),
                BookName = this.BookName,
                AuthorName = this.AuthorName,
                Quantity = Convert.ToInt32(this.Quantity)
            };

            _log.Message("Adding NewBook");
            await _dataAccess.InsertData(book, Properties.Resources.InsertBook);
            GetBooks();
            InvokeBookUpdate();
            ClearAllField();
        }

        private bool CheckAllInputFields()
        {
            bool _isValid = true;

            if (string.IsNullOrEmpty(BookName) || string.IsNullOrEmpty(Quantity) || string.IsNullOrEmpty(AuthorName))
                _isValid = false;

            return _isValid;
        }

        private async void DeleteBook()
        {
            try
            {
                ErrorMsgVisibility = Visibility.Collapsed;
                if (SelectedBook == null)
                {
                    ErrorMsg = Properties.Resources.SelectBookErrorMsg;
                    ErrorMsgVisibility = Visibility.Visible;
                    return;
                }

                var tx = AllOrderData.FirstOrDefault(t => t.BookId == SelectedBook.BookId && t.HasBook == 1);
                ///if the selected book has already been taken it will restrict the user from deleting it.
                ///User can delete this book only when all the book has returned or the user which has this book is deleted.
                if (tx != null)
                {
                    ErrorMsg = "Cannot Delete this book.Someuser owns it\n Please delete that user or return the book";
                    ErrorMsgVisibility = Visibility.Visible;
                    return;
                }
                await _dataAccess.DeleteData(SelectedBook, Properties.Resources.DeleteBook);
                _log.Message("Deleteing Book having BookId: " + SelectedBook.BookId);
                GetBooks();
                InvokeBookUpdate();
                ClearAllField();
            }
            catch (System.Data.SqlClient.SqlException ex)
            {
                _log.Error(ex);
            }
        }



        private void FillAllField(BookModel book)
        {
            Quantity = book.Quantity.ToString();
            BookName = book.BookName;
            AuthorName = book.AuthorName;
        }

        private void ClearAllField()
        {
            BookName = string.Empty;
            Quantity = string.Empty;
            AuthorName = string.Empty;
        }

        /// <summary>
        /// this method raise a event which will notify Checkout section, upon which all the dropdown list will automatically 
        /// get updated with changed books
        /// </summary>
        private void InvokeBookUpdate()
        {
            BookUpdateEvent?.Invoke(new Object(), EventArgs.Empty);
        }


        private async void GetAllOrders()
        {
            Task<List<CheckOutModel>> task = Task.Run<List<CheckOutModel>>(() => {
                var t = _allPastOrderDataAccess.GetAllOrderData(Properties.Resources.GetAllOrders);
                return t;
            });
            _log.Message("Getting All Orders Details from the database");
            var orderCollection = await task;
            AllOrderData.Clear();
            orderCollection.ForEach(_order => AllOrderData.Add(_order));
        }

        private async void GetLastPurchaseHistory(BookModel _book)
        {
            Task<List<CustomerHistoryModel>> task = Task.Run<List<CustomerHistoryModel>>(() => {
                var t = _dataAccess.GetAllData(Properties.Resources.GetLastBookOrderDetails, _book);
               // var tx = DataAccess2<CustomerHistoryModel, BookModel>.GetAllData(Properties.Resources.GetLastBookOrderDetails, _book);
                return t;
            });
            PreviousBookOrderCollection.Clear();
            _log.Message("Getting All the last order history of book " + _book.BookId);
            var _lastHistoryCollection = await task;
            _lastHistoryCollection.ForEach(t => PreviousBookOrderCollection.Add(t));
        }
    }
}
