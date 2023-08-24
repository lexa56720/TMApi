using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace ApiTypes.Shared
{
    internal class DataConstraints
    {
        public bool IsLoginLegal(string login)
        {
            return login.Length < 128;
        }

        public bool IsPasswordLegal(string password)
        {
            return password.Length < 512;
        }

        public bool IsNameLegal(string name)
        {
            return name.Length < 128;
        }

        public bool IsMessageLegal(string message)
        {
            return message.Length < 512;
        }
    }
}
