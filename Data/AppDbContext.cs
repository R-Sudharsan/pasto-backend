using Microsoft.EntityFrameworkCore;
using System;

namespace pasto_backend.Data
{
    public class AppDbContext : DbContext
    {
        public DbSet<Paste> Pastes { get; set; }
        public DbSet<User> Users { get; set; } // Make sure you have this line

        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }
    }
}
