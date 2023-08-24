using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using TMServer.DataBase.Tables;

namespace TMServer.DataBase;

public partial class TmdbContext : DbContext
{
    public TmdbContext()
    {

    }

    public TmdbContext(DbContextOptions<TmdbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<DBAes> AesCrypts { get; set; }

    public virtual DbSet<DBChat> Chats { get; set; }

    public virtual DbSet<DBFriend> Friends { get; set; }

    public virtual DbSet<DBMessage> Messages { get; set; }

    public virtual DbSet<DBRsa> RsaCrypts { get; set; }

    public virtual DbSet<DBToken> Tokens { get; set; }

    public virtual DbSet<DBUser> Users { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder.UseNpgsql("Host=localhost;Port=5432;Database=tmdb;Username=tmadmin;Password=1234;Include Error Detail=True;");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<DBAes>(entity =>
        {
            entity.HasKey(e => e.CryptId).HasName("aes_pkey");

            entity.ToTable("aes");

            entity.Property(e => e.AesKey)
                .HasMaxLength(64)
                .HasColumnName("aes_key");

            entity.Property(e => e.IV)
                .HasMaxLength(32)
                .HasColumnName("iv");

            entity.HasOne(e => e.User)
            .WithOne(u => u.Crypt)
            .HasForeignKey<DBUser>(u => u.CryptId);
        });

        modelBuilder.Entity<DBChat>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("chats_pkey");

            entity.ToTable("chats");

            entity.Property(e => e.ChatId).HasColumnName("chat_id");
            entity.Property(e => e.MemberId).HasColumnName("member_id");
            entity.Property(e => e.AdminId).HasColumnName("admin_id");

            entity.HasOne(e => e.Admin).WithOne()
            .HasForeignKey<DBChat>(c => c.AdminId);

            entity.HasMany(d => d.Members).WithMany(p => p.Chats);

            entity.HasMany(c => c.Messages).WithOne(m => m.Destination)
            .HasForeignKey(m => m.DestinationId);
        });

        modelBuilder.Entity<DBFriend>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("friends_pkey");

            entity.ToTable("friends");

            entity.Property(e => e.UserIdOne)
                .ValueGeneratedNever()
                .HasColumnName("user_id_one");

            entity.Property(e => e.UserIdTwo)
              .ValueGeneratedNever()
              .HasColumnName("user_id_two");

            entity.HasOne(e => e.UserOne)
            .WithMany(e => e.FriendsOne)
            .HasForeignKey(f => f.UserIdOne)
            .HasPrincipalKey(u => u.Id);

            entity.HasOne(e => e.UserTwo)
            .WithMany(e => e.FriendsTwo)
            .HasForeignKey(f => f.UserIdTwo)
            .HasPrincipalKey(u => u.Id);
        });

        modelBuilder.Entity<DBMessage>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("messages_pkey");

            entity.ToTable("messages");

            entity.Property(e => e.AuthorId).HasColumnName("author_id");

            entity.Property(e => e.Content)
                .HasMaxLength(512)
                .HasColumnName("content");

            entity.Property(e => e.DestinationId).HasColumnName("destination_id");

            entity.HasOne(d => d.Author).WithMany()
                .HasForeignKey(d => d.AuthorId)
                .OnDelete(DeleteBehavior.ClientSetNull);

            entity.HasOne(d => d.Destination).WithMany(p => p.Messages)
                .HasForeignKey(d => d.DestinationId)
                .OnDelete(DeleteBehavior.ClientSetNull);
        });

        modelBuilder.Entity<DBRsa>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("rsa_pkey");

            entity.ToTable("rsa");

            entity.Property(e => e.PrivateServerKey)
                .HasMaxLength(4096)
                .HasColumnName("private_server_key");

            entity.Property(e => e.PublicClientKey)
                .HasMaxLength(1024)
                .HasColumnName("public_client_key");

            entity.Property(e => e.CreateDate).HasColumnName("create_date");
        });

        modelBuilder.Entity<DBToken>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("tokens_pkey");

            entity.ToTable("tokens");

            entity.Property(e => e.UserId)
                .HasColumnName("user_id");

            entity.Property(e => e.AccessToken)
             .HasMaxLength(512)
             .HasColumnName("token");

            entity.Property(e => e.Expiration).HasColumnName("expiration");


            entity.HasOne(d => d.User).WithOne(p => p.Token)
                .HasForeignKey<DBToken>(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("tokens_userid_fkey");
        });

        modelBuilder.Entity<DBUser>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("users_pkey");

            entity.ToTable("users");

            entity.Property(e => e.CryptId).HasColumnName("crypt_id");

            entity.Property(e => e.LastRequest).HasColumnName("last_request");

            entity.Property(e => e.Login)
                .HasMaxLength(128)
                .HasColumnName("login");

            entity.Property(e => e.Name)
                .HasMaxLength(255)
                .HasColumnName("name");

            entity.Property(e => e.Password)
                .HasMaxLength(512)
                .HasColumnName("password");

            entity.HasOne(d => d.Crypt).WithOne(p => p.User)
                .HasForeignKey<DBUser>(e => e.CryptId);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
