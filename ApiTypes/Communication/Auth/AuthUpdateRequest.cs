using CSDTP;

namespace ApiTypes.Communication.Auth
{
    public class AuthUpdateRequest : ISerializable<AuthUpdateRequest>
    {
        public static AuthUpdateRequest Deserialize(BinaryReader reader)
        {
            return new AuthUpdateRequest();
        }

        public void Serialize(BinaryWriter writer)
        {
            return;
        }
    }
}
