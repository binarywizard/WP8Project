using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace jadeface
{
    public class ReadingRecord
    {
        public ReadingRecord()
        {
        }

        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        [DataMember(Name = "userid")]
        public string UserId { get; set; }

        [DataMember(Name = "isbn")]
        public string ISBN { get; set; }

        [DataMember(Name = "startpageno")]
        public int StartPageNo { get; set; }

        [DataMember(Name = "endpageno")]
        public int EndPageNo { get; set; }

        [DataMember(Name = "timestamp")]
        public string Timestamp { get; set; }
    }
}
