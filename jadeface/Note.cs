using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace jadeface
{
    class Note
    {
         public Note()
        {
        }

        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        [DataMember(Name = "userid")]
        public string UserId { get; set; }

        [DataMember(Name = "isbn")]
        public string ISBN { get; set; }

        [DataMember(Name = "notecontent")]
        public string NoteContent { get; set; }  

        [DataMember(Name = "Timestamp")]
        public string Timestamp { get; set; }
    }
}
