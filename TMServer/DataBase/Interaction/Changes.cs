using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMServer.DataBase.Tables.LongPolling;
using TMServer.DataBase.Tables;

namespace TMServer.DataBase.Interaction
{
    public class Changes
    {
        public async Task<IEnumerable<int>> HandleModifiedChat(DBChat entity, TmdbContext context)
        {
            var usersToNotify = await GetUsersForChatUpdate(entity.Id, context);
            await UpdateChatForUsers(entity.Id, usersToNotify, context);
            return usersToNotify;
        }
        public async Task<IEnumerable<int>> HandleModifiedUser(DBUser user, TmdbContext context)
        {
            return await Users.GetAllRelatedUsers(user.Id);
        }

        public async Task<IEnumerable<int>> HandleRemovedFriend(DBFriend entity, TmdbContext context)
        {
            await UpdateFriendList(false, entity.SenderId, entity.DestId, context);
            await UpdateFriendList(false, entity.DestId, entity.SenderId, context);
            return [entity.DestId, entity.SenderId];
        }
        public async Task<IEnumerable<int>> HandleNewFriend(DBFriend entity, TmdbContext context)
        {
            await UpdateFriendList(true, entity.SenderId, entity.DestId, context);
            await UpdateFriendList(true, entity.DestId, entity.SenderId, context);
            return [entity.DestId, entity.SenderId];
        }
        private async Task UpdateFriendList(bool isAdded, int userId, int friendId, TmdbContext context)
        {
            var update = await context.FriendListUpdates.Where(f => f.UserId == userId && f.FriendId == friendId)
                                                        .SingleOrDefaultAsync();
            await context.FriendListUpdates.AddAsync(new DBFriendListUpdate()
            {
                FriendId = friendId,
                UserId = userId,
                IsAdded = isAdded,
                Date = DateTime.UtcNow,
            });
        }

        public async Task<IEnumerable<int>> HandleNewFriendRequest(DBFriendRequest entity, TmdbContext context)
        {
            await context.FriendRequestUpdates.AddAsync(new DBFriendRequestUpdate()
            {
                RequestId = entity.Id,
                UserId = entity.ReceiverId
            });
            return [entity.ReceiverId];
        }
        public async Task<IEnumerable<int>> HandleNewMessage(DBMessage message, TmdbContext context)
        {
            var chatMembers = (await context.Chats.Include(c => c.Members)
                                           .FirstAsync(c => c.Id == message.DestinationId))
                                           .Members.Select(m => m.Id);

            //Добавление уведомлений в бд
            foreach (var member in chatMembers)
                if (message.IsSystem || member != message.AuthorId)
                    await context.NewMessageUpdates.AddAsync(new DBNewMessageUpdate()
                    {
                        MessageId = message.Id,
                        UserId = member
                    });
            return chatMembers;
        }
        public async Task<IEnumerable<int>> HandleMessageRead(DBUnreadMessage message, TmdbContext context)
        {
            await context.MessageStatusUpdates.AddAsync(new DBMessageStatusUpdate()
            {
                MessageId = message.MessageId,
                UserId = message.UserId,
            });
            return [message.UserId];
        }

        public async Task<IEnumerable<int>> HandleNewChatMember(DBChatUser entity, TmdbContext context)
        {
            return await UpdateChatMembers(context, entity.ChatId, entity.UserId, true);
        }
        public async Task<IEnumerable<int>> HandleRemovedChatMember(DBChatUser entity, TmdbContext context)
        {
            return await UpdateChatMembers(context, entity.ChatId, entity.UserId, false);
        }
        private async Task<IEnumerable<int>> UpdateChatMembers(TmdbContext context, int chatId, int userId, bool isAdded)
        {
            await context.ChatListUpdates.AddAsync(new DBChatListUpdate()
            {
                ChatId = chatId,
                IsAdded = isAdded,
                UserId = userId,
                Date = DateTime.UtcNow,
            });
            var usersToNotify = await GetUsersForChatUpdate(chatId, context);
            await UpdateChatForUsers(chatId, usersToNotify, context);
            usersToNotify.Add(userId);
            return usersToNotify;
        }


        public async Task<List<int>> GetUsersForChatUpdate(int chatId, TmdbContext context)
        {
            var usersToNotify = new List<int>();
            usersToNotify.AddRange((await context.Chats.Include(c => c.Members)
                                                .SingleAsync(c => c.Id == chatId))
                                                .Members.Select(m => m.Id));

            usersToNotify.AddRange(await context.ChatInvites.Where(i => i.ChatId == chatId)
                                                            .Select(i => i.ToUserId).ToArrayAsync());

            return usersToNotify;
        }
        private async Task UpdateChatForUsers(int chatId, IEnumerable<int> userIds, TmdbContext context)
        {
            foreach (var userId in userIds)
            {
                await context.ChatUpdates.AddAsync(new DBChatUpdate()
                {
                    ChatId = chatId,
                    UserId = userId,
                });
            }
        }

        public async Task<IEnumerable<int>> HandleNewChat(DBChat chat, TmdbContext context)
        {
            var members = chat.Members;
            foreach (var member in members)
                await context.ChatListUpdates.AddAsync(new DBChatListUpdate()
                {
                    ChatId = chat.Id,
                    UserId = member.Id,
                    IsAdded = true,
                    Date = DateTime.UtcNow,
                });
            return members.Select(m => m.Id);
        }
        public async Task<IEnumerable<int>> HandleNewChatInvite(DBChatInvite invite, TmdbContext context)
        {
            await context.ChatInviteUpdates.AddAsync(new DBChatInviteUpdate()
            {
                ChatInviteId = invite.Id,
                UserId = invite.ToUserId,
            });
            return [invite.ToUserId];
        }

    }
}
