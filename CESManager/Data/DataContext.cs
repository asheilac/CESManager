using System;
using CESManager.Models;
using Microsoft.EntityFrameworkCore;

namespace CESManager.Data
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options) { }
        public DbSet<Session> Sessions { get; set; }
        public DbSet<User> Users { get; set; }

        //protected override void OnModelCreating(ModelBuilder modelBuilder)
        //{
        //    Utility.CreatePasswordHash("123456", out byte[] passwordHash, out byte[] passwordSalt);

        //    modelBuilder.Entity<User>().HasData(
        //        new User { Id = 1, PasswordHash = passwordHash, PasswordSalt = passwordSalt, Username = "User1" },
        //        new User { Id = 2, PasswordHash = passwordHash, PasswordSalt = passwordSalt, Username = "User2" }
        //    );

        //    modelBuilder.Entity<Session>().HasData(
        //        new Session
        //        {
        //            Id = 1,
        //            StartDateTime = new DateTime(2020, 11, 19, 14, 0, 0),
        //            EndDateTime = new DateTime(2020, 11, 19, 14, 30, 0),
        //            UserId = 1
        //        },
        //        new Session
        //        {
        //            Id = 2,
        //            StartDateTime = new DateTime(2020, 11, 19, 4, 0, 0),
        //            EndDateTime = new DateTime(2020, 11, 19, 4, 30, 0),
        //            UserId = 2
        //        }
        //    );
        //}
    }
}