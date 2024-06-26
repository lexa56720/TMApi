﻿using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using TMServer.DataBase.Tables;
using TMServer.DataBase.Tables.LongPolling;
using TMServer.RequestHandlers;

namespace TMServer.DataBase;

public partial class TmdbContext : DbContext
{
    public static ChangeHandler ChangeHandler { get; private set; }

    static TmdbContext()
    {
        ChangeHandler = new ChangeHandler(new Interaction.Changes());
    }
    public TmdbContext()
    {
    }

    public TmdbContext(DbContextOptions<TmdbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<DBChat> Chats { get; set; }
    public virtual DbSet<DBFriend> Friends { get; set; }
    public virtual DbSet<DBMessage> Messages { get; set; }
    public virtual DbSet<DBUser> Users { get; set; }
    public virtual DbSet<DBChatInvite> ChatInvites { get; set; }
    public virtual DbSet<DBFriendRequest> FriendRequests { get; set; }
    public virtual DbSet<DBMessageAttachments> MessageAttachments { get; set; }
    public virtual DbSet<DBUnreadMessage> UnreadMessages { get; set; }
    public virtual DbSet<DBChatUpdate> ChatUpdates { get; set; }
    public virtual DbSet<DBMessageAction> MessageActions { get; set; }

    public virtual DbSet<DBChatInviteUpdate> ChatInviteUpdates { get; set; }
    public virtual DbSet<DBFriendRequestUpdate> FriendRequestUpdates { get; set; }
    public virtual DbSet<DBUserProfileUpdate> UserProfileUpdates { get; set; }
    public virtual DbSet<DBFriendListUpdate> FriendListUpdates { get; set; }
    public virtual DbSet<DBUserOnlineUpdate> UserOnlineUpdates { get; set; }
    public virtual DbSet<DBChatListUpdate> ChatListUpdates { get; set; }
    public virtual DbSet<DBNewMessageUpdate> NewMessageUpdates { get; set; }
    public virtual DbSet<DBMessageStatusUpdate> MessageStatusUpdates { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseNpgsql(Settings.TMDBConnectionString);
        //optionsBuilder.LogTo(Console.WriteLine);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        //Код для создания таблицы Chats
        modelBuilder.Entity<DBChat>(entity =>
        {
            entity.ToTable("chats");

            entity.HasKey(e => e.Id).HasName("chats_pkey");
            entity.Property(e => e.Id).HasColumnName("id");

            entity.Property(e => e.AdminId)
                  .HasColumnName("admin_id");

            entity.Property(e => e.Name)
                  .HasColumnName("name");

            entity.Property(e => e.CoverImageId)
                  .HasColumnName("cover_image_id");

            entity.Property(e => e.IsDialogue)
                  .HasColumnName("is_dialogue");

            entity.HasOne(e => e.Admin)
                  .WithMany()
                  .HasForeignKey(c => c.AdminId);

            entity.HasMany(d => d.Members)
                  .WithMany(p => p.Chats)
                  .UsingEntity<DBChatUser>
                  (
                    l => l.HasOne(e => e.User).WithMany().HasForeignKey(e => e.UserId),
                    r => r.HasOne(e => e.Chat).WithMany().HasForeignKey(e => e.ChatId),
                    j =>
                    {
                        j.ToTable("user_chats");
                        j.HasKey(t => new { t.UserId, t.ChatId });
                    }
                  );

            entity.HasMany(c => c.Messages)
                  .WithOne(m => m.Destination)
                  .HasForeignKey(m => m.DestinationId);
        });
        //Код для создания таблицы Friends
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

            entity.HasOne(e => e.Receiver)
            .WithMany(e => e.FriendsTwo)
            .HasForeignKey(f => f.DestId)
            .HasPrincipalKey(u => u.Id);
        });
        //Код для создания таблицы Messages
        modelBuilder.Entity<DBMessage>(entity =>
        {
            entity.ToTable("messages");

            entity.HasKey(e => e.Id).HasName("messages_pkey");
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

            entity.Property(e => e.IsSystem)
                  .HasColumnName("is_system");

            entity.HasMany(e => e.Attachments)
                  .WithOne(m => m.Message)
                  .HasForeignKey(m => m.MessageId);

            entity.HasOne(m => m.Action)
                  .WithOne(a => a.Message)
                  .HasForeignKey<DBMessageAction>(a => a.MessageId)
                  .IsRequired(false);

            entity.HasOne(d => d.Author).WithMany()
                  .HasForeignKey(d => d.AuthorId)
                  .OnDelete(DeleteBehavior.ClientSetNull);

            entity.HasOne(d => d.Destination).WithMany(p => p.Messages)
                  .HasForeignKey(d => d.DestinationId)
                  .OnDelete(DeleteBehavior.ClientSetNull);
        });

