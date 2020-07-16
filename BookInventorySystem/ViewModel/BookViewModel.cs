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
        public string TabName { get; set; }

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
            }
        }

        public ObservableCollection<BookModel> BookCollection { get; set; }
        public List<CheckOutModel> AllOrderData { get; set; }

        public static event EventHandler BookUpdateEvent;

        MessagePopUp popup;
        public BookViewModel()
        {
            TabName = "BookDetails";
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
            if (SelectedBook == null || !CheckAllInputFields())
                return;

            var _searchedBook = BookCollection.FirstOrDefault(s => s.AuthorName == AuthorName);

            // Check if there is already a book with same name and same authorname

            if (_searchedBook != null && _searchedBook.BookName == BookName && Convert.ToInt32(Quantity) == _searchedBook.Quantity)
            {
                MessageBox.Show("Duplicate");
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
            if (!CheckAllInputFields())
                return;

            var _searchedBook = BookCollection.FirstOrDefault(s => s.AuthorName == AuthorName);

            // Check if there is already a book with same name and same authorname

            if(_searchedBook!=null && _searchedBook.BookName == BookName)
            {
                MessageBox.Show("Duplicate");
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
                if (SelectedBook == null)
                    return;

                var tx = AllOrderData.FirstOrDefault(t => t.BookId == SelectedBook.BookId && t.HasBook == 1);
                if(tx!=null)
                {
                    MessageBox.Show("Cannot Delete This Book.SomeUser Owns It");
                    //ShowMessagePopup();
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

        private void ShowMessagePopup()
        {
            popup = new MessagePopUp();
            popup.msg_Label.Content = "This Book is Taken By some User.\n Cannot Delete it.\n Please delete the customer or check in this book";
            popup.CancelButton.Visibility = Visibility.Collapsed;
            popup.question_Label.Visibility = Visibility.Collapsed;
            popup.OkButton.HorizontalAlignment = HorizontalAlignment.Center;
            popup.msg_Label.HorizontalAlignment = HorizontalAlignment.Center;
            popup.msg_Label.Margin = new Thickness(-30, 0, 0, 0);
            popup.OkButton.Click += (a, b) => { popup.Close(); };
            popup.ShowDialog();
        }

    }
}
