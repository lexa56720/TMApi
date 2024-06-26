﻿using CSDTP;
using System.Diagnostics.CodeAnalysis;

namespace ApiTypes.Communication.Messages
{
    public class LastMessagesRequest : ISerializable<LastMessagesRequest>
    {
        public int ChatId { get; set; }

        public int Offset { get; set; } = 0;

        public int MaxCount { get; set; } = 20;

        [SetsRequiredMembers]
        public LastMessagesRequest(int fromId, int offset = 0, int maxCount = 20)
        {
            ChatId = fromId;
            Offset = offset;
            MaxCount = maxCount;
        }
        public LastMessagesRequest()
        {
        }

    }
}
