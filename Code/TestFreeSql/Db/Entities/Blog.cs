using FreeSql.DataAnnotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Db.Entities
{
    public class Blog
    {
        [Column(IsIdentity = true, IsPrimary = true)]
        public int BlogId { get; set; }
        public string Url { get; set; }
        public int Rating { get; set; }
    }
}
