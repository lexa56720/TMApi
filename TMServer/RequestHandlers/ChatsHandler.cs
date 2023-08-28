using ApiTypes;
using ApiTypes.Communication.BaseTypes;
using ApiTypes.Communication.Chats;
using ApiTypes.Shared;
using TMServer.DataBase;

namespace TMServer.RequestHandlers
{
    internal class ChatsHandler
    {
        public static Chat? CreateChat(ApiData<ChatCreationRequest> request)
        {
            if (!Chats.IsCanCreate(request.UserId, request.Data.Members))
                return null;

            var members = new List<int>(request.Data.Members);
            members.Insert(0, request.UserId);
            var chatMembers = members.Distinct().ToArray();

            if (chatMembers.Length < 2 || !DataConstraints.IsNameLegal(request.Data.ChatName))
                return null;

            var chat = Chats.CreateChat(request.Data.ChatName, chatMembers);
            return new Chat(chat.ChatId, chat.Admin.Id, chat.Members.Select(m => m.Id).ToArray());
        }

        public static Chat GetChat(ApiData<IntContainer> request)
        {

        }
        public static SerializableArray<Chat> GetChats(ApiData<IntArrayContainer> request)
        {

        }
        public static void SendChatInvite(ApiData<ChatInvite> request)
        {

        }


        public static ChatInvite GetChatInvite(ApiData<IntContainer> request)
        {

        }
        public static SerializableArray<ChatInvite> GetChatInvites(ApiData<IntArrayContainer> request)
        {

        }

        public static void ChatInviteResponse(ApiData<RequestResponse> request)
        {

        }
    }
}
