using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Configuration;

namespace VKTask
{
    public class UsersDbContext : DbContext
    {
        public DbSet<User> User { get; set; } = null!;
        public DbSet<UserGroup> UserGroup { get; set; } = null!;
        public DbSet<UserState> UserState { get; set; } = null!;

        public UsersDbContext()
        {
            Database.EnsureCreated();
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.LogTo(System.Console.WriteLine, LogLevel.Information);
            optionsBuilder.UseNpgsql(ConfigurationManager.AppSettings.Get("SqlServer"));
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>().HasOne(t => t.UserGroup).WithMany().HasForeignKey(t => t.UserGroupId);
            modelBuilder.Entity<User>().HasOne(t => t.UserState).WithMany().HasForeignKey(t => t.UserStateId);
            modelBuilder.Entity<User>().HasKey(t => t.Id);
            modelBuilder.Entity<UserGroup>(t => t.HasCheckConstraint("CK_user_group_code", "code = \'Admin\' OR code = \'User\'"));
            modelBuilder.Entity<UserState>(t => t.HasCheckConstraint("CK_user_state_code", "code = \'Active\' OR code = \'Blocked\'"));
            modelBuilder.Entity<UserGroup>().HasKey(t => t.Id);
            modelBuilder.Entity<UserState>().HasKey(t => t.Id);
            modelBuilder.Entity<UserState>().HasData(new UserState[] { new UserState { Id = 1, Code = "Active" }, new UserState { Id = 2, Code = "Blocked" } });
            modelBuilder.Entity<UserGroup>().HasData(new UserGroup[] { new UserGroup { Id = 1, Code = "User" }, new UserGroup { Id = 2, Code = "Admin" } });
            base.OnModelCreating(modelBuilder);
        }
    }

}
