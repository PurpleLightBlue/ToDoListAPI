
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
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

            // Configure logging to console
            builder.Logging.ClearProviders();
            builder.Logging.AddConsole();

            // Add services to the container
            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowSpecificOrigins",
                    builder => builder
                        .WithOrigins("http://localhost:3000")
                        .AllowAnyMethod()   // Allows any HTTP method (GET, POST, etc.)
                        .AllowAnyHeader()); // Allows any headers
            });

            // Add services to the container.
            builder.Services.AddControllers();

            // Register the DbContext (SQLite)
            builder.Services.AddDbContext<AppDbContext>(options =>
                options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

            // Register the repository (infrastructure layer)
            builder.Services.AddScoped<IToDoRepository, ToDoRepository>();


            // Get cache expiration value from configuration
            var cacheExpiration = builder.Configuration.GetValue<int>("CacheSettings:ExpirationInMinutes");

            // Register ToDoService (application layer) with the cache expiration setting
            builder.Services.AddScoped<IToDoService>(sp =>
            {
                var toDoRepository = sp.GetRequiredService<IToDoRepository>();
                var memoryCache = sp.GetRequiredService<IMemoryCache>();
                return new ToDoService(toDoRepository, memoryCache, cacheExpiration);
            });

            // Add MemoryCache to the DI container
            builder.Services.AddMemoryCache();

            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            var app = builder.Build();

            // Enable CORS policy globally
            app.UseCors("AllowSpecificOrigins");

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
