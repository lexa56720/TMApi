﻿using CSDTP;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApiTypes.Communication.Chats
{
    public class ChatRequest:ISerializable<ChatRequest>
    {
        public required int[] Ids { get; set; }

        [SetsRequiredMembers]
        public ChatRequest(int[] ids)
        {
            Ids = ids;
        }

        [SetsRequiredMembers]
        public ChatRequest() 
        { 
        }
    }
}
