using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMServer.DataBase.Tables.LongPolling;
using static System.Net.Mime.MediaTypeNames;
using TMServer.DataBase.Tables.FileTables;

namespace TMServer.DataBase
{
    internal class FilesDBContext : DbContext
    {
        public FilesDBContext()
        {
        }

        public FilesDBContext(DbContextOptions<FilesDBContext> options)
            : base(options)
        {
        }

        public virtual DbSet<DBImage> Images { get; set; }
        public virtual DbSet<DBImageSet> ImageSets { get; set; }
        public virtual DbSet<DBBinaryFile> Files { get; set; }


        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseNpgsql(GlobalSettings.FilesDBConnectionString);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<DBImage>(entity =>
            {
                entity.ToTable("images");

                entity.HasKey(e => e.Id).HasName("images_pkey");
                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.SetId)
                      .HasColumnName("set_id")
                      .IsRequired(false);

                entity.Property(e => e.Url)
                      .HasColumnName("url")
                      .HasMaxLength(128);

                entity.Property(e => e.Size)
                      .HasColumnName("size");

                entity.Property(e => e.Data)
                      .HasColumnName("data");
            });

            modelBuilder.Entity<DBImageSet>(entity =>
            {
                entity.ToTable("images_sets");

                entity.HasKey(e => e.Id).HasName("image_sets_pkey");
                entity.Property(e => e.Id).HasColumnName("id");

                entity.HasMany(e => e.Images)
                      .WithOne(e => e.Set)
                      .HasForeignKey(e => e.SetId);
            });

            modelBuilder.Entity<DBBinaryFile>(entity =>
            {
                entity.ToTable("binary_files");

                entity.HasKey(e => e.Id).HasName("binary_files_pkey");
                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Name)
                      .HasColumnName("name")
                      .HasMaxLength(128);

                entity.Property(e => e.Url)
                      .HasColumnName("url")
                      .HasMaxLength(128);

                entity.Property(e => e.Data)
                      .HasColumnName("data");
            });
        }
    }
}
