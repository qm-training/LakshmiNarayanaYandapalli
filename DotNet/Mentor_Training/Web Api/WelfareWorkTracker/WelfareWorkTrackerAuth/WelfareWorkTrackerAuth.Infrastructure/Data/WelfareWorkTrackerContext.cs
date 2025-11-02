namespace WelfareWorkTrackerAuth.Infrastructure.Data;
public partial class WelfareWorkTrackerContext : DbContext
{
    public WelfareWorkTrackerContext() { }

    public WelfareWorkTrackerContext(DbContextOptions<WelfareWorkTrackerContext> options)
        : base(options) { }

    public virtual DbSet<Role> Roles { get; set; } = null!;
    public virtual DbSet<User> Users { get; set; } = null!;
    public virtual DbSet<Constituency> Constituencies { get; set; } = null!;


    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Role>(entity =>
        {
            entity.HasKey(e => e.RoleId).HasName("PK__Role__8AFACE1AD5D4D3F9");
            entity.ToTable("Role");

            entity.HasIndex(e => e.RoleName, "UQ__Role__8A2B616008F2F009").IsUnique();

            entity.Property(e => e.RoleName)
                  .HasMaxLength(50);
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.UserId).HasName("PK__User__1788CC4C5AE7B62D");
            entity.ToTable("User");

            entity.HasIndex(e => e.MobileNumber, "UQ__User__250375B146371D31").IsUnique();
            entity.HasIndex(e => e.Email, "UQ__User__A9D1053452781F4D").IsUnique();

            entity.Property(e => e.FullName).HasMaxLength(100);
            entity.Property(e => e.Email).HasMaxLength(150);
            entity.Property(e => e.Gender).HasMaxLength(10);
            entity.Property(e => e.Address).HasMaxLength(255);
            entity.Property(e => e.ConstituencyName).HasMaxLength(150);
            entity.Property(e => e.RoleName).HasMaxLength(100);
            entity.Property(e => e.Reputation).HasDefaultValue(100.0);
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.RefreshTokenExpiry).HasColumnType("datetime");

            entity.Property(e => e.DateCreated)
                  .HasColumnType("datetime")
                  .HasDefaultValueSql("(getutcdate())");

            entity.Property(e => e.DateUpdated)
                  .HasColumnType("datetime")
                  .HasDefaultValueSql("(getutcdate())");

            // Relationship to Constituency (no navigations)
            entity.HasOne<Constituency>()
                  .WithMany()
                  .HasForeignKey(e => e.ConstituencyId)
                  .OnDelete(DeleteBehavior.ClientSetNull)
                  .HasConstraintName("FK_User_Constituency");

            // Relationship to Role (no navigations)
            entity.HasOne<Role>()
                  .WithMany()
                  .HasForeignKey(e => e.RoleId)
                  .OnDelete(DeleteBehavior.ClientSetNull)
                  .HasConstraintName("FK_User_Role");
        });

        modelBuilder.Entity<Constituency>(entity =>
        {
            entity.HasKey(e => e.ConstituencyId).HasName("PK__Constitu__AD6DB4AF76DBCE07");

            entity.ToTable("Constituency");

            entity.HasIndex(e => e.ConstituencyName, "UQ__Constitu__00348CCEEF87025D").IsUnique();

            entity.Property(e => e.ConstituencyName).HasMaxLength(100);
            entity.Property(e => e.CountryName).HasMaxLength(100);
            entity.Property(e => e.DistrictName).HasMaxLength(100);
            entity.Property(e => e.StateName).HasMaxLength(100);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
