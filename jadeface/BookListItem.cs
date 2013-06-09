using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace jadeface
{
    public enum BookStatus
    {
        WISH,
        READING,
        FINISHED
    }

    public class BookListItem
    {

        public BookListItem()
        {
        }

        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        [DataMember(Name = "userid")]
        public string UserId { get; set; }

        [DataMember(Name = "isbn")]
        public string ISBN { get; set; }

        [DataMember(Name = "title")]
        public string Title { get; set; }

        [DataMember(Name = "author")]
        public string Author { get; set; }

        [DataMember(Name = "marking")]
        public float Marking { get; set; }

        [DataMember(Name = "pageno")]
        public int PageNo { get; set; }

        [DataMember(Name = "curpageno")]
        public int CurPageNo { get; set; }

        [DataMember(Name = "publisher")]
        public string Publisher { get; set; }

        [DataMember(Name = "image")]
        public string Image { get; set; }

        [DataMember(Name = "status")]
        public BookStatus Status { get; set; }

        [DataMember(Name = "summary")]
        public string Summary { get; set; }

        [DataMember(Name = "rating")]
        public double Rating { get; set; }

        [DataMember(Name = "Timestamp")]
        public string Timestamp { get; set; }
    }
}
