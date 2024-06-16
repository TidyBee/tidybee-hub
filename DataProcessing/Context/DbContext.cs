using Microsoft.EntityFrameworkCore;
using System.Configuration;
using DataProcessing.Models.Input;
using File = DataProcessing.Models.Input.File;

namespace DataProcessing.Context;

public partial class DatabaseContext : DbContext
{
    private readonly IConfiguration _configuration;

    public DatabaseContext(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public DatabaseContext(DbContextOptions<DbContext> options, IConfiguration configuration)
        : base(options)
    {
        _configuration = configuration;
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {
            optionsBuilder.UseNpgsql(_configuration.GetConnectionString("DatabaseConnection"));
        }
    }

    public virtual DbSet<DuplicateAssociativeTable> DuplicateAssociativeTables { get; set; }

    public virtual DbSet<File> Files { get; set; }

    public virtual DbSet<Rule> Rules { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<DuplicateAssociativeTable>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("duplicate_associative_table_pkey");

            entity.ToTable("duplicate_associative_table");

            entity.HasIndex(e => new { e.OriginalFileId, e.DuplicateFileId }, "unique_file_pair").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.DuplicateFileId).HasColumnName("duplicate_file_id");
            entity.Property(e => e.OriginalFileId).HasColumnName("original_file_id");

            entity.HasOne(d => d.DuplicateFile).WithMany(p => p.DuplicateAssociativeTableDuplicateFiles)
                .HasForeignKey(d => d.DuplicateFileId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_duplicate_file");

            entity.HasOne(d => d.OriginalFile).WithMany(p => p.DuplicateAssociativeTableOriginalFiles)
                .HasForeignKey(d => d.OriginalFileId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_original_file");
        });

        modelBuilder.Entity<File>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("files_pkey");

            entity.ToTable("files");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.DuplicatedScore)
                .HasMaxLength(1)
                .HasColumnName("duplicated_score");
            entity.Property(e => e.FileHash).HasColumnName("file_hash");
            entity.Property(e => e.GlobalScore)
                .HasMaxLength(1)
                .HasColumnName("global_score");
            entity.Property(e => e.LastModified)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("last_modified");
            entity.Property(e => e.MisnamedScore)
                .HasMaxLength(1)
                .HasColumnName("misnamed_score");
            entity.Property(e => e.Name).HasColumnName("name");
            entity.Property(e => e.PerishedScore)
                .HasMaxLength(1)
                .HasColumnName("perished_score");
            entity.Property(e => e.Size).HasColumnName("size");
        });

        modelBuilder.Entity<Rule>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("rules_pkey");

            entity.ToTable("rules");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Description).HasColumnName("description");
            entity.Property(e => e.Name).HasColumnName("name");
            entity.Property(e => e.RulesConfig)
                .HasColumnType("json")
                .HasColumnName("rules_config");
            entity.Property(e => e.Weight).HasColumnName("weight");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
