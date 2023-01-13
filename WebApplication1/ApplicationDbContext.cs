using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using WebApplication1.Models;

namespace WebApplication1
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions options) : base(options)
        {
            AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
            AppContext.SetSwitch("Npgsql.DisableDateTimeInfinityConversions", true);
        }

        public DbSet<User> Users { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            //Seed Users Table

            var user = new User
            {
                Id = 1,
                Status = 1,
                FullName = "Admin",
                Email = "admin@admin.com",
                Password = "admin",
                CreateDate = DateTime.UtcNow
            };

            PasswordHasher<User>  hasher = new PasswordHasher<User>();
            user.Password = hasher.HashPassword(user, user.Password);

            modelBuilder.Entity<User>().HasData(user);
        }
    }
}
