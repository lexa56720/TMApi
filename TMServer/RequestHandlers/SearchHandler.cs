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
    internal static class SearchHandler
    {
        public static SerializableArray<User> GetUserByName(ApiData<SearchRequest> request)
        {
            if (!DataConstraints.IsSearchQueryValid(request.Data.SearchQuery))
                return new SerializableArray<User>([]);

            var users = Users.GetUserByName(request.Data.SearchQuery)
                .UnionBy(Users.GetUserByLogin(request.Data.SearchQuery),u=>u.Id)
                .Where(u => u.Id != request.UserId)
                .ToArray();

            if (users.Length==0)
                return new SerializableArray<User>([]);

            return new SerializableArray<User>(DbConverter.Convert(users));
        }
    }
}
