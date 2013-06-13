using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace jadeface
{
    class User
    {
        public User()
        {
        }
        
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        [DataMember(Name = "userid")]
        public string UserId { get; set; }

        [DataMember(Name = "password")]
        public string Password { get; set; }

    }
}
