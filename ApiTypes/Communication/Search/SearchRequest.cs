using CSDTP;
using System.Diagnostics.CodeAnalysis;

namespace ApiTypes.Communication.Search
{
    public class SearchRequest : ISerializable<SearchRequest>
    {
        public string SearchQuery { get; set; }

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
