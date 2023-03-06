using Db.Entities;
using FreeSql;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TestFreeSql.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class FreeSqlController : ControllerBase
    {
        //官方文档：https://freesql.net/guide/getting-started.html


        private readonly ILogger<FreeSqlController> _logger;
        private readonly IFreeSql _freeSql;

        private readonly DbContext  _dbContext;

        private IMemoryCache _cache;
        public FreeSqlController(ILogger<FreeSqlController> logger, IFreeSql freeSql, IMemoryCache cache)
        {
            _logger = logger;
            _freeSql = freeSql;
            _dbContext = freeSql.CreateDbContext();
            _cache = cache;
        }

        /// <summary>
        /// 基础使用(增加、查询、删除、)
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public  async Task<string> TestFreeSqlBase()
        {
            //插入单一数据
            var blog = new Blog() { Rating = 1, Url = DateTime.Now.ToString() };
            var save =await _freeSql.Insert<Blog>(blog).ExecuteAffrowsAsync();
            
            //查询
            var select1 =await _freeSql.Select<Blog>().Where(x => x.BlogId == 1).ToListAsync();
            var select2 = _freeSql.Select<Blog>().ToList();


            //分页查询
            var select4 = _freeSql.Select<Blog>()
               .Where(a => a.BlogId > 1);
            var sql = select4.ToSql();
            var total = await select4.CountAsync();
            var list = await select4.Page(1, 20).ToListAsync();


            //修改
            var select3 =await _freeSql.Select<Blog>().FirstAsync();
            select3.Url = DateTime.Now.ToString();
            var save2 = await _freeSql.InsertOrUpdate<Blog>().SetSource(select3).ExecuteAffrowsAsync();


            //删除
            var delete =await _freeSql.Delete<Blog>().Where(x => x.Url == blog.Url).ExecuteAffrowsAsync();


            return "123";
        }


        /// <summary>
        /// 事务使用
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<string> TestFreeSqlDbContext1()
        {
            //工作单元
            var ctx = _dbContext;

            var blog = new Blog() { Rating = 1, Url = DateTime.Now.ToString() };
            ctx.Set<Blog>().Add(blog);
   
            var user = new User() { Name = DateTime.Now.ToString(), Age=1 };
            ctx.Set<User>().Add(user);

           var save=await ctx.SaveChangesAsync();


            return "123";
        }

        /// <summary>
        /// 事务使用
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<string> TestFreeSqlDbContext2()
        {
            //工作单元
            var ctx = _freeSql.CreateDbContext();

            var blog = new Blog() { Rating = 1, Url = DateTime.Now.ToString() };
            ctx.Set<Blog>().Add(blog);

            var user = new User() { Name = DateTime.Now.ToString(), Age = 1 };
            ctx.Set<User>().Add(user);

            var save = await ctx.SaveChangesAsync();


            return "123";
        }




        /// <summary>
        /// 读从库、写主库,伪功能（需要自己实现数据库数据同步）
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<string> TestFreeSqlReadWrite()
        {
            //文档：https://freesql.net/guide/read-write-splitting.html


            var select2 = _freeSql.Select<Blog>().Where(x=>x.BlogId>0).ToList();//读取从库

            //插入单一数据
            var blog = new Blog() { Rating = 1, Url = DateTime.Now.ToString() };
            var saveSql =await _freeSql.Insert<Blog>(blog).ExecuteAffrowsAsync();//写入主库

            var select3 = _freeSql.Select<Blog>().ToList();//读取从库

            var select4 = _freeSql.Select<Blog>().Master().ToList();//读取主库


            return "123";
        }



        /// <summary>
        /// 分表（自动分表）
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<string> TestFreeSqlSeparate()
        {
            //文档：https://github.com/dotnetcore/FreeSql/discussions/1066

            //插入单一数据
            var asTableLog = new AsTableLog() { msg=DateTime.Now.ToString(), createtime=DateTime.Now};
            var saveSql =await _freeSql.Insert<AsTableLog>(asTableLog).ExecuteAffrowsAsync();

            //插入单一数据
            var asTableLog2 = new AsTableLog() { msg = DateTime.Now.ToString(), createtime = DateTime.Now.AddDays(10) };
            var saveSql2 =await _freeSql.Insert<AsTableLog>(asTableLog2).ExecuteAffrowsAsync();


            //查询
            var select = _freeSql.Select<AsTableLog>();
                    //.Where(a => a.createtime.Between(DateTime.Parse("2022-3-1"), DateTime.Parse("2022-5-1")));
            var sql = select.ToSql();
            var list = select.ToList();

            return "123";
        }

        /// <summary>
        /// 分布式事务:执行
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<string> TestFreeSqlCloud3(string id)
        {
            var sql = _cache.Get<string>(id) ?? "";


            List<dynamic> t13 = _freeSql.Ado.Query<dynamic>(sql);

            _cache.Remove(id);

            return "123";
        }


    }
}
