using Microsoft.EntityFrameworkCore;

namespace EIIOS.Data
{
    public class DbContext : Microsoft.EntityFrameworkCore.DbContext
    {
        public DbContext(DbContextOptions<DbContext> options) : base(options) { }

        // Empty for now - you'll add DbSets when you decide on tables

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Empty for now - you'll add configurations later
            base.OnModelCreating(modelBuilder);
        }
    }
}
