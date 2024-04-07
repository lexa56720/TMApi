using ApiTypes.Communication.Messages;
using Microsoft.EntityFrameworkCore;
using System.Data;
using TMServer.DataBase.Tables;
using TMServer.DataBase.Tables.FileTables;

namespace TMServer.DataBase.Interaction
{
    public class Messages
    {
        public DBMessage AddMessage(int authorId, string content, int destinationId)
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
            db.Messages.Add(message);
            db.SaveChanges(true);

            return message;
        }

        public DBMessage AddMessage(int authorId, string content, DBImage[] images, DBBinaryFile[] files, int destinationId)
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
            db.Messages.Add(message);

            foreach (var image in images)
            {
                db.MessageAttachments.Add(new DBMessageAttachments()
                { 
                    AttachmentId=image.Id,
                    Kind=AttachmentKind.Image,
                    Message=message,
                });
            }
            foreach (var file in files)
            {
                db.MessageAttachments.Add(new DBMessageAttachments()
                {
                    AttachmentId = file.Id,
                    Kind = AttachmentKind.File,
                    Message = message,
                });
            }
            db.SaveChanges(true);
            return message;
        }

        public void AddSystemMessage(int chatId, int executorId, ActionKind kind, int? targetId, string text, TmdbContext db)
        {
            var message = new DBMessage()
            {
                AuthorId = executorId,
                DestinationId = chatId,
                Content = text,
                IsSystem = true,
                SendTime = DateTime.UtcNow,
            };
            AddSystemMessagToDB(executorId, kind, targetId, message, db);
        }
        public void AddSystemMessage(DBChat chat, int executorId, ActionKind kind, int? targetId, string text, TmdbContext db)
        {
            var message = new DBMessage()
            {
                AuthorId = executorId,
                Destination = chat,
                Content = text,
                IsSystem = true,
                SendTime = DateTime.UtcNow,
            };
            AddSystemMessagToDB(executorId, kind, targetId, message, db);
        }
        private void AddSystemMessagToDB(int executorId, ActionKind kind, int? targetId, DBMessage message, TmdbContext db)
        {
            var action = new DBMessageAction()
            {
                ExecutorId = executorId,
                TargetId = targetId,
                Kind = kind,
                Message = message,
            };
            db.Messages.Add(message);
            db.MessageActions.Add(action);
        }

        public bool AddToUnread(int messageId, int chatId)
        {
            using var db = new TmdbContext();
            var members = db.Chats.Include(c => c.Members)
                                  .First(c => c.Id == chatId).Members;

            foreach (var member in members)
                db.UnreadMessages.Add(new DBUnreadMessage()
                {
                    UserId = member.Id,
                    MessageId = messageId,
                });
            return db.SaveChanges() > 0;
        }



        public DBMessage[] GetMessages(int chatId, int offset, int count)
        {
            using var db = new TmdbContext();

            return db.Messages
                .Include(m => m.Action)
                .Where(m => m.DestinationId == chatId)
                .OrderByDescending(m => m.SendTime)
                .ThenByDescending(m => m.Id)
                .Skip(offset)
                .Take(count)
                .ToArray();
        }
        public DBMessage[] GetLastMessages(int[] chatId)
        {
            using var db = new TmdbContext();

            return chatId.Select(id => db.Messages.Where(m => m.DestinationId == id && !m.IsSystem)
                                                  .AsEnumerable()
                                                  .MaxBy(m => m.Id))
                         .Where(m => m != null)
                         .ToArray();
        }
        public DBMessage[] GetMessages(int chatId, int offset, int count, int lastMessageId)
        {
            using var db = new TmdbContext();

            return db.Messages.Include(m => m.Action)
                              .Where(m => m.DestinationId == chatId)
                              .OrderByDescending(m => m.SendTime)
                              .ThenByDescending(m => m.Id)
                              .Where(m => m.Id < lastMessageId)
                              .Skip(offset)
                              .Take(count)
                              .ToArray();
        }
        public DBMessage[] GetMessages(int[] ids)
        {
            using var db = new TmdbContext();

            return db.Messages.Include(m => m.Action)
                              .OrderByDescending(m => m.SendTime)
                              .ThenByDescending(m => m.Id)
                              .Where(m => ids.Contains(m.Id))
                              .ToArray();
        }
        public bool ReadAllInChat(int userId, int chatId)
        {
            using var db = new TmdbContext();
            //Чтение всех собщений в чате для юзера userId и отметка о прочитке собщений их авторам 
            var messsagesToMark =
                db.UnreadMessages.Include(um => um.Message)
                                 .Where(um => um.Message.DestinationId == chatId && userId != um.Message.AuthorId &&
                                       (um.UserId == userId || um.UserId == um.Message.AuthorId));

            db.UnreadMessages.RemoveRange(messsagesToMark);
            return db.SaveChanges(true) > 0;
        }
        public bool MarkAsReaded(int userId, int[] ids)
        {
            using var db = new TmdbContext();

            var messsagesToMark = db.UnreadMessages.Include(um => um.Message)
                                   .Where(um => (um.UserId == userId || um.UserId == um.Message.AuthorId) &&
                                          ids.Contains(um.MessageId));

            db.UnreadMessages.RemoveRange(messsagesToMark);
            try
            {
                return db.SaveChanges(true) > 0;
            }
            catch (DbUpdateConcurrencyException)
            {
                return true;
            }
        }
        public bool IsMessageReaded(int userId, int messageId)
        {
            using var db = new TmdbContext();
            return db.UnreadMessages.Include(m => m.Message).
                All(m => (m.UserId != userId || m.MessageId != messageId) || (m.UserId == m.Message.AuthorId && m.UserId != userId));
        }
        public bool[] IsMessageReaded(int userId, IEnumerable<int> messageIds)
        {
            return messageIds.Select(id => IsMessageReaded(userId, id))
                             .ToArray();
        }
    }
}
