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
using BookInventorySystem.View;
namespace BookInventorySystem.ViewModel
{
    public class BookViewModel : ViewModelBase
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
                if(value!=null)
                    FillAllField(value);
                OnPropertyChange();
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

        public static event EventHandler BookUpdateEvent;

        MessagePopUp popup;
        public BookViewModel()
        {
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
            AllOrderData = new List<CheckOutModel>();
            ErrorMsgVisibility = Visibility.Collapsed;
        }


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

            await DataAccess<BookModel>.UpdateData(book,Properties.Resources.UpdateBook);
            GetBooks();
            InvokeBookUpdate();
            ClearAllField();
        }
        
        private async void GetBooks()
        {
            ErrorMsgVisibility = Visibility.Collapsed;
            BookCollection.Clear();

            Task<List<BookModel>> task = Task.Run<List<BookModel>>(() => {
              var t =  DataAccess<BookModel>.GetAllData(Properties.Resources.GetAllBooks);
              return t;
            });
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

            if(_searchedBook!=null && _searchedBook.BookName == BookName)
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

            await DataAccess<BookModel>.InsertData(book,Properties.Resources.InsertBook);
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
                if(tx!=null)
                {
                    ErrorMsg = "Cannot Delete this book.Someuser owns it\n Please delete that user or return the book";
                    ErrorMsgVisibility = Visibility.Visible;
                    return;
                }
                await DataAccess<BookModel>.DeleteData(SelectedBook,Properties.Resources.DeleteBook);
                GetBooks();
                InvokeBookUpdate();
                ClearAllField();
            }
            catch (System.Data.SqlClient.SqlException ex)
            {
                MessageBox.Show(ex.Message);
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

        private void InvokeBookUpdate()
        {
            BookUpdateEvent?.Invoke(new Object(), EventArgs.Empty);
        }


        private async void GetAllOrders()
        {
            Task<List<CheckOutModel>> task = Task.Run<List<CheckOutModel>>(() => {
                var t = CheckOutIn<CheckOutModel>.GetAllOrderData(Properties.Resources.GetAllOrders);
                return t;
            });
            var orderCollection = await task;
            AllOrderData.Clear();
            orderCollection.ForEach(_order => AllOrderData.Add(_order));
        }
    }
}
