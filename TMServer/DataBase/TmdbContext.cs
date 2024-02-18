using ApiTypes.Communication.LongPolling;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using TMServer.DataBase.Tables;
using TMServer.DataBase.Tables.LongPolling;

namespace TMServer.DataBase;

public partial class TmdbContext : DbContext
{
    public static DBChangeHandler ChangeHandler { get; private set; } = new DBChangeHandler();

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

    public virtual DbSet<DBChatInviteUpdate> ChatInviteUpdates { get; set; }
    public virtual DbSet<DBChatUpdate> ChatUpdates { get; set; }
    public virtual DbSet<DBFriendRequestUpdate> FriendRequestUpdates { get; set; }
    public virtual DbSet<DBFriendProfileUpdate> FriendProfileUpdates { get; set; }
    public virtual DbSet<DBFriendListUpdate> FriendListUpdate { get; set; }



    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseNpgsql(GlobalSettings.ConnectionString);
        //optionsBuilder.LogTo(Console.WriteLine);
    }

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

            entity.HasOne(e => e.Receiver)
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

            entity.Property(e => e.Expiration).HasColumnName("expiration_date");
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

            entity.Property(e => e.SenderId)
                .ValueGeneratedNever()
                .HasColumnName("user_id_one");

            entity.Property(e => e.ReceiverId)
              .ValueGeneratedNever()
              .HasColumnName("user_id_two");

            entity.HasOne(e => e.Sender)
            .WithOne().HasForeignKey<DBFriendRequest>(r => r.SenderId);

            entity.HasOne(e => e.Receiver)
           .WithOne().HasForeignKey<DBFriendRequest>(r => r.ReceiverId);
        });
        modelBuilder.Entity<DBMessageMedia>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("message_medias_pkey");
            entity.ToTable("message_medias");
            entity.Property(e => e.Id).HasColumnName("id");

            entity.Property(e => e.MediaType).HasColumnName("type");

            entity.Property(e => e.Data).HasColumnName("data");

        });


        modelBuilder.Entity<DBChatUpdate>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("chat_updates_pkey");
            entity.ToTable("chat_updates");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.MessageId).HasColumnName("message_id");
            entity.Property(e => e.UserId).HasColumnName("user_id");

            entity.HasOne(e => e.User)
                  .WithMany()
                  .HasForeignKey(u => u.UserId);

            entity.HasOne(e => e.Message)
                  .WithMany()
                  .HasForeignKey(u => u.MessageId);
        });
        modelBuilder.Entity<DBChatInviteUpdate>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("chat_invite_updates_pkey");
            entity.ToTable("chat_invite_updates");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.ChatInviteId).HasColumnName("invite_id");
            entity.Property(e => e.UserId).HasColumnName("user_id");

            entity.HasOne(e => e.User)
                  .WithOne()
                  .HasForeignKey<DBChatInviteUpdate>(u => u.UserId);

            entity.HasOne(e => e.Invite)
                  .WithOne()
                  .HasForeignKey<DBChatInviteUpdate>(u => u.ChatInviteId);
        });
        modelBuilder.Entity<DBFriendProfileUpdate>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("friend_profile_updates_pkey");
            entity.ToTable("friend_profile_updates");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.FriendId).HasColumnName("friend_id");
            entity.Property(e => e.UserId).HasColumnName("user_id");

            entity.HasOne(e => e.User)
                  .WithOne()
                  .HasForeignKey<DBFriendProfileUpdate>(u => u.UserId);

            entity.HasOne(e => e.Friend)
                  .WithOne()
                  .HasForeignKey<DBFriendProfileUpdate>(u => u.FriendId);
        });
        modelBuilder.Entity<DBFriendRequestUpdate>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("friend_request_updates_pkey");
            entity.ToTable("friend_request_updates");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.RequestId).HasColumnName("request_id");
            entity.Property(e => e.UserId).HasColumnName("user_id");

            entity.HasOne(e => e.User)
                  .WithOne()
                  .HasForeignKey<DBFriendRequestUpdate>(u => u.UserId);

            entity.HasOne(e => e.FriendRequest)
                  .WithOne()
                  .HasForeignKey<DBFriendRequestUpdate>(u => u.RequestId);
        });
        modelBuilder.Entity<DBFriendListUpdate>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("friend_list_updates_pkey");
            entity.ToTable("friend_list_updates");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.FriendId).HasColumnName("friend_id");
            entity.Property(e => e.UserId).HasColumnName("user_id");

            entity.HasOne(e => e.User)
                  .WithOne()
                  .HasForeignKey<DBFriendListUpdate>(u => u.UserId);

            entity.HasOne(e => e.Friend)
                  .WithOne()
                  .HasForeignKey<DBFriendListUpdate>(u => u.FriendId);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    public override int SaveChanges()
    {
        if (!ChangeHandler.IsUpdateTracked|| !ChangeTracker.HasChanges())
            return base.SaveChanges();

        var changes = GetChangedEntities();
        var value = base.SaveChanges();
        ChangeHandler.HandleChanges(changes);
        return value;
    }

    private (EntityEntry entity, EntityState state)[] GetChangedEntities()
    {
        var entries = ChangeTracker.Entries();
        return entries.Select(e => (e, e.State)).ToArray();
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