        modelBuilder.Entity<DBUser>(entity =>
        {
            entity.ToTable("users");

            entity.HasKey(e => e.Id).HasName("users_pkey");
            entity.Property(e => e.Id).HasColumnName("id");

            entity.Property(e => e.LastRequest)
                  .HasColumnName("last_request");

            entity.Property(e => e.RegisterDate)
                  .HasColumnName("register_date");

            entity.Property(e => e.ProfileImageId)
                  .HasColumnName("profile_image_id");

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
            entity.ToTable("chat_invites");

            entity.HasKey(e => e.Id).HasName("chat_invites_pkey");
            entity.Property(e => e.Id).HasColumnName("id");

            entity.Property(e => e.ChatId)
                  .HasColumnName("chat_id");

            entity.Property(e => e.InviterId)
                  .HasColumnName("inviter_id");

            entity.Property(e => e.ToUserId)
                  .HasColumnName("user_id");

            entity.HasOne(e => e.Chat)
                  .WithMany()
                  .HasForeignKey(i => i.ChatId);

            entity.HasOne(e => e.Inviter)
                  .WithMany()
                  .HasForeignKey(i => i.InviterId);

            entity.HasOne(e => e.DestinationUser)
                  .WithMany()
                  .HasForeignKey(i => i.ToUserId);
        });
        modelBuilder.Entity<DBFriendRequest>(entity =>
        {
            entity.ToTable("friend_requests");

            entity.HasKey(e => e.Id).HasName("friend_requests_pkey");
            entity.Property(e => e.Id).HasColumnName("id");

            entity.Property(e => e.SenderId)
                  .HasColumnName("user_id_one");

            entity.Property(e => e.ReceiverId)
                  .HasColumnName("user_id_two");

            entity.HasOne(e => e.Sender)
                  .WithMany()
                  .HasForeignKey(r => r.SenderId);

            entity.HasOne(e => e.Receiver)
                  .WithMany()
                  .HasForeignKey(r => r.ReceiverId);
        });
        modelBuilder.Entity<DBMessageAttachments>(entity =>
        {
            entity.ToTable("message_attachments");

            entity.HasKey(e => e.Id).HasName("message_attachments_pkey");
            entity.Property(e => e.Id).HasColumnName("id");


            entity.Property(e => e.Kind)
                  .HasColumnName("kind");

            entity.Property(e => e.AttachmentId)
                  .HasColumnName("attachment_id");
        });
        modelBuilder.Entity<DBUnreadMessage>(entity =>
        {
            entity.ToTable("unread_messages");

            entity.HasKey(e => e.Id).HasName("unread_messages_pkey");
            entity.Property(e => e.Id).HasColumnName("id");

            entity.Property(e => e.MessageId)
                  .HasColumnName("message_id");

            entity.Property(e => e.UserId)
                  .HasColumnName("user_id");

            entity.HasOne(e => e.User)
                  .WithMany()
                  .HasForeignKey(u => u.UserId);

            entity.HasOne(e => e.Message)
                  .WithMany()
                  .HasForeignKey(u => u.MessageId);
        });
        modelBuilder.Entity<DBMessageAction>(entity =>
        {
            entity.ToTable("message_action");

            entity.HasKey(e => e.Id).HasName("message_action_pkey");
            entity.Property(e => e.Id).HasColumnName("id");

            entity.Property(e => e.MessageId)
                  .HasColumnName("message_id");

            entity.Property(e => e.ExecutorId)
                  .HasColumnName("executor_id");

            entity.Property(e => e.TargetId)
                  .HasColumnName("target_id")
                  .IsRequired(false);

            entity.Property(e => e.Kind)
                  .HasColumnName("kind");

            entity.HasOne(e => e.Target)
                  .WithMany()
                  .HasForeignKey(e => e.TargetId)
                  .IsRequired(false);

            entity.HasOne(e => e.Executor)
                  .WithMany()
                  .HasForeignKey(e => e.ExecutorId);
        });

