using FreeSql.DataAnnotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Db.Entities
{
    /// <summary>
    /// 每七天分一次表
    /// </summary>
    [Table(Name = "as_table_log_{yyyyMMdd}", AsTable = "createtime=2023-1-1(7 day)")]
   public class AsTableLog
    {
        public Guid id { get; set; }
        public string msg { get; set; }
        public DateTime createtime { get; set; }
    }
}
