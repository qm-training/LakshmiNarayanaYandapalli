using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using WelfareTracker.Core.Models;

namespace WelfareTracker.Infrastructure.Data;

public partial class WelfareTrackerContext : DbContext
{
    public WelfareTrackerContext()
    {
    }

    public WelfareTrackerContext(DbContextOptions<WelfareTrackerContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Comment> Comments { get; set; }

    public virtual DbSet<Complaint> Complaints { get; set; }

    public virtual DbSet<ComplaintFeedback> ComplaintFeedbacks { get; set; }

    public virtual DbSet<ComplaintImage> ComplaintImages { get; set; }

    public virtual DbSet<ComplaintStatus> ComplaintStatuses { get; set; }

    public virtual DbSet<Constituency> Constituencies { get; set; }

    public virtual DbSet<DailyComplaint> DailyComplaints { get; set; }

    public virtual DbSet<DailyComplaintStatus> DailyComplaintStatuses { get; set; }

    public virtual DbSet<Role> Roles { get; set; }

    public virtual DbSet<User> Users { get; set; }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Comment>(entity =>
        {
            entity.HasKey(e => e.CommentId).HasName("PK__Comments__C3B4DFCACEFE8573");

            entity.Property(e => e.DateCreated)
                .HasDefaultValueSql("(getutcdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.DateUpdated)
                .HasDefaultValueSql("(getutcdate())")
                .HasColumnType("datetime");

            entity.HasOne(d => d.Complaint).WithMany(p => p.Comments)
                .HasForeignKey(d => d.ComplaintId)
                .HasConstraintName("FK_Comments_Complaint");

            entity.HasOne(d => d.DailyComplaint).WithMany(p => p.Comments)
                .HasForeignKey(d => d.DailyComplaintId)
                .HasConstraintName("FK_Comments_DailyComplaint");

            entity.HasOne(d => d.User).WithMany(p => p.Comments)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Comments_User");
        });

        modelBuilder.Entity<Complaint>(entity =>
        {
            entity.HasKey(e => e.ComplaintId).HasName("PK__Complain__740D898FAA10BD9D");

            entity.ToTable("Complaint");

            entity.Property(e => e.Attempts).HasDefaultValue(1);
            entity.Property(e => e.DateCreated)
                .HasDefaultValueSql("(getutcdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.DateUpdated)
                .HasDefaultValueSql("(getutcdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.OpenedDate).HasColumnType("datetime");
            entity.Property(e => e.Status).HasDefaultValue(1);

            entity.HasOne(d => d.Citizen).WithMany(p => p.ComplaintCitizens)
                .HasForeignKey(d => d.CitizenId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Complaint_Citizen");

            entity.HasOne(d => d.Constituency).WithMany(p => p.Complaints)
                .HasForeignKey(d => d.ConstituencyId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Complaint_Constituency");

            entity.HasOne(d => d.Leader).WithMany(p => p.ComplaintLeaders)
                .HasForeignKey(d => d.LeaderId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Complaint_Leader");
        });

        modelBuilder.Entity<ComplaintFeedback>(entity =>
        {
            entity.HasKey(e => e.CitizenFeedbackId).HasName("PK__Complain__056FD829FBCC567B");

            entity.ToTable("ComplaintFeedback");

            entity.Property(e => e.DateCreated)
                .HasDefaultValueSql("(getutcdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.DateUpdated)
                .HasDefaultValueSql("(getutcdate())")
                .HasColumnType("datetime");

            entity.HasOne(d => d.Citizen).WithMany(p => p.ComplaintFeedbacks)
                .HasForeignKey(d => d.CitizenId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ComplaintFeedback_User");

            entity.HasOne(d => d.Complaint).WithMany(p => p.ComplaintFeedbacks)
                .HasForeignKey(d => d.ComplaintId)
                .HasConstraintName("FK_ComplaintFeedback_Complaint");

            entity.HasOne(d => d.DailyComplaint).WithMany(p => p.ComplaintFeedbacks)
                .HasForeignKey(d => d.DailyComplaintId)
                .HasConstraintName("FK_ComplaintFeedback_DailyComplaint");
        });

        modelBuilder.Entity<ComplaintImage>(entity =>
        {
            entity.HasKey(e => e.ComplaintImageId).HasName("PK__Complain__F5DE709DBB943C29");

            entity.ToTable("ComplaintImage");

            entity.HasIndex(e => e.ImageUrl, "UQ__Complain__372DE2C558BA7383").IsUnique();

            entity.Property(e => e.DateCreated)
                .HasDefaultValueSql("(getutcdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.DateUpdated)
                .HasDefaultValueSql("(getutcdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.ImageUrl)
                .HasMaxLength(255)
                .HasColumnName("ImageURL");

            entity.HasOne(d => d.Complaint).WithMany(p => p.ComplaintImages)
                .HasForeignKey(d => d.ComplaintId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ComplaintImage_Complaint");
        });

        modelBuilder.Entity<ComplaintStatus>(entity =>
        {
            entity.HasKey(e => e.ComplaintStatusId).HasName("PK__Complain__A5FEB61E8266377F");

            entity.ToTable("ComplaintStatus");

            entity.Property(e => e.AttemptNumber).HasDefaultValue(1);
            entity.Property(e => e.DateCreated)
                .HasDefaultValueSql("(getutcdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.DeadlineDate).HasColumnType("datetime");
            entity.Property(e => e.RejectReason).HasMaxLength(255);

            entity.HasOne(d => d.Complaint).WithMany(p => p.ComplaintStatuses)
                .HasForeignKey(d => d.ComplaintId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ComplaintStatus_Complaint");
        });

        modelBuilder.Entity<Constituency>(entity =>
        {
            entity.HasKey(e => e.ConstituencyId).HasName("PK__Constitu__AD6DB4AF4EE53786");

            entity.ToTable("Constituency");

            entity.HasIndex(e => e.ConstituencyName, "UQ__Constitu__00348CCE656B9B58").IsUnique();

            entity.Property(e => e.ConstituencyName).HasMaxLength(100);
            entity.Property(e => e.CountryName).HasMaxLength(100);
            entity.Property(e => e.DistrictName).HasMaxLength(100);
            entity.Property(e => e.StateName).HasMaxLength(100);
        });

        modelBuilder.Entity<DailyComplaint>(entity =>
        {
            entity.HasKey(e => e.DailyComplaintId).HasName("PK__DailyCom__BE8C98A3892427FF");

            entity.ToTable("DailyComplaint");

            entity.Property(e => e.DateCreated)
                .HasDefaultValueSql("(getutcdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.DateUpdated)
                .HasDefaultValueSql("(getutcdate())")
                .HasColumnType("datetime");

            entity.HasOne(d => d.Constituency).WithMany(p => p.DailyComplaints)
                .HasForeignKey(d => d.ConstituencyId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_DailyComplaint_Constituency");

            entity.HasOne(d => d.Leader).WithMany(p => p.DailyComplaints)
                .HasForeignKey(d => d.LeaderId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_DailyComplaint_Leader");
        });

        modelBuilder.Entity<DailyComplaintStatus>(entity =>
        {
            entity.HasKey(e => e.DailyComplaintStatusId).HasName("PK__DailyCom__0FA8065F07C2404E");

            entity.ToTable("DailyComplaintStatus");

            entity.Property(e => e.DateCreated)
                .HasDefaultValueSql("(getutcdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.DateUpdated)
                .HasDefaultValueSql("(getutcdate())")
                .HasColumnType("datetime");

            entity.HasOne(d => d.DailyComplaint).WithMany(p => p.DailyComplaintStatuses)
                .HasForeignKey(d => d.DailyComplaintId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_DailyComplaintStatus_DailyComplaint");
        });

        modelBuilder.Entity<Role>(entity =>
        {
            entity.HasKey(e => e.RoleId).HasName("PK__Role__8AFACE1A229E97B0");

            entity.ToTable("Role");

            entity.HasIndex(e => e.RoleName, "UQ__Role__8A2B6160730394DF").IsUnique();

            entity.Property(e => e.RoleName).HasMaxLength(50);
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.UserId).HasName("PK__User__1788CC4C983A234A");

            entity.ToTable("User");

            entity.HasIndex(e => e.MobileNumber, "UQ__User__250375B107801990").IsUnique();

            entity.HasIndex(e => e.Email, "UQ__User__A9D10534F9B57434").IsUnique();

            entity.Property(e => e.Address).HasMaxLength(255);
            entity.Property(e => e.ConstituencyName).HasMaxLength(150);
            entity.Property(e => e.DateCreated)
                .HasDefaultValueSql("(getutcdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.DateUpdated)
                .HasDefaultValueSql("(getutcdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Email).HasMaxLength(150);
            entity.Property(e => e.FullName).HasMaxLength(100);
            entity.Property(e => e.Gender).HasMaxLength(10);
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.RefreshTokenExpiry).HasColumnType("datetime");
            entity.Property(e => e.Reputation).HasDefaultValue(100.0);
            entity.Property(e => e.RoleName).HasMaxLength(100);

            entity.HasOne(d => d.Constituency).WithMany(p => p.Users)
                .HasForeignKey(d => d.ConstituencyId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_User_Constituency");

            entity.HasOne(d => d.Role).WithMany(p => p.Users)
                .HasForeignKey(d => d.RoleId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_User_Role");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
