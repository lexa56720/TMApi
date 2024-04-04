using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApiTypes
{
    public interface IApiData
    {
        public string Token { get; set; }
        public int UserId { get; set; }
        public int CryptId { get; set; }
    }
}
