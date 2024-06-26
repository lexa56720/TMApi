﻿using ApiTypes.Communication.Medias;
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
        ChatRenamed, 
        NewCover,
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

        public PhotoLink[] Photos { get; set; } = [];
        public FileLink[] Files { get; set; } = [];

        public Message()
        {

        }

        public Message(int id, int authorId, int destinationId, string text,
                       DateTime sendTime, bool isReaded, PhotoLink[] photos, FileLink[] files)
        {
            Id = id;
            AuthorId = authorId;
            DestinationId = destinationId;
            Text = text;
            SendTime = sendTime;
            IsReaded = isReaded;
            Photos = photos;
            Files = files;
        }

        public Message(int id, int authorId, int destinationId, string text,DateTime sendTime,
                       bool isReaded, ActionKind action, int executorId, int targetId)
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
