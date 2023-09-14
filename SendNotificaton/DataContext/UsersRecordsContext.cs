using Microsoft.EntityFrameworkCore;
using SendNotificaton.DataContext;
using System;
using System.Collections.Generic;
using System.Linq;

public partial class UsersRecordsContext : DbContext
{
    public UsersRecordsContext()
    {
    }

    public UsersRecordsContext(DbContextOptions<UsersRecordsContext> options)
        : base(options)
    {
    }
    public virtual DbSet<ExecutionLog> ExecutionLogs { get; set; }
    public virtual DbSet<Record> Records { get; set; }
    public List<Record> GetUsersRecordsFromStoredProcedure()
    {
        var results = Records
            .FromSqlRaw("EXEC dbo.spGetRecentUsersRecords")
            .ToList();

        return results;
    }
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {
            optionsBuilder.UseSqlServer(Environment.GetEnvironmentVariable("SqlServerConnectionString"));
        }
    }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Do not generate migration script for context class UsersRecordsContext
        modelBuilder.Entity<UsersRecordsContext>(entity =>
        {
            entity.HasNoKey();
        });
        // Migrations for ExecutionLog and Record needs to be generated
        modelBuilder.Entity<ExecutionLog>(entity =>
        {
            entity.HasKey(e => e.LogId);

            entity.ToTable("ExecutionLog");

            entity.Property(e => e.LastExecutionTime).HasColumnType("datetime");
            entity.Property(e => e.ProcedureName)
                .HasMaxLength(50)
                .IsUnicode(false);
        });
        modelBuilder.Entity<Record>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.ToTable("Records");
            entity.Property(e => e.CreatedTime).HasColumnType("datetime");
            entity.Property(e => e.DataValue)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.RecordId)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.UserEmail)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.UserId)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.UserName)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.NotificationFlag)
                .HasColumnType("bit").HasConversion<bool>();
        });
        OnModelCreatingPartial(modelBuilder);
    }
    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
