using ApiTypes;
using ApiTypes.Communication.LongPolling;
using CSDTP.Packets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMServer.DataBase.Interaction;
using TMServer.DataBase.Tables.LongPolling;

namespace TMServer.RequestHandlers
{
    internal class LongPollHandler
    {
        public static Notification GetUpdates(int userId)
        {
            return new Notification()
            {
                MessagesIds = LongPolling.GetChatUpdate(userId),
            };
        }

        public static void SaveToDB(int userId, IPacket packet)
        {
            using var ms = new MemoryStream();
            using var bw = new BinaryWriter(ms);
            packet.Serialize(bw);
            LongPolling.SaveRequest(userId, ms.ToArray(), packet.GetType().AssemblyQualifiedName);
        }
        public static IPacket? LoadFromDB(int userId)
        {
            var data = LongPolling.LoadRequest(userId);
            using var ms = new MemoryStream(data.RequestPacket);
            using var br = new BinaryReader(ms);
            var packet = Activator.CreateInstance(Type.GetType(data.DataType)) as IPacket;
            return packet.Deserialize(br);
        }
    }
}
