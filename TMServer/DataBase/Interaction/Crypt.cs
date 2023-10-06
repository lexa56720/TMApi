﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMServer.DataBase.Tables;

namespace TMServer.DataBase.Interaction
{
    internal static class Crypt
    {
        public static int SaveRsaKeyPair(string serverPrivateKey, string clientPublicKey)
        {
            using var db = new TmdbContext();

            var rsa = new DBRsa()
            {
                PrivateServerKey = serverPrivateKey,
                PublicClientKey = clientPublicKey,
                CreateDate = DateTime.UtcNow,
            };
            db.RsaCrypts.Add(rsa);
            db.SaveChanges();

            return rsa.Id;
        }

        public static DBRsa? GetRsaKeysById(int rsaId)
        {
            using var db = new TmdbContext();
            return db.RsaCrypts.Find(rsaId);
        }

        public static void SetDeprecated(int cryptId)
        {
            using var db = new TmdbContext();

            var aes = db.AesCrypts.Find(cryptId);

            if (aes != null)
            {
                aes.IsDeprecated = true;
                aes.DeprecatedDate = DateTime.UtcNow;
            }

            db.SaveChanges();
        }

        public static byte[]? GetAesKey(int cryptId)
        {
            using var db = new TmdbContext();

            var dbAes = db.AesCrypts.SingleOrDefault(a => 
            a.Id == cryptId && a.IsDeprecated==false);
            if (dbAes == null)
                return null;

            return dbAes.AesKey;
        }
    }
}
