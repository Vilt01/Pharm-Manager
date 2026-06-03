using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using TaskManager.Models;

namespace TaskManager.Data;

public partial class AppDbContext : DbContext
{
    public AppDbContext()
    {
    }

    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Department> Departments { get; set; }

    public virtual DbSet<Request> Requests { get; set; }

    public virtual DbSet<Role> Roles { get; set; }

    public virtual DbSet<StoryModification> StoryModifications { get; set; }

    public virtual DbSet<User> Users { get; set; }

    public virtual DbSet<Zapros> Zapros { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseNpgsql("Host=localhost;Port=5432;Database=postgres;Username=postgres;Password=2417");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Department>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("Department_pkey");

            entity.ToTable("Department");

            entity.Property(e => e.Id)
                .UseIdentityAlwaysColumn()
                .HasColumnName("id");
            entity.Property(e => e.Name)
                .HasColumnType("character varying")
                .HasColumnName("name");
        });

        modelBuilder.Entity<Request>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("Request_pkey1");

            entity.ToTable("Request");

            entity.Property(e => e.Id)
                .UseIdentityAlwaysColumn()
                .HasColumnName("id");
            entity.Property(e => e.Daas).HasColumnName("daas");
            entity.Property(e => e.FkUser).HasColumnName("fk_user");
            entity.Property(e => e.FkZapros).HasColumnName("fk_zapros");
            entity.Property(e => e.NumberOrder).HasColumnName("number_order");
            entity.Property(e => e.Party).HasColumnName("party");

            entity.HasOne(d => d.FkUserNavigation).WithMany(p => p.Requests)
                .HasForeignKey(d => d.FkUser)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("Request_fk_user_fkey");

            entity.HasOne(d => d.FkZaprosNavigation).WithMany(p => p.Requests)
                .HasForeignKey(d => d.FkZapros)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("Request_fk_zapros_fkey");
        });

        modelBuilder.Entity<Role>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("Role_pkey");

            entity.ToTable("Role");

            entity.Property(e => e.Id)
                .UseIdentityAlwaysColumn()
                .HasColumnName("id");
            entity.Property(e => e.Name)
                .HasMaxLength(100)
                .HasColumnName("name");
        });

        modelBuilder.Entity<StoryModification>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("StoryModification_pkey");

            entity.ToTable("StoryModification");

            entity.Property(e => e.Id)
                .UseIdentityAlwaysColumn()
                .HasColumnName("id");
            entity.Property(e => e.Action)
                .HasColumnType("character varying")
                .HasColumnName("action");
            entity.Property(e => e.Date).HasColumnName("date");
            entity.Property(e => e.FkRequest).HasColumnName("fk_request");
            entity.Property(e => e.FkUser).HasColumnName("fk_user");

            entity.HasOne(d => d.FkRequestNavigation).WithMany(p => p.StoryModifications)
                .HasForeignKey(d => d.FkRequest)
                .HasConstraintName("StoryModification_fk_request_fkey");

            entity.HasOne(d => d.FkUserNavigation).WithMany(p => p.StoryModifications)
                .HasForeignKey(d => d.FkUser)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("StoryModification_fk_user_fkey");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("User_pkey");

            entity.ToTable("User");

            entity.Property(e => e.Id)
                .UseIdentityAlwaysColumn()
                .HasColumnName("id");
            entity.Property(e => e.Avatar).HasColumnName("avatar");
            entity.Property(e => e.FkDepartment).HasColumnName("fk_department");
            entity.Property(e => e.FkManagerId).HasColumnName("fk_manager_id");
            entity.Property(e => e.FkRole).HasColumnName("fk_role");
            entity.Property(e => e.IsDeleted)
                .HasDefaultValue(false)
                .HasColumnName("is_deleted");
            entity.Property(e => e.Lastname)
                .HasMaxLength(100)
                .HasColumnName("lastname");
            entity.Property(e => e.Login)
                .HasColumnType("character varying")
                .HasColumnName("login");
            entity.Property(e => e.Mail)
                .HasColumnType("character varying")
                .HasColumnName("mail");
            entity.Property(e => e.Name)
                .HasMaxLength(100)
                .HasColumnName("name");
            entity.Property(e => e.Password)
                .HasColumnType("character varying")
                .HasColumnName("password");
            entity.Property(e => e.Phone).HasColumnName("phone");
            entity.Property(e => e.Surname)
                .HasMaxLength(100)
                .HasColumnName("surname");

            entity.HasOne(d => d.FkDepartmentNavigation).WithMany(p => p.Users)
                .HasForeignKey(d => d.FkDepartment)
                .HasConstraintName("User_fk_department_fkey");

            entity.HasOne(d => d.FkManager).WithMany(p => p.InverseFkManager)
                .HasForeignKey(d => d.FkManagerId)
                .HasConstraintName("User_fk_manager_id_fkey");

            entity.HasOne(d => d.FkRoleNavigation).WithMany(p => p.Users)
                .HasForeignKey(d => d.FkRole)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("User_FkRole_fkey");
        });

        modelBuilder.Entity<Zapros>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("Request_pkey");

            entity.Property(e => e.Id)
                .UseIdentityAlwaysColumn()
                .HasIdentityOptions(4L, null, null, null, null, null)
                .HasColumnName("id");
            entity.Property(e => e.Amount).HasColumnName("amount");
            entity.Property(e => e.DateComplete)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("date_complete");
            entity.Property(e => e.DateCreate).HasColumnName("date_create");
            entity.Property(e => e.DateProcess)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("date_process");
            entity.Property(e => e.FkUser).HasColumnName("fk_user");
            entity.Property(e => e.Name)
                .HasColumnType("character varying")
                .HasColumnName("name");
            entity.Property(e => e.Ozm).HasColumnName("ozm");
            entity.Property(e => e.Reason)
                .HasColumnType("character varying")
                .HasColumnName("reason");
            entity.Property(e => e.StatusRequest)
                .HasColumnType("character varying")
                .HasColumnName("status_request");
            entity.Property(e => e.UnitMeasure).HasColumnName("unit_measure");
            entity.Property(e => e.Url).HasColumnName("url");

            entity.HasOne(d => d.FkUserNavigation).WithMany(p => p.Zapros)
                .HasForeignKey(d => d.FkUser)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("Zapros_fk_user_fkey");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
