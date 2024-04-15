using ApiTypes.Communication.Messages;
using Microsoft.EntityFrameworkCore;
using System.Data;
using TMServer.DataBase.Tables;
using TMServer.DataBase.Tables.FileTables;

namespace TMServer.DataBase.Interaction
{
    public class Messages
    {
        public async Task<DBMessage> AddMessage(int authorId, string content, int destinationId)
        {
            using var db = new TmdbContext();
            var message = new DBMessage()
            {
                AuthorId = authorId,
                DestinationId = destinationId,
                Content = content,
                IsSystem = false,
                SendTime = DateTime.UtcNow,
            };
            await db.Messages.AddAsync(message);
            await db.SaveChangesAsync(true);

            return message;
        }

        public async Task<DBMessage> AddMessage(int authorId, string content, DBImage[] images, DBBinaryFile[] files, int destinationId)
        {
            using var db = new TmdbContext();

            var message = new DBMessage()
            {
                AuthorId = authorId,
                DestinationId = destinationId,
                Content = content,
                IsSystem = false,
                SendTime = DateTime.UtcNow,
            };
            await db.Messages.AddAsync(message);

            foreach (var image in images)
            {

                message.Attachments.Add(new DBMessageAttachments()
                {
                    AttachmentId = image.Id,
                    Kind = AttachmentKind.Image,
                    Message = message,
                });
            }
            foreach (var file in files)
            {
                message.Attachments.Add(new DBMessageAttachments()
                {
                    AttachmentId = file.Id,
                    Kind = AttachmentKind.File,
                    Message = message,
                });
            }
            await db.SaveChangesAsync(true);
            return message;
        }

        public async Task AddSystemMessage(int chatId, int executorId, ActionKind kind, int? targetId, string text, TmdbContext db)
        {
            var message = new DBMessage()
            {
                AuthorId = executorId,
                DestinationId = chatId,
                Content = text,
                IsSystem = true,
                SendTime = DateTime.UtcNow,
            };
            await AddSystemMessagToDB(executorId, kind, targetId, message, db);
        }
        public async Task AddSystemMessage(DBChat chat, int executorId, ActionKind kind, int? targetId, string text, TmdbContext db)
        {
            var message = new DBMessage()
            {
                AuthorId = executorId,
                Destination = chat,
                Content = text,
                IsSystem = true,
                SendTime = DateTime.UtcNow,
            };
            await AddSystemMessagToDB(executorId, kind, targetId, message, db);
        }
        private async Task AddSystemMessagToDB(int executorId, ActionKind kind, int? targetId, DBMessage message, TmdbContext db)
        {
            var action = new DBMessageAction()
            {
                ExecutorId = executorId,
                TargetId = targetId,
                Kind = kind,
                Message = message,
            };
            await db.Messages.AddAsync(message);
            await db.MessageActions.AddAsync(action);
        }

        public async Task<bool> AddToUnread(int messageId, int chatId)
        {
            using var db = new TmdbContext();
            var members = (await db.Chats.Include(c => c.Members)
                                  .FirstAsync(c => c.Id == chatId)).Members;

            foreach (var member in members)
                await db.UnreadMessages.AddAsync(new DBUnreadMessage()
                {
                    UserId = member.Id,
                    MessageId = messageId,
                });
            return await db.SaveChangesAsync() > 0;
        }



        public async Task<DBMessage[]> GetMessages(int chatId, int offset, int count)
        {
            using var db = new TmdbContext();

            return await db.Messages
                           .Include(m => m.Action)
                           .Include(m => m.Attachments)
                           .Where(m => m.DestinationId == chatId)
                           .OrderByDescending(m => m.SendTime)
                           .ThenByDescending(m => m.Id)
                           .Skip(offset)
                           .Take(count)
                           .ToArrayAsync();
        }
        public async Task<DBMessage[]> GetLastMessages(int[] chatId)
        {
            using var db = new TmdbContext();

            var messages = chatId.Select(id => db.Messages.Where(m => m.DestinationId == id && !m.IsSystem).ToArrayAsync());

            var queredMessages = await Task.WhenAll(messages);
            if (queredMessages == null)
                return [];
            return queredMessages.Select(m => m.MaxBy(x => x.Id))
                                 .Where(m => m != null)
                                 .ToArray();
        }
        public async Task<DBMessage[]> GetMessages(int chatId, int offset, int count, int lastMessageId)
        {
            using var db = new TmdbContext();

            return await db.Messages.Include(m => m.Action)
                                    .Include(m => m.Attachments)
                                    .Where(m => m.DestinationId == chatId)
                                    .OrderByDescending(m => m.SendTime)
                                    .ThenByDescending(m => m.Id)
                                    .Where(m => m.Id < lastMessageId)
                                    .Skip(offset)
                                    .Take(count)
                                    .ToArrayAsync();
        }
        public async Task<DBMessage[]> GetMessages(int[] ids)
        {
            using var db = new TmdbContext();

            return await db.Messages.Include(m => m.Action)
                                    .Include(m => m.Attachments)
                                    .OrderByDescending(m => m.SendTime)
                                    .ThenByDescending(m => m.Id)
                                    .Where(m => ids.Contains(m.Id))
                                    .ToArrayAsync();
        }
        public async Task<bool> ReadAllInChat(int userId, int chatId)
        {
            using var db = new TmdbContext();
            //Чтение всех собщений в чате для юзера userId и отметка о прочитке собщений их авторам 
            var messsagesToMark =
                db.UnreadMessages.Include(um => um.Message)
                                 .Where(um => um.Message.DestinationId == chatId && userId != um.Message.AuthorId &&
                                       (um.UserId == userId || um.UserId == um.Message.AuthorId));

            db.UnreadMessages.RemoveRange(messsagesToMark);
            return await db.SaveChangesAsync(true) > 0;
        }
        public async Task<bool> MarkAsReaded(int userId, int[] ids)
        {
            using var db = new TmdbContext();

            var messsagesToMark = db.UnreadMessages
                                    .Include(um => um.Message)
                                    .Where(um => (um.UserId == userId || um.UserId == um.Message.AuthorId) &&
                                           ids.Contains(um.MessageId));

            db.UnreadMessages.RemoveRange(messsagesToMark);
            try
            {
                return await db.SaveChangesAsync(true) > 0;
            }
            catch (DbUpdateConcurrencyException)
            {
                return true;
            }
        }
        public async Task<bool> IsMessageReaded(int userId, int messageId)
        {
            using var db = new TmdbContext();
            return await db.UnreadMessages.Include(m => m.Message).
                AllAsync(m => (m.UserId != userId || m.MessageId != messageId)
                           || (m.UserId == m.Message.AuthorId && m.UserId != userId));
        }
        public async Task<bool[]> IsMessageReaded(int userId, IEnumerable<int> messageIds)
        {
            return await Task.WhenAll(messageIds.Select(id => IsMessageReaded(userId, id)));
        }
    }
}
