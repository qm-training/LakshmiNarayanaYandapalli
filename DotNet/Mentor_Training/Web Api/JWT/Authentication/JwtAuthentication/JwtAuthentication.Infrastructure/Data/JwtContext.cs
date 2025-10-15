namespace JwtAuthentication.Infrastructure.Data;
public partial class JwtContext : DbContext
{
    public JwtContext()
    {
    }

    public JwtContext(DbContextOptions<JwtContext> options)
        : base(options)
    {
    }

    public virtual DbSet<RefreshToken> RefreshTokens { get; set; }

    public virtual DbSet<Role> Roles { get; set; }

    public virtual DbSet<User> Users { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<RefreshToken>(entity =>
        {
            entity.HasKey(e => e.Id);

            entity.HasIndex(e => e.UserId, "UQ__RefreshT__1788CC4D3A9C14E5").IsUnique();

            entity.Property(e => e.Token).HasMaxLength(500);

            entity.HasOne(d => d.User).WithOne(p => p.RefreshToken)
                .HasForeignKey<RefreshToken>(d => d.UserId);
        });

        modelBuilder.Entity<Role>(entity =>
        {
            entity.HasKey(e => e.RoleId);

            entity.HasIndex(e => e.RoleName, "UQ__Roles__8A2B6160E226BB1B").IsUnique();

            entity.Property(e => e.RoleName).HasMaxLength(100);
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.UserId);

            entity.Property(e => e.Email).HasMaxLength(150);
            entity.Property(e => e.PasswordHash).HasMaxLength(255);
            entity.Property(e => e.Salt).HasMaxLength(255);
            entity.Property(e => e.Username).HasMaxLength(100);

            entity.HasOne(d => d.Role).WithMany(p => p.Users)
                .HasForeignKey(d => d.RoleId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
