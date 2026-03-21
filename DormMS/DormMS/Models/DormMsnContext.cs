using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace DormMS.Models;

public partial class DormMsnContext : DbContext
{
    public DormMsnContext()
    {
    }

    public DormMsnContext(DbContextOptions<DormMsnContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Allotment> Allotments { get; set; }

    public virtual DbSet<Complaint> Complaints { get; set; }

    public virtual DbSet<Hostel> Hostels { get; set; }

    public virtual DbSet<HostelUser> HostelUsers { get; set; }

    public virtual DbSet<Inventory> Inventories { get; set; }

    public virtual DbSet<Payment> Payments { get; set; }

    public virtual DbSet<Role> Roles { get; set; }

    public virtual DbSet<Room> Rooms { get; set; }

    public virtual DbSet<Visitor> Visitors { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {

        if (!optionsBuilder.IsConfigured)
        {
            optionsBuilder.UseSqlServer(GetConnectionString());
        }
    }
    private string? GetConnectionString()
    {
        IConfiguration configuration = new ConfigurationBuilder()
         .SetBasePath(Directory.GetCurrentDirectory())
         .AddJsonFile("appsettings.json")
         .Build();

        return configuration["ConnectionStrings:MyCnn"];
    }


    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Allotment>(entity =>
        {
            entity.HasKey(e => e.AllotmentId).HasName("PK__allotmen__18DE9A9643B05D06");

            entity.ToTable("allotments");

            entity.Property(e => e.AllotmentId).HasColumnName("allotment_id");
            entity.Property(e => e.AllotDate).HasColumnName("allot_date");
            entity.Property(e => e.LeaveDate).HasColumnName("leave_date");
            entity.Property(e => e.RoomId).HasColumnName("room_id");
            entity.Property(e => e.Status)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("status");
            entity.Property(e => e.UserId).HasColumnName("user_id");

            entity.HasOne(d => d.Room).WithMany(p => p.Allotments)
                .HasForeignKey(d => d.RoomId)
                .HasConstraintName("FK__allotment__room___5CD6CB2B");

            entity.HasOne(d => d.User).WithMany(p => p.Allotments)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK__allotment__user___5BE2A6F2");
        });

        modelBuilder.Entity<Complaint>(entity =>
        {
            entity.HasKey(e => e.ComplaintId).HasName("PK__complain__A771F61C89BEDB25");

            entity.ToTable("complaints");

            entity.Property(e => e.ComplaintId).HasColumnName("complaint_id");
            entity.Property(e => e.DateFiled).HasColumnName("date_filed");
            entity.Property(e => e.Issue)
                .HasColumnType("text")
                .HasColumnName("issue");
            entity.Property(e => e.RoomId).HasColumnName("room_id");
            entity.Property(e => e.Status)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("status");
            entity.Property(e => e.UserId).HasColumnName("user_id");

            entity.HasOne(d => d.Room).WithMany(p => p.Complaints)
                .HasForeignKey(d => d.RoomId)
                .HasConstraintName("FK__complaint__room___5EBF139D");

            entity.HasOne(d => d.User).WithMany(p => p.Complaints)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK__complaint__user___5DCAEF64");
        });

        modelBuilder.Entity<Hostel>(entity =>
        {
            entity.HasKey(e => e.HostelId).HasName("PK__hostels__A3EE317EADA038B2");

            entity.ToTable("hostels");

            entity.Property(e => e.HostelId).HasColumnName("hostel_id");
            entity.Property(e => e.Location)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("location");
            entity.Property(e => e.Name)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("name");
            entity.Property(e => e.Status)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("status");
            entity.Property(e => e.TotalRooms).HasColumnName("total_rooms");
            entity.Property(e => e.Type)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("type");
        });

        modelBuilder.Entity<HostelUser>(entity =>
        {
            entity.HasKey(e => e.UserId).HasName("PK__hostel_u__B9BE370FF9B8DF51");

            entity.ToTable("hostel_user");

            entity.Property(e => e.UserId).HasColumnName("user_id");
            entity.Property(e => e.Address)
                .HasColumnType("text")
                .HasColumnName("address");
            entity.Property(e => e.Course)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("course");
            entity.Property(e => e.Dob).HasColumnName("dob");
            entity.Property(e => e.Email)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("email");
            entity.Property(e => e.Gender)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("gender");
            entity.Property(e => e.HostelId).HasColumnName("hostel_id");
            entity.Property(e => e.Name)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("name");
            entity.Property(e => e.Password)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("password");
            entity.Property(e => e.Phone)
                .HasMaxLength(15)
                .IsUnicode(false)
                .HasColumnName("phone");
            entity.Property(e => e.Role).HasColumnName("role");
            entity.Property(e => e.Status)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("status");
            entity.Property(e => e.Username)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("username");

            entity.HasOne(d => d.Hostel).WithMany(p => p.HostelUsers)
                .HasForeignKey(d => d.HostelId)
                .HasConstraintName("FK__hostel_us__hoste__5AEE82B9");

            entity.HasOne(d => d.RoleNavigation).WithMany(p => p.HostelUsers)
                .HasForeignKey(d => d.Role)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__hostel_use__role__628FA481");
        });

        modelBuilder.Entity<Inventory>(entity =>
        {
            entity.HasKey(e => e.ItemId).HasName("PK__inventor__52020FDDC1A06E71");

            entity.ToTable("inventory");

            entity.Property(e => e.ItemId).HasColumnName("item_id");
            entity.Property(e => e.Condition)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("condition");
            entity.Property(e => e.HostelId).HasColumnName("hostel_id");
            entity.Property(e => e.ItemName)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("item_name");
            entity.Property(e => e.Quantity).HasColumnName("quantity");

            entity.HasOne(d => d.Hostel).WithMany(p => p.Inventories)
                .HasForeignKey(d => d.HostelId)
                .HasConstraintName("FK__inventory__hoste__5FB337D6");
        });

        modelBuilder.Entity<Payment>(entity =>
        {
            entity.HasKey(e => e.PaymentId).HasName("PK__payment__ED1FC9EA343611F2");

            entity.ToTable("payment");

            entity.Property(e => e.PaymentId).HasColumnName("payment_id");
            entity.Property(e => e.Amount)
                .HasColumnType("decimal(10, 2)")
                .HasColumnName("amount");
            entity.Property(e => e.Date).HasColumnName("date");
            entity.Property(e => e.Method)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("method");
            entity.Property(e => e.Status)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("status");
            entity.Property(e => e.UserId).HasColumnName("user_id");

            entity.HasOne(d => d.User).WithMany(p => p.Payments)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK__payment__user_id__60A75C0F");
        });

        modelBuilder.Entity<Role>(entity =>
        {
            entity.HasKey(e => e.RoleId).HasName("PK__roles__760965CC7D245A6A");

            entity.ToTable("roles");

            entity.Property(e => e.RoleId).HasColumnName("role_id");
            entity.Property(e => e.RoleName)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("role_name");
        });

        modelBuilder.Entity<Room>(entity =>
        {
            entity.HasKey(e => e.RoomId).HasName("PK__rooms__19675A8A689D0C2B");

            entity.ToTable("rooms");

            entity.Property(e => e.RoomId).HasColumnName("room_id");
            entity.Property(e => e.Capacity).HasColumnName("capacity");
            entity.Property(e => e.HostelId).HasColumnName("hostel_id");
            entity.Property(e => e.Occupied).HasColumnName("occupied");
            entity.Property(e => e.RoomNumber)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("room_number");
            entity.Property(e => e.Status)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("status");
            entity.Property(e => e.Type)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("type");

            entity.HasOne(d => d.Hostel).WithMany(p => p.Rooms)
                .HasForeignKey(d => d.HostelId)
                .HasConstraintName("FK__rooms__hostel_id__59FA5E80");
        });

        modelBuilder.Entity<Visitor>(entity =>
        {
            entity.HasKey(e => e.VisitorId).HasName("PK__visitors__87ED1B51CE44995C");

            entity.ToTable("visitors");

            entity.Property(e => e.VisitorId).HasColumnName("visitor_id");
            entity.Property(e => e.InTime).HasColumnName("in_time");
            entity.Property(e => e.Name)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("name");
            entity.Property(e => e.OutTime).HasColumnName("out_time");
            entity.Property(e => e.Relation)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("relation");
            entity.Property(e => e.UserId).HasColumnName("user_id");
            entity.Property(e => e.VisitDate).HasColumnName("visit_date");

            entity.HasOne(d => d.User).WithMany(p => p.Visitors)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK__visitors__user_i__619B8048");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
