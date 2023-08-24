using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApiTypes.Packets
{
    public class IdHolder
    {
        public IdHolder(int id)
        {
            InstanceValue = id;
        }
        public int InstanceValue { get; }
        public static int Value { get; set; }
    }
}
