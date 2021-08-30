using Microsoft.EntityFrameworkCore;

namespace linq2db_sample.EFCore
{
    public class DatabaseContext : DbContext
    {
        public DatabaseContext(DbContextOptions options) : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Company> Companies { get; set; }
        public DbSet<DbContract> Contracts { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Company>().ToTable("company");
            modelBuilder.Entity<Company>().Property(x => x.Id).HasColumnName("id");
            modelBuilder.Entity<Company>().Property(x => x.Name).HasColumnName("name");

            modelBuilder.Entity<User>().ToTable("users");
            modelBuilder.Entity<User>().Property(x => x.CognitoId).HasColumnName("cognito_id");
            modelBuilder.Entity<User>().Property(x => x.Id).HasColumnName("id");
            modelBuilder.Entity<User>().HasMany(x => x.Contracts)
                .WithOne(x => x.User).HasForeignKey("users_id");

            modelBuilder.Entity<Category>().ToTable("category");
            modelBuilder.Entity<Category>().Property(x => x.Id).HasColumnName("id");
            modelBuilder.Entity<Category>().Property(x => x.Name).HasColumnName("name");

            modelBuilder.Entity<DbContract>().ToTable("contract");
            modelBuilder.Entity<DbContract>().Property(x => x.Id).HasColumnName("id");
            modelBuilder.Entity<DbContract>().Property(x => x.CompanyId).HasColumnName("company_id");
            modelBuilder.Entity<DbContract>().HasOne(x => x.Category)
                .WithMany(x => x.Contracts).HasForeignKey("category_id");

        }
    }
}