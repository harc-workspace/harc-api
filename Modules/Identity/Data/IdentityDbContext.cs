using Microsoft.EntityFrameworkCore;

namespace Harc.Api.Modules.Identity.Data;

public class IdentityDbContext : DbContext
{
    public IdentityDbContext(DbContextOptions<IdentityDbContext> options) : base(options)
    {
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