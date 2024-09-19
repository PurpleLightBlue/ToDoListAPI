
using Microsoft.EntityFrameworkCore;
using ToDoList.Application.Services;
using ToDoList.Domain.Interfaces;
using ToDoList.Infrastructure;
using ToDoList.Infrastructure.Repositories;

namespace ToDoList.Api
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers();

            // Register the DbContext (SQLite)
            builder.Services.AddDbContext<AppDbContext>(options =>
                options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

            // Register the repository (infrastructure layer)
            builder.Services.AddScoped<IToDoRepository, ToDoRepository>();

            // Register the service (application layer)
            builder.Services.AddScoped<IToDoService, ToDoService>();

            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
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
}
