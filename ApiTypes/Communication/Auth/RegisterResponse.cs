using CSDTP;

namespace ApiTypes.Communication.Auth
{
    public class RegisterResponse : ISerializable<RegisterResponse>
    {
        public required bool IsSuccessful { get; init; }

        public RegisterResponse()
        {

        }

        public void Serialize(BinaryWriter writer)
        {
            writer.Write(IsSuccessful);
        }

        public static RegisterResponse Deserialize(BinaryReader reader)
        {
            return new RegisterResponse()
            {
                IsSuccessful = reader.ReadBoolean(),
            };
        }
    }
}
