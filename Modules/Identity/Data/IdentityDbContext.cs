using Harc.Api.Common.Models;
using Microsoft.EntityFrameworkCore;

namespace Harc.Api.Modules.Identity.Data;

public class IdentityDbContext : DbContext
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    public IdentityDbContext(DbContextOptions<IdentityDbContext> options, IHttpContextAccessor httpContextAccessor) : base(options)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        ApplyAuditInfo();
        return base.SaveChangesAsync(cancellationToken);
    }

    public override int SaveChanges()
    {
        ApplyAuditInfo();
        return base.SaveChanges();
    }

    private void ApplyAuditInfo()
    {
        var user = _httpContextAccessor.HttpContext?.User?.FindFirst("name")?.Value ?? _httpContextAccessor.HttpContext?.User?.FindFirst("email")?.Value;
        var now = DateTime.UtcNow;

        foreach (var entry in ChangeTracker.Entries<BaseEntity>())
        {
            if (entry.State == EntityState.Added)
            {
                entry.Entity.CreatedAt = now;
                entry.Entity.CreatedBy = user;
            }
            else if (entry.State == EntityState.Modified)
            {
                entry.Entity.UpdatedAt = now;
                entry.Entity.UpdatedBy = user;
                
                entry.Property(x => x.CreatedAt).IsModified = false;
                entry.Property(x => x.CreatedBy).IsModified = false;
            }
            else if (entry.State == EntityState.Deleted)
            {
                entry.Entity.DeletedAt = now;
                entry.Entity.UpdatedBy = user;
                
                entry.Property(x => x.CreatedAt).IsModified = false;
                entry.Property(x => x.CreatedBy).IsModified = false;
                entry.Property(x => x.UpdatedAt).IsModified = false;

                entry.State = EntityState.Modified; // Soft delete
            }
        }
    }

    public DbSet<UserEntity> Users => Set<UserEntity>();
    public DbSet<RoleEntity> Roles => Set<RoleEntity>();
    public DbSet<TeamEntity> Teams => Set<TeamEntity>();
    public DbSet<TitleEntity> Titles => Set<TitleEntity>();
    public DbSet<Leave.Data.LeaveEntity> Leaves => Set<Leave.Data.LeaveEntity>();
    public DbSet<Leave.Data.LeaveSettingEntity> LeaveSettings => Set<Leave.Data.LeaveSettingEntity>();
    public DbSet<Document.Data.DocumentEntity> Documents => Set<Document.Data.DocumentEntity>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.HasDefaultSchema("identity");

        modelBuilder.Entity<RoleEntity>(entity =>
        {
            entity.Property(r => r.Id).ValueGeneratedOnAdd();
            entity.HasIndex(r => r.Name).IsUnique();
            entity.Property(r => r.DisplayName).HasColumnType("jsonb").IsRequired();
        });

        modelBuilder.Entity<TeamEntity>(entity =>
        {
            entity.Property(t => t.Id).ValueGeneratedOnAdd();
            entity.HasIndex(t => t.Name).IsUnique();
            entity.Property(t => t.DisplayName).HasColumnType("jsonb").IsRequired();
        });

        modelBuilder.Entity<TitleEntity>(entity =>
        {
            entity.Property(t => t.Id).ValueGeneratedOnAdd();
            entity.HasIndex(t => t.Name).IsUnique();
            entity.Property(t => t.DisplayName).HasColumnType("jsonb").IsRequired();
        });

        modelBuilder.Entity<UserEntity>(entity =>
        {
            entity.HasIndex(u => u.Email).IsUnique();

            entity.HasOne(u => u.Role)
                .WithMany(r => r.Users)
                .HasForeignKey(u => u.RoleId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(u => u.Team)
                .WithMany()
                .HasForeignKey(u => u.TeamId)
                .OnDelete(DeleteBehavior.SetNull);

            entity.HasOne(u => u.Title)
                .WithMany(t => t.Users)
                .HasForeignKey(u => u.TitleId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(u => u.Manager)
                .WithMany()
                .HasForeignKey(u => u.ManagerId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        //Leave entity configuration
        modelBuilder.Entity<Leave.Data.LeaveEntity>().ToTable("Leaves", "leave");
        modelBuilder.Entity<Leave.Data.LeaveSettingEntity>().ToTable("LeaveSettings", "leave");

        //Document entity configuration
        modelBuilder.Entity<Document.Data.DocumentEntity>().ToTable("Documents", "document");
    }
}