using CSDTP;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApiTypes.Users
{
    public class User : ISerializable<User>
    {
        public string Name { get; private set; }

        public int Id { get; private set; }

        public bool IsOnline { get; set; }=false;

        public User(string name, int id)
        {
            Name = name;
            Id = id;
        }

        public static User Deserialize(BinaryReader reader)
        {
            return new User(reader.ReadString(), reader.ReadInt32())
            {
                IsOnline = reader.ReadBoolean()
            };
        }

        public void Serialize(BinaryWriter writer)
        {
            writer.Write(Name);
            writer.Write(Id);
            writer.Write(IsOnline);
        }
    }
}
