﻿namespace ApiTypes.Communication.Packets
{
    public class IdHolder
    {
        public IdHolder(int id)
        {
            InstanceValue = id;
        }
        public int InstanceValue { get; set; }
        public static int Value { get; set; }
    }
}
