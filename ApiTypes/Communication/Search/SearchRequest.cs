using CSDTP;
using System.Diagnostics.CodeAnalysis;

namespace ApiTypes.Communication.Search
{
    internal class SearchRequest : ISerializable<SearchRequest>
    {
        public required string SearchQuery { get; init; }

        public SearchRequest()
        {

        }

        [SetsRequiredMembers]
        public SearchRequest(string searchQuery)
        {
            SearchQuery = searchQuery;
        }

        public static SearchRequest Deserialize(BinaryReader reader)
        {
            return new SearchRequest(reader.ReadString());
        }

        public void Serialize(BinaryWriter writer)
        {
            writer.Write(SearchQuery);
        }
    }
}
