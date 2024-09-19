using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace ToDoList.Infrastructure
{
    internal class AppDbContextFactory : IDesignTimeDbContextFactory<AppDbContext>
    {
        public AppDbContext CreateDbContext(string[] args)
        {

            // Set up the DbContextOptions
            var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();

            // Assume connection string is passed as an argument during design-time migrations
            var connectionString = args.Length > 0 ? args[0] : "Data Source=ToDoApp.db";


            optionsBuilder.UseSqlite(connectionString);

            return new AppDbContext(optionsBuilder.Options);
        }
    }
}
