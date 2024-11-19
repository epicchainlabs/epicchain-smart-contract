using EpicChain.SmartContract.Framework;
using EpicChain.SmartContract.Framework.Services.EpicChain;
using System.ComponentModel;

namespace Domain
{
    [Features(ContractFeatures.HasStorage)]
    public class Contract1 : SmartContract
    {
        private static readonly byte[] Admin = "AdminPublicKeyInHex".ToScriptHash();
        private const int ExpirationTime = 30 * 24 * 60 * 60; // 30 days in seconds

        public static object Main(string method, params object[] args)
        {
            return method switch
            {
                "query" => Query((string)args[0]),
                "register" => Register((string)args[0], (byte[])args[1]),
                "transfer" => Transfer((string)args[0], (byte[])args[1]),
                "delete" => Delete((string)args[0]),
                "renew" => Renew((string)args[0], (byte[])args[1]),
                "checkExpiration" => CheckExpiration((string)args[0]),
                _ => false,
            };
        }

        [DisplayName("query")]
        private static byte[] Query(string domain)
        {
            return Storage.Get(Storage.CurrentContext, domain);
        }

        [DisplayName("register")]
        private static bool Register(string domain, byte[] owner)
        {
            if (!Runtime.CheckWitness(owner)) return false;
            byte[] value = Storage.Get(Storage.CurrentContext, domain);
            if (value != null) return false;

            Storage.Put(Storage.CurrentContext, domain, owner);
            Storage.Put(Storage.CurrentContext, domain + ":expiration", Runtime.Time + ExpirationTime);
            return true;
        }

        [DisplayName("transfer")]
        private static bool Transfer(string domain, byte[] to)
        {
            if (!Runtime.CheckWitness(to)) return false;
            byte[] from = Storage.Get(Storage.CurrentContext, domain);
            if (from == null) return false;
            if (!Runtime.CheckWitness(from)) return false;

            Storage.Put(Storage.CurrentContext, domain, to);
            return true;
        }

        [DisplayName("delete")]
        private static bool Delete(string domain)
        {
            byte[] owner = Storage.Get(Storage.CurrentContext, domain);
            if (owner == null) return false;
            if (!Runtime.CheckWitness(owner)) return false;

            Storage.Delete(Storage.CurrentContext, domain);
            Storage.Delete(Storage.CurrentContext, domain + ":expiration");
            return true;
        }

        [DisplayName("renew")]
        private static bool Renew(string domain, byte[] owner)
        {
            if (!Runtime.CheckWitness(owner)) return false;
            byte[] currentOwner = Storage.Get(Storage.CurrentContext, domain);
            if (currentOwner == null || !currentOwner.Equals(owner)) return false;

            Storage.Put(Storage.CurrentContext, domain + ":expiration", Runtime.Time + ExpirationTime);
            return true;
        }

        [DisplayName("checkExpiration")]
        private static bool CheckExpiration(string domain)
        {
            byte[] expirationData = Storage.Get(Storage.CurrentContext, domain + ":expiration");
            if (expirationData == null) return false;

            long expirationTime = (long)expirationData;
            if (Runtime.Time > expirationTime)
            {
                Storage.Delete(Storage.CurrentContext, domain);
                Storage.Delete(Storage.CurrentContext, domain + ":expiration");
                return false; // Domain has expired
            }

            return true; // Domain is still valid
        }
    }
}
