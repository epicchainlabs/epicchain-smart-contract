using EpicChain.SmartContract.Framework;
using EpicChain.SmartContract.Framework.Services.EpicChain;
using EpicChain.SmartContract.Framework.Services.System;
using System;
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
                    "update" => Update((string)args[0], (string)args[1]),
                    "delete" => Delete((string)args[0]),
                    "getAll" => GetAll(),
                    "putWithLimit" => PutWithLimit((string)args[0]),
                    _ => true
                };
            }
            return true;
        }

        // Ajiye saƙon idan ba a riga an ajiye shi ba
        [DisplayName("put")]
        public static bool Put(string message)
        {
            if (Exists(message)) return false;
            var blockChainHeight = Blockchain.GetHeight();
            Storage.Put(message, blockChainHeight);
            Saved(message, blockChainHeight);

            return true;
        }

        // Samu darajar da aka ajiye wanda aka danganta da saƙo
        [DisplayName("get")]
        public static int Get(string message)
        {
            if (!Exists(message)) return -1;
            return (int)Storage.Get(message).TryToBigInteger();
        }

        // Duba idan saƙo yana cikin ajiya
        [DisplayName("exists")]
        public static bool Exists(string message)
        {
            return Storage.Get(message) != null;
        }

        // Sabunta saƙo da sabon saƙo
        [DisplayName("update")]
        public static bool Update(string oldMessage, string newMessage)
        {
            if (!Exists(oldMessage)) return false;

            var blockChainHeight = Blockchain.GetHeight();
            Storage.Put(newMessage, blockChainHeight);
            Storage.Delete(oldMessage); // Cire tsohon saƙon
            Saved(newMessage, blockChainHeight);

            return true;
        }

        // Cire saƙo daga ajiya
        [DisplayName("delete")]
        public static bool Delete(string message)
        {
            if (!Exists(message)) return false;

            Storage.Delete(message);
            return true;
        }

        // Samu dukkan saƙonnin da aka ajiye (Wannan hanya yana da tushe don aiwatar da shi)
        [DisplayName("getAll")]
        public static string[] GetAll()
        {
            // Anan za ku aiwatar da lissafin dukkan saƙonnin da aka ajiye
            // Ana iya yin wannan ta hanyar ajiye maɓallan a cikin tsari na musamman ko kulawa da jeri
            return new string[] { };
        }

        // Ajiye saƙo tare da iyaka kan yawan saƙonnin da za a ajiye
        private const int MaxStoredMessages = 100;

        [DisplayName("putWithLimit")]
        public static bool PutWithLimit(string message)
        {
            if (Exists(message)) return false;

            // Duba yawan saƙonnin da aka ajiye (Wannan na iya zama mafi inganci bisa tsarin ajiya)
            if (GetStoredMessagesCount() >= MaxStoredMessages)
            {
                // Zaka iya zaɓar cire tsohon saƙo ko ƙin karɓar sabon
                DeleteOldestMessage();
            }

            var blockChainHeight = Blockchain.GetHeight();
            Storage.Put(message, blockChainHeight);
            Saved(message, blockChainHeight);

            return true;
        }

        // Samu yawan saƙonnin da aka ajiye
        public static int GetStoredMessagesCount()
        {
            // Tushe don lissafin yawan saƙonnin da aka ajiye
            return 0;
        }

        // Cire tsohon saƙo (Wannan yana da tushe don aiwatar da shi)
        public static void DeleteOldestMessage()
        {
            // Tsarin cire tsohon saƙo daga ajiya
        }
    }

    // Ajiye kayan aiki don canza zuwa BigInteger
    public static class Helper
    {
        public static BigInteger TryToBigInteger(this byte[] value)
        {
            return value?.ToBigInteger() ?? 0;
        }
    }
}
