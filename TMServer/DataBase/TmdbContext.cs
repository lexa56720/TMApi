﻿using Microsoft.EntityFrameworkCore;
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

    public virtual DbSet<DBChatInvite> ChatInvites { get; set; }

    public virtual DbSet<DBFriendRequest> FriendRequests { get; set; }

    public virtual DbSet<DBMessageMedia> MessageMedias { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder.UseNpgsql(GlobalSettings.ConnectionString);

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<DBAes>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("aes_pkey");
            entity.ToTable("aes");
            entity.Property(e => e.Id).HasColumnName("id");

            entity.Property(e => e.AesKey)
                .HasMaxLength(32)
                .HasColumnName("aes_key");

            entity.Property(e => e.IsDeprecated).HasColumnName("is_depricated");
            entity.Property(e => e.DeprecatedDate).HasColumnName("depricated_date");

            entity.HasOne(e => e.User).WithMany(u => u.Crypts)
            .HasForeignKey(u => u.UserId);
        });

        modelBuilder.Entity<DBChat>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("chats_pkey");
            entity.ToTable("chats");
            entity.Property(e => e.Id).HasColumnName("id");

            entity.Property(e => e.AdminId).HasColumnName("admin_id");
            entity.Property(e => e.Name).HasColumnName("name");

            entity.Property(e => e.IsDialogue).HasColumnName("is_dialogue");

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
            entity.Property(e => e.Id).HasColumnName("id");

            entity.Property(e => e.SenderId)
                .ValueGeneratedNever()
                .HasColumnName("sender_id");

            entity.Property(e => e.DestId)
              .ValueGeneratedNever()
              .HasColumnName("dest_id");

            entity.HasOne(e => e.Sender)
            .WithMany(e => e.FriendsOne)
            .HasForeignKey(f => f.SenderId)
            .HasPrincipalKey(u => u.Id);

            entity.HasOne(e => e.Dest)
            .WithMany(e => e.FriendsTwo)
            .HasForeignKey(f => f.DestId)
            .HasPrincipalKey(u => u.Id);
        });

        modelBuilder.Entity<DBMessage>(entity =>
        {
            entity.HasKey(e => e.Id)
            .HasName("messages_pkey");
            entity.ToTable("messages");
            entity.Property(e => e.Id).HasColumnName("id");

            entity.Property(e => e.AuthorId)
            .HasColumnName("author_id");

            entity.Property(e => e.Content)
                .HasMaxLength(512)
                .HasColumnName("content");

            entity.Property(e => e.DestinationId)
            .HasColumnName("destination_id");

            entity.Property(e => e.SendTime)
            .HasColumnName("send_time");

            entity.HasMany(e => e.Medias)
            .WithOne(m => m.Message)
            .HasForeignKey(m => m.MessageId);

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
            entity.Property(e => e.Id).HasColumnName("id");

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
            entity.Property(e => e.Id).HasColumnName("id");

            entity.Property(e => e.UserId)
                .HasColumnName("user_id");

            entity.Property(e => e.AccessToken)
             .HasMaxLength(512)
             .HasColumnName("token");

            entity.Property(e => e.Expiration).HasColumnName("expiration");


            entity.HasOne(d => d.User).WithMany(p => p.Tokens)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("tokens_userid_fkey");
        });

        modelBuilder.Entity<DBUser>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("users_pkey");
            entity.ToTable("users");
            entity.Property(e => e.Id).HasColumnName("id");

            entity.Property(e => e.LastRequest).HasColumnName("last_request");

            entity.Property(e => e.RegisterDate).HasColumnName("register_date");

            entity.Property(e => e.Login)
                .HasMaxLength(128)
                .HasColumnName("login");

            entity.Property(e => e.Name)
                .HasMaxLength(128)
                .HasColumnName("name");

            entity.Property(e => e.Password)
                .HasMaxLength(512)
                .HasColumnName("password");
        });

        modelBuilder.Entity<DBChatInvite>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("chat_invites_pkey");
            entity.ToTable("chat_invites");
            entity.Property(e => e.Id).HasColumnName("id");

            entity.Property(e => e.ChatId).HasColumnName("chat_id");

            entity.Property(e => e.InviterId).HasColumnName("inviter_id");

            entity.Property(e => e.ToUserId).HasColumnName("user_id");

            entity.HasOne(e => e.Chat).WithOne()
            .HasForeignKey<DBChatInvite>(i => i.ChatId);

            entity.HasOne(e => e.Inviter).WithOne()
            .HasForeignKey<DBChatInvite>(i => i.InviterId);

            entity.HasOne(e => e.DestinationUser).WithOne()
            .HasForeignKey<DBChatInvite>(i => i.ToUserId);
        });

        modelBuilder.Entity<DBFriendRequest>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("friend_requests_pkey");
            entity.ToTable("friend_requests");
            entity.Property(e => e.Id).HasColumnName("id");

            entity.Property(e => e.UserOneId)
                .ValueGeneratedNever()
                .HasColumnName("user_id_one");

            entity.Property(e => e.UserTwoId)
              .ValueGeneratedNever()
              .HasColumnName("user_id_two");

            entity.HasOne(e => e.UserOne)
            .WithOne().HasForeignKey<DBFriendRequest>(r => r.UserOneId);

            entity.HasOne(e => e.UserTwo)
           .WithOne().HasForeignKey<DBFriendRequest>(r => r.UserTwoId);
        });

        modelBuilder.Entity<DBMessageMedia>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("message_medias_pkey");
            entity.ToTable("message_medias");
            entity.Property(e => e.Id).HasColumnName("id");

            entity.Property(e=>e.MediaType).HasColumnName("type");

            entity.Property(e=>e.Data).HasColumnName("data");

        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
