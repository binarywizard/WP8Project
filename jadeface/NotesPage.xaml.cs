using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using SQLite;
using System.IO;
using Windows.Storage;
using System.Diagnostics;

namespace jadeface
{
    public partial class NotesPage : PhoneApplicationPage
    {
        //Local Database Storage Definition
        /// <summary>
        /// The database path.
        /// </summary>
        /// 
        private BookService bookService;
        PhoneApplicationService phoneAppServeice = PhoneApplicationService.Current;

        private string dbPath;

        /// <summary>
        /// The sqlite connection.
        /// </summary>
        private SQLiteConnection dbConn;

        private BookListItem book;

        private string ISBN = "";

        public NotesPage()
        {
            InitializeComponent();
        }

        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            if (NavigationContext.QueryString.TryGetValue("BookISBN", out ISBN))
            {
                dbPath = Path.Combine(Path.Combine(ApplicationData.Current.LocalFolder.Path, "jadeface.sqlite"));
                dbConn = new SQLiteConnection(dbPath);
                SQLiteCommand command = dbConn.CreateCommand("select * from booklistitem where isbn = '" + ISBN + "'");
                List<BookListItem> books = command.ExecuteQuery<BookListItem>();
                if (books.Count == 1)
                {
                    book = books.First();
                    //BookDetailGrid.DataContext = book;
                }


            }
            else
            {
                MessageBox.Show("详细信息页面加载出错！");
                NavigationService.Navigate(new Uri("/MainPage.xaml", UriKind.Relative));
            }

            bookService = BookService.getInstance();
            showNoteList();

        }


        //private void BookListDrag(object sender, GestureEventArgs e)
        //{
        //    ApplicationBar.IsVisible = false;
        //}

        //private void BookListDragCompleted(object sender, GestureEventArgs e)
        //{
        //    ApplicationBar.IsVisible = true;
        //}

        //清除笔记
        private void clearnotebtn_Click(object sender, RoutedEventArgs e)
        {
            this.notecontent.Text = "";
        }

        private void savenotebtn_Click(object sender, RoutedEventArgs e)
        {
            ReadingNote note = new ReadingNote();
            note.UserId = phoneAppServeice.State["username"].ToString();
            note.ISBN = ISBN;
            note.NoteContent = this.notecontent.Text + "\n";
            note.NoteTime = DateTime.Now.ToString();

            Debug.WriteLine("[DEBUG]note.UserId: " + note.UserId + "  note.ISBN:" + note.ISBN + "  note.NoteContent" + note.NoteContent +
                "  note.NoteTime:" + note.NoteTime);

            bool result = bookService.insertNote(note);

            if (result)
            {
                MessageBox.Show("Succeed!");
                showNoteList();
            }
            else
            {
                MessageBox.Show("Failed!");
            }


           
        }

        private void notecontent_GotFocus(object sender, RoutedEventArgs e)
        {
            if (this.notecontent.Text.Equals("请把笔记内容输入到这里"))
            {
                this.notecontent.Text = "";
            }
        }

        private void showNoteList()
        {
            List<ReadingNote> notelist = new List<ReadingNote>();

            string username = phoneAppServeice.State["username"].ToString();
            notelist = bookService.RefreshReadingNote(username,ISBN);

            if (notelist.Count > 0)
            {
                NoteListItems.ItemsSource = notelist;
                this.bn.Text = "书名：";
                this.bookname.Text = book.Title + "\n";
            }
        }

    }
}