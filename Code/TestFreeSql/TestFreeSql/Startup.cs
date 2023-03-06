using Db.Entities;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TestFreeSql
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {

            services.AddControllers();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "TestFreeSql", Version = "v1" });
            });


            Func<IServiceProvider, IFreeSql> fsql = r =>
            {
                IFreeSql fsql = new FreeSql.FreeSqlBuilder()
                    .UseConnectionString(FreeSql.DataType.MySql, @"Data Source=localhost;Database=testfreesql;User ID=root;Password=123456;pooling=true;port=3306;sslmode=none;CharSet=utf8;")
                    .UseMonitorCommand(cmd => Console.WriteLine($"Sql��{cmd.CommandText}"))//����SQL���
                    // .UseSlave(@"Data Source = localhost; Database = testfreesql1; User ID = root; Password = 123456; pooling = true; port = 3306; sslmode = none; CharSet = utf8; ", @"Data Source=localhost;Database=testfreesql2;User ID=root;Password=123456;pooling=true;port=3306;sslmode=none;CharSet=utf8;")
                    .UseAutoSyncStructure(true) //�Զ�ͬ��ʵ��ṹ�����ݿ⣬FreeSql����ɨ����򼯣�ֻ��CRUDʱ�Ż����ɱ�
                    .Build();
                return fsql;
            };
            services.AddSingleton<IFreeSql>(fsql);

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "TestFreeSql v1"));
            }

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

            //����Ŀ����ʱ���������л�ȡIFreeSqlʵ������ִ��һЩ������ͬ������������,FluentAPI��
            using (IServiceScope serviceScope = app.ApplicationServices.CreateScope())
            {
                var fsql = serviceScope.ServiceProvider.GetRequiredService<IFreeSql>();
                fsql.CodeFirst.SyncStructure(typeof(Blog), typeof(User), typeof(AsTableLog));//Topic ΪҪͬ����ʵ����
            }

        }
    }
}
