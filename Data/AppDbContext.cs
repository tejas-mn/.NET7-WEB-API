using Microsoft.EntityFrameworkCore;

namespace asp_net_web_api.API.Models
{
    public class AppDbContext : DbContext
    {
        
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options){
            this.ChangeTracker.LazyLoadingEnabled = false;
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Category>()
            .HasMany(c => c.Products)
            .WithOne(a => a.Category)
            .HasForeignKey(a => a.CategoryId);
            
            modelBuilder.Entity<Product>()
            .HasOne(c=>c.Category)
            .WithMany(c => c.Products)
            .HasForeignKey(c => c.CategoryId);
            
            modelBuilder.Entity<UserRole>()
            .HasKey(d => new { d.UserId , d.RoleId} );

            modelBuilder.Entity<UserRole>()
            .HasOne(c => c.User)
            .WithMany(c => c.UserRoles)
            .HasForeignKey(c => c.UserId);

            modelBuilder.Entity<UserRole>()
            .HasOne(ur => ur.Role)
            .WithMany(r => r.UserRoles)
            .HasForeignKey(ur => ur.RoleId);

            modelBuilder.Entity<RolePermission>()
            .HasKey(d => new { d.RoleId, d.PermissionId} );

            modelBuilder.Entity<RolePermission>()
            .HasOne(r => r.Role)
            .WithMany(r => r.RolePermissions)
            .HasForeignKey(c=>c.RoleId);

            modelBuilder.Entity<RolePermission>()
            .HasOne(r => r.Permission)
            .WithMany(r => r.RolePermissions)
            .HasForeignKey(c=>c.PermissionId);
            
            modelBuilder.Seed();
        }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            // optionsBuilder.EnableSensitiveDataLogging();
        }

        public DbSet<Product> Products { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<User> Users {get; set;}
        public DbSet<Role> Roles {get;set;}
        public DbSet<Permission> Permissions {get;set;}
        public DbSet<UserRole> UserRoles {get;set;}
        public DbSet<RolePermission> RolePermissions {get;set;}

    }
    
}