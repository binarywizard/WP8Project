using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace jadeface
{
    public class ReadingNote
    {
        public ReadingNote()
        {
        }

        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        [DataMember(Name = "userid")]
        public string UserId { get; set; }

        [DataMember(Name = "isbn")]
        public string ISBN { get; set; }

        //笔记时间
        [DataMember(Name = "notetime")]
        public string NoteTime { get; set; }

        //笔记内容
        [DataMember(Name = "notecontent")]
        public string NoteContent { get; set; }

        
    }
}
