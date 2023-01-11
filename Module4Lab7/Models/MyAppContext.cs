using Microsoft.EntityFrameworkCore;

namespace Module4Lab7.Models
{
    public class MyAppContext:DbContext
    {
        public DbSet<User> Users { get; set; }


        public MyAppContext(DbContextOptions<MyAppContext> options)
            : base(options)
        {
            Database.EnsureCreated();   // создаем базу данных при первом обращении
        }
    }
}
