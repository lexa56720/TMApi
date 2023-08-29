using ApiTypes;
using ApiTypes.Communication.BaseTypes;
using ApiTypes.Communication.Search;
using ApiTypes.Communication.Users;
using ApiTypes.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMServer.DataBase.Interaction;

namespace TMServer.RequestHandlers
{
    internal class SearchHandler
    {
        public static SerializableArray<User> GetUserByName(ApiData<SearchRequest> request)
        {
            if (!DataConstraints.IsSearchQueryValid(request.Data.SearchQuery))
                return new SerializableArray<User>(Array.Empty<User>());

            var users = Users.GetUserByName(request.Data.SearchQuery);
            if (!users.Any())
                return new SerializableArray<User>(Array.Empty<User>());

            return new SerializableArray<User>(users.Select(u => new User(u.Name, u.Id, u.IsOnline)).ToArray());
        }
    }
}
