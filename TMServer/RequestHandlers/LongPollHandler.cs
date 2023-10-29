using ApiTypes;
using ApiTypes.Communication.LongPolling;
using ApiTypes.Communication.Packets;
using CSDTP.Packets;
using CSDTP.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
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
            bw.Write(((ITMPacket)packet).Id.InstanceValue);
            bw.WriteBytes(packet.Source.GetAddressBytes());
            LongPolling.SaveRequest(userId, ms.ToArray(), packet.GetType().AssemblyQualifiedName);
        }

        public static IPacket? LoadFromDB(int userId)
        {
            var data = LongPolling.LoadRequest(userId);
            if (data == null)
                return null;
            using var ms = new MemoryStream(data.RequestPacket);
            using var br = new BinaryReader(ms);
            br.ReadByteArray();

            var packet = Activator.CreateInstance(Type.GetType(data.DataType)) as IPacket;
            packet = packet.Deserialize(br);
            ((ITMPacket)packet).Id.InstanceValue=br.ReadInt32();
            packet.Source = new IPAddress(br.ReadByteArray());

            return packet;
        }

        public static bool IsHaveNotifications(int userId)
        {
            return LongPolling.IsHaveUpdates(userId);
        }
    }
}
