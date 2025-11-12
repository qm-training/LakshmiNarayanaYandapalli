namespace WelfareWorkTrackerAuth.Infrastructure.Data;

public partial class WelfareWorkTrackerContext : DbContext
{
    public WelfareWorkTrackerContext()
    {
    }

    public WelfareWorkTrackerContext(DbContextOptions<WelfareWorkTrackerContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Constituency> Constituencies { get; set; }

    public virtual DbSet<EmailOutbox> EmailOutboxes { get; set; }

    public virtual DbSet<EmailPlaceholder> EmailPlaceholders { get; set; }

    public virtual DbSet<EmailTemplate> EmailTemplates { get; set; }

    public virtual DbSet<Role> Roles { get; set; }

    public virtual DbSet<User> Users { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {

        // ---------------- Constituency ----------------
        modelBuilder.Entity<Constituency>(entity =>
        {
            entity.HasKey(e => e.ConstituencyId).HasName("PK__Constitu__AD6DB4AF");
            entity.ToTable("Constituency");

            entity.Property(e => e.ConstituencyName)
                .HasMaxLength(255)
                .IsUnicode(false);

            entity.Property(e => e.DistrictName)
                .HasMaxLength(150)
                .IsUnicode(false);

            entity.Property(e => e.StateName)
                .HasMaxLength(150)
                .IsUnicode(false);

            entity.Property(e => e.CountryName)
                .HasMaxLength(150)
                .IsUnicode(false);
        });

        // ---------------- Role ----------------
        modelBuilder.Entity<Role>(entity =>
        {
            entity.HasKey(e => e.RoleId).HasName("PK__Role__8AFACE1A");
            entity.ToTable("Role");

            entity.Property(e => e.RoleName)
                .HasMaxLength(100)
                .IsUnicode(false);
        });

        // ---------------- User ----------------
        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.UserId).HasName("PK__User__1788CC4C");
            entity.ToTable("User");

            entity.HasIndex(e => e.Email).IsUnique();

            entity.Property(e => e.FullName)
                .HasMaxLength(255)
                .IsUnicode(false);

            entity.Property(e => e.Email)
                .HasMaxLength(255)
                .IsUnicode(false);

            entity.Property(e => e.Address)
                .HasMaxLength(255)
                .IsUnicode(false);

            entity.Property(e => e.Gender)
                .HasMaxLength(50)
                .IsUnicode(false);

            entity.Property(e => e.PasswordHash).HasMaxLength(500);
            entity.Property(e => e.PasswordSalt).HasMaxLength(500);
            entity.Property(e => e.RefreshToken).HasMaxLength(500);

            entity.Property(e => e.RefreshTokenExpiry).HasColumnType("datetime");

            entity.Property(e => e.IsActive).HasDefaultValue(true);

            // default reputation (adjust if you prefer 0)
            entity.Property(e => e.Reputation).HasDefaultValue(100.0);

            entity.Property(e => e.DateCreated)
                .HasColumnType("datetime")
                .HasDefaultValueSql("(getutcdate())");

            entity.Property(e => e.DateUpdated)
                .HasColumnType("datetime")
                .HasDefaultValueSql("(getutcdate())");

            entity.HasOne<Role>()
                .WithMany()
                .HasForeignKey(e => e.RoleId)
                .OnDelete(DeleteBehavior.NoAction)
                .HasConstraintName("FK_User_Role");

            entity.HasOne<Constituency>()
                .WithMany()
                .HasForeignKey(e => e.ConstituencyId)
                .OnDelete(DeleteBehavior.NoAction)
                .HasConstraintName("FK_User_Constituency");
        });

        // ---------------- EmailOutbox ----------------
        modelBuilder.Entity<EmailOutbox>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__EmailOut__3214EC07");
            entity.ToTable("EmailOutbox");

            entity.Property(e => e.ToEmail)
                .HasMaxLength(256);

            entity.Property(e => e.FromEmail)
                .HasMaxLength(256);

            entity.Property(e => e.SentAt)
                .HasColumnType("datetime");

            entity.HasOne<EmailTemplate>()
                .WithMany()
                .HasForeignKey(e => e.EmailTemplateId)
                .OnDelete(DeleteBehavior.NoAction)
                .HasConstraintName("FK_EmailOutbox_Template");
        });

        // ---------------- EmailPlaceholder ----------------
        modelBuilder.Entity<EmailPlaceholder>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__EmailPla__3214EC07");
            entity.ToTable("EmailPlaceholder");

            entity.Property(e => e.PlaceHolderKey)
                .HasMaxLength(150);

            entity.Property(e => e.PlaceHolderValue)
                .HasColumnType(nameof(SqlDbType.VarChar))
                .HasMaxLength(4000);

            entity.Property(e => e.DateCreated)
                .HasColumnType("datetime")
                .HasDefaultValueSql("(getutcdate())");

            entity.Property(e => e.DateUpdated)
                .HasColumnType("datetime");

            entity.HasOne<EmailOutbox>()
                .WithMany()
                .HasForeignKey(e => e.EmailOutboxId)
                .OnDelete(DeleteBehavior.NoAction)
                .HasConstraintName("FK_EmailPlaceholder_Outbox");
        });

        // ---------------- EmailTemplate ----------------
        modelBuilder.Entity<EmailTemplate>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__EmailTem__3214EC07");
            entity.ToTable("EmailTemplate");

            entity.Property(e => e.Name)
                .HasMaxLength(150);

            entity.Property(e => e.Subject)
                .HasMaxLength(300);

            entity.Property(e => e.Body)
                .HasColumnType(nameof(SqlDbType.VarChar))
                .HasMaxLength(4000);

            entity.Property(e => e.IsActive).IsRequired();

            entity.Property(e => e.DateCreated)
                .HasColumnType("datetime")
                .HasDefaultValueSql("(getutcdate())");

            entity.Property(e => e.DateUpdated)
                .HasColumnType("datetime");

            entity.HasOne<User>()
                .WithMany()
                .HasForeignKey(e => e.CreatedBy)
                .OnDelete(DeleteBehavior.NoAction)
                .HasConstraintName("FK_EmailTemplate_User");
        });
    }

}
