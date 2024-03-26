using ApiTypes.Communication.Medias;
using CSDTP;
using System.Diagnostics.CodeAnalysis;

namespace ApiTypes.Communication.Messages
{
    public enum ActionKind
    {
        None,
        UserInvite,
        UserEnter,
        UserLeave,
        UserKicked,
        ChatCreated,
    }
    public class Message : ISerializable<Message>
    {
        public int Id { get; set; }

        public int AuthorId { get; set; }

        public int DestinationId { get; set; }

        public string Text { get; set; } = string.Empty;

        public DateTime SendTime { get; set; }

        public bool IsReaded { get; set; }

        public ActionKind Kind { get; set; } = ActionKind.None;

        public int ExecutorId { get; set; } = -1;

        public int TargetId { get; set; } = -1;

        public Photo[] Photos { get; set; } = [];

        public Message()
        {

        }

        public Message(int id, int authorId, int destinationId, string text,
                       DateTime sendTime, bool isReaded, Photo[] photos)
        {
            Id = id;
            AuthorId = authorId;
            DestinationId = destinationId;
            Text = text;
            SendTime = sendTime;
            IsReaded = isReaded;
            Photos = photos;
        }

        public Message(int id, int authorId, int destinationId, string text,
               DateTime sendTime, bool isReaded, ActionKind action, int executorId, int targetId)
        {
            Id = id;
            AuthorId = authorId;
            DestinationId = destinationId;
            Text = text;
            SendTime = sendTime;
            IsReaded = isReaded;
            ExecutorId = executorId;
            TargetId = targetId;
            Kind = action;
        }
    }
}
