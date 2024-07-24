

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration.AzureAppConfiguration;
using Microsoft.Extensions.Logging.Configuration;
using Microsoft.Extensions.Primitives;
using System.Diagnostics;
using TestRepo.Controllers;
using TestRepo.Services;
using static System.Net.WebRequestMethods;

namespace TestRepo
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            string env = String.IsNullOrEmpty(Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")) ? "Development" : Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Unknown";
            
            // Add services to the container.

            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();
            builder.Services.AddHealthChecks();

            builder.Services.AddAzureAppConfiguration();
            builder.Configuration.AddAzureAppConfiguration(options =>
            {
                //options.Connect("Endpoint=https://lab-app-configuration-gs.azconfig.io;Id=gp7b-l0-s0:6x6DZDWK2D0UC69eJRY5;Secret=FCCzIpeiCfbHsYI395NS6ZicfPDrD5nMnxWULZbprgs=")
                options.Connect("Endpoint=https://appconf-poc-lab.azconfig.io;Id=eZkE;Secret=7FPyoHb4B1allfvA43nGmq4xJb4JYjMupdOJIGgIstgQzlzlnG9wJQQJ99AGACYeBjFWLoKKAAABAZAClb3v")
                .Select("TestRepo:*", LabelFilter.Null)
                .ConfigureRefresh(refreshOptions =>
                {
                    refreshOptions.Register("TestRepo:Refresh", refreshAll: true);
                });
            });

            builder.Services.Configure<Settings>(builder.Configuration.GetSection("TestRepo:Settings"));

            builder.Services.AddDbContext<ApplicationDbContext>(
                options => {
                    options.UseSqlServer(builder.Configuration.GetValue<string>("TestRepo:Db:ConnectionString"));
                    //options.UseSqlServer("Data Source=.;Initial Catalog=TSA;Integrated Security=SSPI;TrustServerCertificate=True");
                    options.LogTo(message => {
                        Debug.WriteLine(message);
                    });
                }
             ); 

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            //if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseAzureAppConfiguration();

            app.MapHealthChecks("/health");

            app.UseAuthorization();

            app.MapControllers();

            app.Run();
        }
    }
}
