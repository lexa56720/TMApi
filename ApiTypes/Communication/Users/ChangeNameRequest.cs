using CSDTP;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApiTypes.Communication.Users
{
    public class ChangeNameRequest : ISerializable<ChangeNameRequest>
    {
        public string NewName { get; set; }

        public ChangeNameRequest()
        {

        }

        [SetsRequiredMembers]
        public ChangeNameRequest(string newName)
        {
            NewName = newName;
        }
    }
}
