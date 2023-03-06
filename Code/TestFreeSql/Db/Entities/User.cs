using FreeSql.DataAnnotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Db.Entities
{

    public class User
    {
        [Column(IsIdentity = true, IsPrimary = true)]
        public long UserId { get; set; }
        public string Name { get; set; }
        public int Age { get; set; }
    }

}
