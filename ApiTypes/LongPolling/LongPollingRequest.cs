﻿using CSDTP;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApiTypes.LongPolling
{
    public class LongPollingRequest : ISerializable<LongPollingRequest>
    {
        public static LongPollingRequest Deserialize(BinaryReader reader)
        {
            return new LongPollingRequest();
        }

        public void Serialize(BinaryWriter writer)
        {
            return;
        }
    }
}
