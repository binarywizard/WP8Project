using Microsoft.WindowsAzure.MobileServices;
using SQLite;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;

namespace jadeface
{
    public class BookService
    {
        private IMobileServiceTable<BookListItem> bookListTable = null;
        //private IMobileServiceTable<ReadingRecord> readingRecordsTable = null;

        private string dbPath;

        private SQLiteConnection dbConn;

        private static BookService service = null;

        private BookService() 
        {
            dbPath = Path.Combine(Path.Combine(ApplicationData.Current.LocalFolder.Path, "jadeface.sqlite"));
            dbConn = new SQLiteConnection(dbPath);
            dbConn.CreateTable<BookListItem>();
            dbConn.CreateTable<ReadingRecord>();
        }

        public void destoryInstance()
        {
            dbConn.Close();
        }

        public static BookService getInstance()
        {

            if (service == null)
            {
                service = new BookService();
                return service;
            }
            else
            {
                return service;
            }
        }

        public List<BookListItem> RefreshWishBookList(string username)
        {
            if (username.Equals("") || username == null)
            {
                return null;
            }
            Debug.WriteLine("[DEBUG]Refresh SQL is : " + "select * from booklistitem where userid='" + username + "'");
            SQLiteCommand command = dbConn.CreateCommand("select * from booklistitem where userid='" + username + "' and status = 0");
            List<BookListItem> books = command.ExecuteQuery<BookListItem>();
            return books;
        }

        public List<BookListItem> RefreshBookList(string username)
        {
            if (username.Equals("")||username == null)
            {
                return null;
            }
            Debug.WriteLine("[DEBUG]Refresh SQL is : " + "select * from booklistitem where userid='" + username + "'");
            SQLiteCommand command = dbConn.CreateCommand("select * from booklistitem where userid='" + username + "' and status = 1");
            List<BookListItem> books = command.ExecuteQuery<BookListItem>();
            return books;
        }

        public List<BookListItem> RefreshFinishBookList(string username)
        {
            if (username.Equals("") || username == null)
            {
                return null;
            }
            Debug.WriteLine("[DEBUG]Refresh SQL is : " + "select * from booklistitem where userid='" + username + "'");
            SQLiteCommand command = dbConn.CreateCommand("select * from booklistitem where userid='" + username + "' and status = 2");
            List<BookListItem> books = command.ExecuteQuery<BookListItem>();
            return books;
        }

        public bool update(BookListItem book)
        {

            if (book.UserId == null || book.Title == null || book.ISBN == null || book.UserId.Equals("") || book.Title.Equals("") || book.ISBN.Equals(""))
            {
                return false;
            }

            if (dbConn.Update(book) > 0)
            {
                return true;
            }

            return false;
        }

        public bool insert(BookListItem book)
        {
            if (book.UserId == null || book.Title == null || book.ISBN == null || book.UserId.Equals("") || book.Title.Equals("") || book.ISBN.Equals(""))
            {
                return false;
            }

            if (dbConn.Insert(book) > 0)
            {
                return true;
            }

            return false;
        }

        public bool delete(BookListItem book, string username)
        {
            if (username.Equals("") || username == null)
            {
                return false;
            }
            Debug.WriteLine("delete from booklistitem where isbn = " + book.ISBN + " and userid = '" + username + "'");
            SQLiteCommand command = dbConn.CreateCommand("delete from booklistitem where isbn = " + book.ISBN + " and userid = '" + username + "'");
            command.ExecuteQuery<BookListItem>();
            return true;
        }

        public List<BookListItem> searchByISBN(string ISBN, string username)
        {
            if (username.Equals("") || username == null)
            {
                return null;
            }
            Debug.WriteLine("[DEBUG]Select SQL is : " + "select * from booklistitem where isbn = '" + ISBN + "' and userid = '" + username + "'");
            SQLiteCommand command = dbConn.CreateCommand("select * from booklistitem where isbn = '" + ISBN + "' and userid = '" + username + "'");
            List<BookListItem> items = command.ExecuteQuery<BookListItem>();
            return items;
        }

        public List<BookListItem> searchAllBooks(string username)
        {
            if (username == null || username.Equals(""))
            {
                return null;
            }
            Debug.WriteLine("[DEBUG]Select SQL is : " + "select * from booklistitem where userid = '" + username + "'");
            SQLiteCommand command = dbConn.CreateCommand("select * from booklistitem where userid = '" + username + "'");
            List<BookListItem> items = command.ExecuteQuery<BookListItem>();
            return items;
        }

        public void SychcBookData(string username)
        {
            if (username == null || username.Equals(""))
            {
                return;
            }
            else if (bookListTable == null)
            {
                bookListTable = App.MobileService.GetTable<BookListItem>();
            }

            List<BookListItem> items = searchAllBooks(username);
            if (items.Count > 0)
            {
                string timestamp = DateTime.Now.ToString();
                foreach (BookListItem item in items)
                {
                    item.Timestamp = timestamp;
                }

                foreach (BookListItem item in items)
                {
                    bookListTable.InsertAsync(item);
                }
            }
        }

        

        // 读书记录的数据库操作

        public bool insertRecord(ReadingRecord record)
        {
            if (record.UserId == null || record.ISBN == null || record.Timestamp == null || record.UserId.Equals("") || record.ISBN.Equals("") || record.Timestamp.Equals(""))
            {
                return false;
            }

            if (dbConn.Insert(record) > 0)
            {
                return true;
            }

            return false;
        }

        public bool deleteRecord(ReadingRecord record)
        {
            if (record.Id.Equals(""))
            {
                return false;
            }
            Debug.WriteLine("delete from readingrecord where Id = " + record.Id);
            SQLiteCommand command = dbConn.CreateCommand("delete from readingrecord where Id = " + record.Id);
            command.ExecuteQuery<ReadingRecord>();
            return true;
        }

        public List<ReadingRecord> RefreshReadingRecord(string username, string isbn)
        {
            if (username.Equals("") || username == null || isbn.Equals("") || isbn == null)
            {
                return null;
            }
            Debug.WriteLine("[DEBUG]Refresh SQL is : " + "select * from readingrecord where userid='" + username + "' and isbn='" + isbn + "'");
            SQLiteCommand command = dbConn.CreateCommand("select * from readingrecord where userid='" + username + "' and isbn='" + isbn + "'");
            List<ReadingRecord> records = command.ExecuteQuery<ReadingRecord>();
            Debug.WriteLine("[DEBUG]records.Count = " + records.Count);
            return records;
        }
        
    }
}