        modelBuilder.Entity<DBNewMessageUpdate>(entity =>
        {
            entity.ToTable("new_messages");

            entity.HasKey(e => e.Id).HasName("new_message_update_pkey");
            entity.Property(e => e.Id).HasColumnName("id");

            entity.Property(e => e.MessageId)
                  .HasColumnName("message_id");

            entity.Property(e => e.UserId)
                  .HasColumnName("user_id");

            entity.HasOne(e => e.User)
                  .WithMany()
                  .HasForeignKey(u => u.UserId);

            entity.HasOne(e => e.Message)
                  .WithMany()
                  .HasForeignKey(u => u.MessageId);
        });
        modelBuilder.Entity<DBMessageStatusUpdate>(entity =>
        {
            entity.ToTable("message_status_updates");

            entity.HasKey(e => e.Id).HasName("message_status_updates_pkey");
            entity.Property(e => e.Id).HasColumnName("id");

            entity.Property(e => e.MessageId)
                  .HasColumnName("message_id");

            entity.Property(e => e.UserId)
                  .HasColumnName("user_id");

            entity.HasOne(e => e.User)
                  .WithMany()
                  .HasForeignKey(u => u.UserId);

            entity.HasOne(e => e.Message)
                  .WithMany()
                  .HasForeignKey(u => u.MessageId);
        });
        modelBuilder.Entity<DBChatInviteUpdate>(entity =>
        {
            entity.ToTable("chat_invite_updates");

            entity.HasKey(e => e.Id).HasName("chat_invite_updates_pkey");
            entity.Property(e => e.Id).HasColumnName("id");


            entity.Property(e => e.ChatInviteId)
                  .HasColumnName("invite_id");

            entity.Property(e => e.UserId)
                  .HasColumnName("user_id");

            entity.HasOne(e => e.User)
                  .WithMany()
                  .HasForeignKey(u => u.UserId);

            entity.HasOne(e => e.Invite)
                  .WithMany()
                  .HasForeignKey(u => u.ChatInviteId);
        });
        modelBuilder.Entity<DBUserProfileUpdate>(entity =>
        {
            entity.ToTable("user_profile_updates");

            entity.HasKey(e => e.Id).HasName("user_profile_updates_pkey");
            entity.Property(e => e.Id).HasColumnName("id");

            entity.Property(e => e.ProfileId)
                  .HasColumnName("profile_id");

            entity.Property(e => e.UserId)
                  .HasColumnName("user_id");

            entity.HasOne(e => e.User)
                  .WithMany()
                  .HasForeignKey(u => u.UserId);

            entity.HasOne(e => e.Profile)
                  .WithMany()
                  .HasForeignKey(u => u.ProfileId);
        });
        modelBuilder.Entity<DBFriendRequestUpdate>(entity =>
        {
            entity.ToTable("friend_request_updates");

            entity.HasKey(e => e.Id).HasName("friend_request_updates_pkey");
            entity.Property(e => e.Id).HasColumnName("id");

            entity.Property(e => e.RequestId)
                  .HasColumnName("request_id");

            entity.Property(e => e.UserId)
                  .HasColumnName("user_id");

            entity.HasOne(e => e.User)
                  .WithMany()
                  .HasForeignKey(u => u.UserId);

            entity.HasOne(e => e.FriendRequest)
                  .WithMany()
                  .HasForeignKey(u => u.RequestId);
        });
        modelBuilder.Entity<DBFriendListUpdate>(entity =>
        {
            entity.ToTable("friend_list_updates");

            entity.HasKey(e => e.Id).HasName("friend_list_updates_pkey");
            entity.Property(e => e.Id).HasColumnName("id");

            entity.Property(e => e.FriendId)
                  .HasColumnName("friend_id");

            entity.Property(e => e.UserId)
                  .HasColumnName("user_id");

            entity.Property(e => e.IsAdded)
                  .HasColumnName("is_added");

            entity.Property(e => e.Date)
                  .HasColumnName("date");

            entity.HasOne(e => e.User)
                  .WithMany()
                  .HasForeignKey(u => u.UserId);

            entity.HasOne(e => e.Friend)
                  .WithMany()
                  .HasForeignKey(u => u.FriendId);
        });
        modelBuilder.Entity<DBUserOnlineUpdate>(entity =>
        {
            entity.ToTable("user_online_updates");

            entity.HasKey(e => e.Id).HasName("user_online_pkey");
            entity.Property(e => e.Id).HasColumnName("id");

            entity.Property(e => e.RelatedUserId)
                  .HasColumnName("related_user_id");

            entity.Property(e => e.UserId)
                  .HasColumnName("user_id");

            entity.Property(e => e.IsAdded)
                  .HasColumnName("is_added");

            entity.Property(e => e.Date)
                  .HasColumnName("date");

            entity.HasOne(e => e.User)
                  .WithMany()
                  .HasForeignKey(u => u.UserId);

            entity.HasOne(e => e.RelatedUser)
                  .WithMany()
                  .HasForeignKey(u => u.RelatedUserId);
        });

