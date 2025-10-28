using System;
using Microsoft.EntityFrameworkCore;
using WelfareWorkTracker.Core.Models;

namespace WelfareWorkTracker.Infrastructure.Data
{
    public partial class WelfareWorkTrackerContext : DbContext
    {
        public WelfareWorkTrackerContext() { }

        public WelfareWorkTrackerContext(DbContextOptions<WelfareWorkTrackerContext> options)
            : base(options) { }

        public virtual DbSet<Comment> Comments { get; set; }
        public virtual DbSet<Complaint> Complaints { get; set; }
        public virtual DbSet<ComplaintFeedback> ComplaintFeedbacks { get; set; }
        public virtual DbSet<ComplaintImage> ComplaintImages { get; set; }
        public virtual DbSet<ComplaintStatus> ComplaintStatuses { get; set; }
        public virtual DbSet<Constituency> Constituencies { get; set; }
        public virtual DbSet<DailyComplaint> DailyComplaints { get; set; }
        public virtual DbSet<DailyComplaintStatus> DailyComplaintStatuses { get; set; }
        public virtual DbSet<Leader> Leaders { get; set; }
        public virtual DbSet<Role> Roles { get; set; }
        public virtual DbSet<User> Users { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // -------------------------
            // Comment
            // -------------------------
            modelBuilder.Entity<Comment>(entity =>
            {
                entity.HasKey(e => e.CommentId).HasName("PK__Comments__C3B4DFCAB2A04424");

                entity.Property(e => e.DateCreated)
                      .HasColumnType("datetime")
                      .HasDefaultValueSql("(getutcdate())");

                entity.Property(e => e.DateUpdated)
                      .HasColumnType("datetime")
                      .HasDefaultValueSql("(getutcdate())");

                entity.HasOne<User>()
                      .WithMany()
                      .HasForeignKey(e => e.UserId)
                      .OnDelete(DeleteBehavior.ClientSetNull)
                      .HasConstraintName("FK_Comments_User");

                entity.HasOne<Complaint>()
                      .WithMany()
                      .HasForeignKey(e => e.ComplaintId)
                      .HasConstraintName("FK_Comments_Complaint");

                entity.HasOne<DailyComplaint>()
                      .WithMany()
                      .HasForeignKey(e => e.DailyComplaintId)
                      .HasConstraintName("FK_Comments_DailyComplaint");
            });

            // -------------------------
            // Complaint
            // -------------------------
            modelBuilder.Entity<Complaint>(entity =>
            {
                entity.HasKey(e => e.ComplaintId).HasName("PK__Complain__740D898F73E03E5F");
                entity.ToTable("Complaint");

                entity.Property(e => e.Attempts).HasDefaultValue(1);
                entity.Property(e => e.Status).HasDefaultValue(1);
                entity.Property(e => e.OpenedDate).HasColumnType("datetime");

                entity.Property(e => e.DateCreated)
                      .HasColumnType("datetime")
                      .HasDefaultValueSql("(getutcdate())");

                entity.Property(e => e.DateUpdated)
                      .HasColumnType("datetime")
                      .HasDefaultValueSql("(getutcdate())");

                entity.HasOne<User>()
                      .WithMany()
                      .HasForeignKey(e => e.CitizenId)
                      .OnDelete(DeleteBehavior.ClientSetNull)
                      .HasConstraintName("FK_Complaint_Citizen");

                entity.HasOne<Constituency>()
                      .WithMany()
                      .HasForeignKey(e => e.ConstituencyId)
                      .OnDelete(DeleteBehavior.ClientSetNull)
                      .HasConstraintName("FK_Complaint_Constituency");

                entity.HasOne<User>()
                      .WithMany()
                      .HasForeignKey(e => e.LeaderId)
                      .OnDelete(DeleteBehavior.ClientSetNull)
                      .HasConstraintName("FK_Complaint_Leader");
            });

            // -------------------------
            // ComplaintFeedback
            // -------------------------
            modelBuilder.Entity<ComplaintFeedback>(entity =>
            {
                entity.HasKey(e => e.CitizenFeedbackId).HasName("PK__Complain__056FD82994775A59");
                entity.ToTable("ComplaintFeedback");

                entity.Property(e => e.DateCreated)
                      .HasColumnType("datetime")
                      .HasDefaultValueSql("(getutcdate())");

                entity.Property(e => e.DateUpdated)
                      .HasColumnType("datetime")
                      .HasDefaultValueSql("(getutcdate())");

                entity.HasOne<User>()
                      .WithMany()
                      .HasForeignKey(e => e.CitizenId)
                      .OnDelete(DeleteBehavior.ClientSetNull)
                      .HasConstraintName("FK_ComplaintFeedback_User");

                entity.HasOne<Complaint>()
                      .WithMany()
                      .HasForeignKey(e => e.ComplaintId)
                      .HasConstraintName("FK_ComplaintFeedback_Complaint");

                entity.HasOne<DailyComplaint>()
                      .WithMany()
                      .HasForeignKey(e => e.DailyComplaintId)
                      .HasConstraintName("FK_ComplaintFeedback_DailyComplaint");
            });

            // -------------------------
            // ComplaintImage
            // -------------------------
            modelBuilder.Entity<ComplaintImage>(entity =>
            {
                entity.HasKey(e => e.ComplaintImageId).HasName("PK__Complain__F5DE709DFFCF2820");
                entity.ToTable("ComplaintImage");

                entity.HasIndex(e => e.ImageUrl, "UQ__Complain__372DE2C5EAC554F0").IsUnique();

                entity.Property(e => e.ImageUrl)
                      .HasMaxLength(255)
                      .HasColumnName("ImageURL");

                entity.Property(e => e.DateCreated)
                      .HasColumnType("datetime")
                      .HasDefaultValueSql("(getutcdate())");

                entity.Property(e => e.DateUpdated)
                      .HasColumnType("datetime")
                      .HasDefaultValueSql("(getutcdate())");

                entity.HasOne<Complaint>()
                      .WithMany()
                      .HasForeignKey(e => e.ComplaintId)
                      .OnDelete(DeleteBehavior.ClientSetNull)
                      .HasConstraintName("FK_ComplaintImage_Complaint");
            });

            // -------------------------
            // ComplaintStatus
            // -------------------------
            modelBuilder.Entity<ComplaintStatus>(entity =>
            {
                entity.HasKey(e => e.ComplaintStatusId).HasName("PK__Complain__A5FEB61E7E9D868D");
                entity.ToTable("ComplaintStatus");

                entity.Property(e => e.AttemptNumber).HasDefaultValue(1);
                entity.Property(e => e.DeadlineDate).HasColumnType("datetime");
                entity.Property(e => e.RejectReason).HasMaxLength(255);

                entity.Property(e => e.DateCreated)
                      .HasColumnType("datetime")
                      .HasDefaultValueSql("(getutcdate())");

                entity.HasOne<Complaint>()
                      .WithMany()
                      .HasForeignKey(e => e.ComplaintId)
                      .OnDelete(DeleteBehavior.ClientSetNull)
                      .HasConstraintName("FK_ComplaintStatus_Complaint");
            });

            // -------------------------
            // Constituency
            // -------------------------
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

            // -------------------------
            // DailyComplaint
            // -------------------------
            modelBuilder.Entity<DailyComplaint>(entity =>
            {
                entity.HasKey(e => e.DailyComplaintId).HasName("PK__DailyCom__BE8C98A3D3C67E15");
                entity.ToTable("DailyComplaint");

                entity.Property(e => e.DateCreated)
                      .HasColumnType("datetime")
                      .HasDefaultValueSql("(getutcdate())");

                entity.Property(e => e.DateUpdated)
                      .HasColumnType("datetime")
                      .HasDefaultValueSql("(getutcdate())");

                entity.HasOne<Constituency>()
                      .WithMany()
                      .HasForeignKey(e => e.ConstituencyId)
                      .OnDelete(DeleteBehavior.ClientSetNull)
                      .HasConstraintName("FK_DailyComplaint_Constituency");

                entity.HasOne<User>()
                      .WithMany()
                      .HasForeignKey(e => e.LeaderId)
                      .OnDelete(DeleteBehavior.ClientSetNull)
                      .HasConstraintName("FK_DailyComplaint_Leader");
            });

            // -------------------------
            // DailyComplaintStatus
            // -------------------------
            modelBuilder.Entity<DailyComplaintStatus>(entity =>
            {
                entity.HasKey(e => e.DailyComplaintStatusId).HasName("PK__DailyCom__0FA8065FD645D6E3");
                entity.ToTable("DailyComplaintStatus");

                entity.Property(e => e.DateCreated)
                      .HasColumnType("datetime")
                      .HasDefaultValueSql("(getutcdate())");

                entity.Property(e => e.DateUpdated)
                      .HasColumnType("datetime")
                      .HasDefaultValueSql("(getutcdate())");

                entity.HasOne<DailyComplaint>()
                      .WithMany()
                      .HasForeignKey(e => e.DailyComplaintId)
                      .OnDelete(DeleteBehavior.ClientSetNull)
                      .HasConstraintName("FK_DailyComplaintStatus_DailyComplaint");
            });

            // -------------------------
            // Leader
            // -------------------------
            modelBuilder.Entity<Leader>(entity =>
            {
                entity.HasKey(e => e.Id).HasName("PK__Leader__3214EC07FCA2AE70");
                entity.ToTable("Leader");

                entity.Property(e => e.PartyName).HasMaxLength(100);
                entity.Property(e => e.EndDate).HasColumnType("datetime");
                entity.Property(e => e.StartDate)
                      .HasColumnType("datetime")
                      .HasDefaultValueSql("(getutcdate())");

                entity.HasOne<User>()
                      .WithMany()
                      .HasForeignKey(e => e.UserId)
                      .OnDelete(DeleteBehavior.ClientSetNull)
                      .HasConstraintName("FK_Leader_User");
            });

            // -------------------------
            // Role
            // -------------------------
            modelBuilder.Entity<Role>(entity =>
            {
                entity.HasKey(e => e.RoleId).HasName("PK__Role__8AFACE1AD5D4D3F9");
                entity.ToTable("Role");

                entity.HasIndex(e => e.RoleName, "UQ__Role__8A2B616008F2F009").IsUnique();
                entity.Property(e => e.RoleName).HasMaxLength(50);
            });

            // -------------------------
            // User
            // -------------------------
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

                entity.HasOne<Constituency>()
                      .WithMany()
                      .HasForeignKey(e => e.ConstituencyId)
                      .OnDelete(DeleteBehavior.ClientSetNull)
                      .HasConstraintName("FK_User_Constituency");

                entity.HasOne<Role>()
                      .WithMany()
                      .HasForeignKey(e => e.RoleId)
                      .OnDelete(DeleteBehavior.ClientSetNull)
                      .HasConstraintName("FK_User_Role");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
