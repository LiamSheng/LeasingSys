using LeasingSys_API.Data;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace LeasingSys_API;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.
        // 1. 从 appsettings.json 中读取连接字符串
        var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

        // 2. 添加 DbContext 服务，并配置它使用 SQLite
        builder.Services.AddDbContext<ApplicationDbContext>(options =>
            options.UseSqlite(connectionString)
        );

        // 如果客户端通过 Accept 请求头请求一个我们的 API 无法生成的数据格式，
        // 那么服务器将严格地返回一个 406 Not Acceptable 错误，而不是“自作主张”地返回一个默认格式（如 JSON）的数据.
        builder.Services.AddControllers(option => { option.ReturnHttpNotAcceptable = true; }
        ).AddNewtonsoftJson().AddXmlDataContractSerializerFormatters();
        // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi

        builder.Services.AddSwaggerGen();

        var app = builder.Build();

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseHttpsRedirection();

        app.UseAuthorization();


        app.MapControllers();

        app.Run();
    }
}