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
        private readonly Users Users;
        private readonly DbConverter Converter;

        public SearchHandler(Users users, DbConverter converter)
        {
            Users = users;
            Converter = converter;
        }
        public async Task<SerializableArray<User>> GetUserByName(ApiData<SearchRequest> request)
        {
            if (!DataConstraints.IsSearchQueryValid(request.Data.SearchQuery))
                return new SerializableArray<User>([]);

            var users = (await Users.GetUserByName(request.Data.SearchQuery))
                .UnionBy(await Users.GetUserByLogin(request.Data.SearchQuery), u => u.Id)
                .Where(u => u.Id != request.UserId)
                .ToArray();

            if (users.Length == 0)
                return new SerializableArray<User>([]);

            return new SerializableArray<User>(await Converter.Convert(users));
        }
    }
}
