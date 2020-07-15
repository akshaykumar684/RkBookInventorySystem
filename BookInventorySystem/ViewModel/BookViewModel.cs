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
                FillAllField(value);
                OnPropertyChange();
            }
        }

        public ObservableCollection<BookModel> BookCollection { get; set; }

        public static event EventHandler BookUpdateEvent;
        public BookViewModel()
        {
            TabName = "BookDetails";
            InitializeProperty();
            GetBooks();
        }


        private void InitializeProperty()
        {
            GetBook = new ApplicationCommand(GetBooks);
            AddBook = new ApplicationCommand(InsertBook);
            Update = new ApplicationCommand(UpdateBook);
            Delete = new ApplicationCommand(DeleteBook);
            BookCollection = new ObservableCollection<BookModel>();
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
    }
}
