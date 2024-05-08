using AuthManager.Models;
using Microsoft.EntityFrameworkCore;

namespace AuthManager.Data;

public class AkashicDbContext(DbContextOptions<AkashicDbContext> options) : DbContext(options)
{
    public DbSet<Account> Accounts { get; set; }
    public DbSet<Service> Services { get; set; }
    public DbSet<Access> Accesses { get; set; }
    public DbSet<SuspensionLog> SuspensionLogs { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        #region Account
        
        modelBuilder.Entity<Account>()
            .HasIndex(a => a.Email)
            .IsUnique();
        
        modelBuilder.Entity<Account>()
            .HasIndex(a => a.Username)
            .IsUnique();

        modelBuilder.Entity<Account>()
            .Property(a => a.CreatedAt)
            .HasDefaultValueSql("(UTC_TIMESTAMP)");

        modelBuilder.Entity<Account>()
            .Property(a => a.Verified)
            .HasDefaultValue(0);
        
        modelBuilder.Entity<Account>()
            .Property(a => a.Admin)
            .HasDefaultValue(0);
        
        #endregion

        #region Service

        modelBuilder.Entity<Service>()
            .HasIndex(s => s.Name)
            .IsUnique();

        #endregion

        #region Access

        modelBuilder.Entity<Account>()
            .HasMany(a => a.Services)
            .WithMany(s => s.Accounts)
            .UsingEntity<Access>(
                r => r.HasOne<Service>(e => e.Service).WithMany(e => e.Accesses).HasForeignKey(e => e.Sid), 
                l => l.HasOne<Account>(e => e.Account).WithMany(e => e.Accesses).HasForeignKey(e => e.Uid));

        modelBuilder.Entity<Access>()
            .Property(a => a.Banned)
            .HasDefaultValue(0);
        
        modelBuilder.Entity<Access>()
            .Property(a => a.SuspensionEndAt)
            .HasDefaultValueSql("NULL");

        #endregion

        #region SuspensionLog

        modelBuilder.Entity<Account>()
            .HasMany(a => a.SuspensionLogs)
            .WithOne(s => s.AssigneeAccount)
            .HasForeignKey("AssigneeUid")
            .IsRequired();
        
        modelBuilder.Entity<Account>()
            .HasMany(a => a.ActionLogs)
            .WithOne(s => s.AssignerAccount)
            .HasForeignKey("AssignerUid")
            .IsRequired(false);
        
        modelBuilder.Entity<Service>()
            .HasMany(s => s.SuspensionLogs)
            .WithOne(s => s.Service)
            .HasForeignKey("Sid")
            .IsRequired();
        
        modelBuilder.Entity<SuspensionLog>()
            .Property(s => s.LoggedAt)
            .HasDefaultValueSql("(UTC_TIMESTAMP)");
        
        modelBuilder.Entity<SuspensionLog>()
            .Property(s => s.SuspensionEndAt)
            .HasDefaultValueSql("NULL");
        
        modelBuilder.Entity<SuspensionLog>()
            .Property(s => s.Reason)
            .HasDefaultValue(string.Empty);

        #endregion
    }
}