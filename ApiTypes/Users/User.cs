using CSDTP;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApiTypes.Users
{
    public class User : ISerializable<User>
    {
        public required string Name { get; init; }

        public required int Id { get; init; }

        public required bool IsOnline { get; init; } =false;

        [SetsRequiredMembers]
        public User(string name, int id, bool isOnline)
        {
            Name = name;
            Id = id;
            IsOnline = isOnline;
        }
        public User()
        {

        }

        public static User Deserialize(BinaryReader reader)
        {
            return new User()
            {
                Name= reader.ReadString(),
                Id= reader.ReadInt32(),
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
