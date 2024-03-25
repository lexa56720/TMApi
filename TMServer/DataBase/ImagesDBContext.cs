using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMServer.DataBase.Tables.LongPolling;
using TMServer.DataBase.Tables;
using static System.Net.Mime.MediaTypeNames;

namespace TMServer.DataBase
{
    internal class ImagesDBContext : DbContext
    {
        public ImagesDBContext()
        {
        }

        public ImagesDBContext(DbContextOptions<ImagesDBContext> options)
            : base(options)
        {
        }

        public virtual DbSet<DBImage> Images { get; set; }
        public virtual DbSet<DBImageSet> ImageSets { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseNpgsql(GlobalSettings.ImagesDBConnectionString);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<DBImage>(entity =>
            {
                entity.ToTable("images");

                entity.HasKey(e => e.Id).HasName("images_pkey");
                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.SetId)
                      .HasColumnName("set_id");

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
        }
    }
}
