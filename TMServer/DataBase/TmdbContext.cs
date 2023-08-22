using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using TMServer.DataBase.Tables;

namespace TMServer.DataBase;

public partial class TmdbContext : DbContext
{
    public TmdbContext()
    {
        Database.EnsureCreated();
    }

    public TmdbContext(DbContextOptions<TmdbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<AesCrypt> AesCrypts { get; set; }

    public virtual DbSet<Chat> Chats { get; set; }

    public virtual DbSet<Friend> Friends { get; set; }

    public virtual DbSet<Message> Messages { get; set; }

    public virtual DbSet<RsaCrypt> RsaCrypts { get; set; }

    public virtual DbSet<Token> Tokens { get; set; }

    public virtual DbSet<User> Users { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder.UseNpgsql("Host=localhost;Port=5432;Database=tmdb;Username=tmadmin;Password=1234");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<AesCrypt>(entity =>
        {
            entity.HasKey(e => e.CryptId).HasName("aescrypts_pkey");

            entity.ToTable("aescrypts");

            entity.Property(e => e.CryptId).HasColumnName("cryptid");
            entity.Property(e => e.AesKey)
                .HasMaxLength(64)
                .HasColumnName("aeskey");
            entity.Property(e => e.IV)
                .HasMaxLength(32)
                .HasColumnName("iv");
        });

        modelBuilder.Entity<Chat>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("chats_pkey");

            entity.ToTable("chats");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.MemberId).HasColumnName("memberid");

            entity.HasOne(d => d.Member).WithMany(p => p.Chats)
                .HasForeignKey(d => d.MemberId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("chats_memberid_fkey");
        });

        modelBuilder.Entity<Friend>(entity =>
        {
            entity.HasKey(e => e.Userid).HasName("friends_pkey");

            entity.ToTable("friends");

            entity.Property(e => e.Userid)
                .ValueGeneratedNever()
                .HasColumnName("userid");
            entity.Property(e => e.FriendId).HasColumnName("friendid");

            entity.HasOne(d => d.FriendNavigation).WithMany(p => p.FriendsNavigations)
                .HasForeignKey(d => d.FriendId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("friends_friendid_fkey");

            entity.HasOne(d => d.User).WithOne(p => p.FriendUser)
                .HasForeignKey<Friend>(d => d.Userid)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("friends_userid_fkey");
        });

        modelBuilder.Entity<Message>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("messages_pkey");

            entity.ToTable("messages");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.AuthorId).HasColumnName("authorid");
            entity.Property(e => e.Content)
                .HasMaxLength(255)
                .HasColumnName("content");
            entity.Property(e => e.DestinationId).HasColumnName("destinationid");

            entity.HasOne(d => d.Author).WithMany(p => p.Messages)
                .HasForeignKey(d => d.AuthorId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("messages_authorid_fkey");

            entity.HasOne(d => d.Destination).WithMany(p => p.Messages)
                .HasForeignKey(d => d.DestinationId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("messages_destinationid_fkey");
        });

        modelBuilder.Entity<RsaCrypt>(entity =>
        {
            entity.HasKey(e => e.Ip).HasName("rsacrypts_pkey");

            entity.ToTable("rsacrypts");

            entity.Property(e => e.Ip)
                .ValueGeneratedNever()
                .HasColumnName("ip");
            entity.Property(e => e.PrivateServerKey)
                .HasMaxLength(2048)
                .HasColumnName("privateserverkey");
            entity.Property(e => e.PublicClientKey)
                .HasMaxLength(512)
                .HasColumnName("publicclientkey");
        });

        modelBuilder.Entity<Token>(entity =>
        {
            entity.HasKey(e => e.UserId).HasName("tokens_pkey");

            entity.ToTable("tokens");

            entity.Property(e => e.UserId)
                .ValueGeneratedNever()
                .HasColumnName("userid");
            entity.Property(e => e.Expiration).HasColumnName("expiration");
            entity.Property(e => e.AccessToken)
                .HasMaxLength(512)
                .HasColumnName("token");

            entity.HasOne(d => d.User).WithOne(p => p.Token)
                .HasForeignKey<Token>(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("tokens_userid_fkey");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("users_pkey");

            entity.ToTable("users");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.CryptId).HasColumnName("cryptid");
            entity.Property(e => e.IsOnline).HasColumnName("isonline");
            entity.Property(e => e.Login)
                .HasMaxLength(128)
                .HasColumnName("login");
            entity.Property(e => e.Name)
                .HasMaxLength(255)
                .HasColumnName("name");
            entity.Property(e => e.Password)
                .HasMaxLength(512)
                .HasColumnName("password");

            entity.HasOne(d => d.Crypt).WithMany(p => p.Users)
                .HasForeignKey(d => d.CryptId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("users_cryptid_fkey");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