        modelBuilder.Entity<DBChatListUpdate>(entity =>
        {
            entity.ToTable("chat_list_updates");

            entity.HasKey(e => e.Id).HasName("chat_list_updates_pkey");
            entity.Property(e => e.Id).HasColumnName("id");

            entity.Property(e => e.ChatId)
                  .HasColumnName("chat_id");

            entity.Property(e => e.UserId)
                  .HasColumnName("user_id");

            entity.Property(e => e.IsAdded)
                  .HasColumnName("is_added");

            entity.Property(e => e.Date)
                  .HasColumnName("date");

            entity.HasOne(e => e.User)
                  .WithMany()
                  .HasForeignKey(u => u.UserId);

            entity.HasOne(e => e.Chat)
                  .WithMany()
                  .HasForeignKey(u => u.ChatId);
        });
        modelBuilder.Entity<DBChatUpdate>(entity =>
        {
            entity.ToTable("chat_updates");

            entity.HasKey(e => e.Id).HasName("chat_updates_pkey");
            entity.Property(e => e.Id).HasColumnName("id");

            entity.Property(e => e.ChatId)
                  .HasColumnName("chat_id");

            entity.Property(e => e.UserId)
                  .HasColumnName("user_id");

            entity.HasOne(e => e.User)
                  .WithMany()
                  .HasForeignKey(u => u.UserId);

            entity.HasOne(e => e.Chat)
                  .WithMany()
                  .HasForeignKey(u => u.ChatId);
        });
        OnModelCreatingPartial(modelBuilder);
    }

    public async Task<int> SaveChangesAsync(bool trackChanges)
    {
        //Проверка на то, нужно ли обрабатывать изменения
        if (!trackChanges || !ChangeHandler.IsUpdateTracked || !ChangeTracker.HasChanges())
            return await base.SaveChangesAsync();

        //Получение массива изменённых сущностей
        var changes = GetChangedEntities();

        //Сохранение изменений
        var value = await base.SaveChangesAsync();

        //Передача изменённых сущностей обработчику изменений
        ChangeHandler.HandleChanges(changes);
        return value;
    }

    //Получение массива изменённых сущностей
    private (EntityEntry entity, EntityState state)[] GetChangedEntities()
    {
        var entries = ChangeTracker.Entries();
        return entries.Select(e => (e, e.State)).ToArray();
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
