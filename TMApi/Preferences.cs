using ApiTypes.Communication.Packets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TMApi
{
    internal static class Preferences
    {   
        public static int CtyptId 
        { 
            get => IdHolder.Value;
            set
            {
                IdHolder.Value = value;
            }
        }
        private static int ctyptId;
    }
}
