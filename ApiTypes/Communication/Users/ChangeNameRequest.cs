using CSDTP;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApiTypes.Communication.Users
{
    public class ChangeNameRequest : ISerializable<ChangeNameRequest>
    {
        public string NewName { get; set; }
        public void Serialize(BinaryWriter writer)
        {
            writer.Write(NewName);
        }

        public ChangeNameRequest()
        {

        }

        [SetsRequiredMembers]
        public ChangeNameRequest(string newName)
        {
            NewName = newName;
        }
        public static ChangeNameRequest Deserialize(BinaryReader reader)
        {
            return new ChangeNameRequest()
            {
                NewName = reader.ReadString()
            };
        }
    }
}
