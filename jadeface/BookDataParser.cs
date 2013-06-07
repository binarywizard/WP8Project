using Microsoft.WindowsAzure.MobileServices;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace jadeface
{
    class BookDataParser
    {
        public static List<BookListItem> parse(string content)
        {
            List<BookListItem> books = new List<BookListItem>();
            BookListItem book;
            JObject json = JObject.Parse(content);
            for (int i = 0; i < 5; i++)
            {
                int PageNo;
                book = new BookListItem();
                book.Title = (string)json["books"][i]["title"];
                book.ISBN = (string)json["books"][i]["isbn10"];
                book.Author = (string)(json["books"][i]["author"].First);
                int.TryParse((string)json["books"][i]["pages"], out PageNo);
                book.PageNo = PageNo;
                book.CurPageNo = 0;
                book.Publisher = (string)json["books"][i]["publisher"];
                book.Image = (string)json["books"][i]["images"]["small"];
                book.Summary = (string)json["books"][i]["summary"];
                books.Add(book);
            }
            return books;
        }
    }
}
