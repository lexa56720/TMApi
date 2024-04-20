using Microsoft.EntityFrameworkCore;
using PerformanceUtils.Collections;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMServer.DataBase.MemoryEntities;

namespace TMServer.DataBase.Interaction
{
    public class Crypts : IDisposable
    {
        private readonly LifeTimeDictionary<int, RamRsa> RsaKeys;
        private readonly LifeTimeDictionary<int, RamAes> AesKeys;
        private readonly TimeSpan RsaLifeTime;
        private readonly TimeSpan AesLifeTime;

        private int RsaId;
        private int AesId;

        public bool IsDisposed;

        public Crypts(TimeSpan rsaLifeTime, TimeSpan aesLifeTime)
        {
            RsaKeys = new();
            AesKeys = new();
            RsaLifeTime = rsaLifeTime;
            AesLifeTime = aesLifeTime;
        }
        public void Dispose()
        {
            if (IsDisposed)
                return;
            AesKeys.Clear();
            RsaKeys.Clear();
            IsDisposed = true;
        }
        public RamRsa AddRsaKeys(string serverPrivateKey, string clientPublicKey)
        {
            var expiration = DateTime.UtcNow + RsaLifeTime;
            var rsa = new RamRsa()
            {
                Id = Interlocked.Increment(ref RsaId),
                PrivateServerKey = serverPrivateKey,
                PublicClientKey = clientPublicKey,
                Expiration = expiration,
            };
            RsaKeys.TryAdd(rsa.Id, rsa, RsaLifeTime);
            return rsa;
        }
        public RamAes AddAes(int userId, byte[] aesKey)
        {
            var aes = new RamAes()
            {
                Id = Interlocked.Increment(ref AesId),
                AesKey = (byte[])aesKey.Clone(),
                Expiration = DateTime.MaxValue,
                UserId = userId,
            };
            AesKeys.TryAdd(aes.Id, aes, AesLifeTime);
            return aes;
        }
        public RamRsa? GetRsaKeysById(int rsaId)
        {
            return RsaKeys.TryGetValue(rsaId, out var rsa) ? rsa : null;
        }

        public bool SetDeprecated(int cryptId)
        {
            if (!AesKeys.TryGetValue(cryptId, out var aes))
                return false;
            if (aes.Expiration - DateTime.UtcNow > TimeSpan.FromHours(1))
                AesKeys.UpdateLifetime(cryptId, TimeSpan.FromHours(1));
            return true;
        }

        public RamAes? GetAesKey(int cryptId)
        {
            if (AesKeys.TryGetValue(cryptId, out var aes))
                return aes;
            return null;
        }

    }
}
