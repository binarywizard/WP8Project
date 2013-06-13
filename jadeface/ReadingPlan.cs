using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace jadeface
{
    public class ReadingPlan
    {
        public ReadingPlan()
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

        //截止日期
        [DataMember(Name = "datePicker")]
        public string DatePicker { get; set; }

        [DataMember(Name = "priority")]
        public string Priority { get; set; }

        [DataMember(Name = "isreminder")]
        public bool IsReminder { get; set; }

        //提醒时间
        [DataMember(Name = "ringtime")]
        public string RingTime { get; set; }

        //备注
        [DataMember(Name = "detail")]
        public string Detail { get; set; }

        [DataMember(Name = "image")]
        public string Image { get; set; }
    }
}
