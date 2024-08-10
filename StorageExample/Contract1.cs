using Neo.SmartContract.Framework;
using Neo.SmartContract.Framework.Services.Neo;
using Neo.SmartContract.Framework.Services.System;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Numerics;

namespace StorageExample
{
    [Features(ContractFeatures.HasStorage)]
    public class Contract1 : SmartContract
    {
        [DisplayName("saved")]
        public static event Action<string, uint> Saved;

        public static object Main(string method, object[] args)
        {
            if (Runtime.Trigger == TriggerType.Application)
            {
                return method switch
                {
                    "put" => Put((string)args[0]),
                    "get" => Get((string)args[0]),
                    "exists" => Exists((string)args[0]),
                    _ => true
                };
            }
            return true;
        }

        [DisplayName("put")]
        public static bool Put(string message)
        {
            if (Exists(message)) return false;
            var blockChainHeight = Blockchain.GetHeight();
            Storage.Put(message, blockChainHeight);
            Saved(message, blockChainHeight);

            return true;
        }

        [DisplayName("get")]
        public static int Get(string message)
        {
            if (!Exists(message)) return -1;
            return (int)Storage.Get(message).TryToBigInteger();
        }

        [DisplayName("exists")]
        public static bool Exists(string message)
        {
            return Storage.Get(message) != null;
        }

    }

    public static class Helper
    {
        public static BigInteger TryToBigInteger(this byte[] value)
        {
            return value?.ToBigInteger() ?? 0;
        }
    }
}
