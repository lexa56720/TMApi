using ApiTypes;
using ApiTypes.Communication.Packets;
using CSDTP.Cryptography.Algorithms;
using CSDTP.Cryptography.Providers;
using CSDTP.Packets;
using CSDTP.Requests.RequestHeaders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TMApi
{
    internal class ApiEncryptProvider : IEncryptProvider
    {
        private readonly Dictionary<int, AesEncrypter> Crypts;
        private bool IsDisposed;

        private int LastId;
        public ApiEncryptProvider(int cryptId, byte[] key)
        {
            Crypts = [];
            Add(cryptId, key);
        }
        public void Dispose()
        {
            if (IsDisposed)
                return;

            foreach (var encrypter in Crypts.Values)
                encrypter.Dispose();
            Crypts.Clear();

            IsDisposed = true;
        }

        public void Add(int cryptId, byte[] key)
        {
            Crypts.Add(cryptId, new AesEncrypter(key));
            LastId = cryptId;
        }
        public void DisposeEncrypter(IEncrypter encrypter)
        {
            return;
        }
        public IEncrypter? GetDecrypter(ReadOnlySpan<byte> bytes)
        {
            var cryptId = BitConverter.ToInt32(bytes.Slice(bytes.Length - 4, 4));
            if (!Crypts.TryGetValue(cryptId, out var encrypter))
                return null;
            return encrypter;
        }
        public IEncrypter? GetEncrypter(IPacketInfo responsePacket, IPacketInfo? requestPacket = null)
        {
            if (responsePacket is not ITMPacket packet)
                return null;

            var (cryptId, aesCrypter) = GetLast();
            packet.Id = cryptId;
            if (packet is IPacket<IRequestContainer> castedPacket && castedPacket.Data.DataObj is IApiData apiData)
                apiData.CryptId = cryptId;
            return aesCrypter;
        }

        public (int cryptId, AesEncrypter encrypter) GetLast()
        {
            return (LastId, Crypts[LastId]);
        }
    }
}
