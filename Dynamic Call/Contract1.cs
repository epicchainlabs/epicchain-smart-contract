using EpicChain.SmartContract.Framework;
using EpicChain.SmartContract.Framework.Services.EpicChain;
using System;
using System.Numerics;

namespace Dynamic_Call
{
    public class Contract1 : SmartContract
    {
        // Delegate for dynamic calls
        delegate object Dyncall(string method, object[] args);

        // 小端序 scriptHash 的定义 (Hausa: Ma'anar ƙaramin tsari scriptHash)
        // 0x230cf5ef1e1bd411c7733fa92bb6f9c39714f8f9 in little-endian format
        static byte[] ScriptHash = "f9f81497c3f9b62ba93f73c711d41b1eeff50c23".HexToBytes();

        public static object Main(string operation, object[] args)
        {
            try
            {
                // Logging the operation for transparency (Hausa: Lissafin aikin don nuna gaskiya)
                Runtime.Log("Operation: " + operation);

                if (operation == "name")
                {
                    return GetName();
                }
                if (operation == "totalSupply")
                {
                    return GetTotalSupply();
                }
                if (operation == "balanceOf")
                {
                    if (args.Length < 1 || !(args[0] is byte[]))
                    {
                        Runtime.Log("Invalid arguments for balanceOf");
                        return false;
                    }
                    return GetBalanceOf((byte[])args[0]);
                }
                if (operation == "transfer")
                {
                    if (args.Length < 3 || !(args[0] is byte[]) || !(args[1] is byte[]) || !(args[2] is BigInteger))
                    {
                        Runtime.Log("Invalid arguments for transfer");
                        return false;
                    }
                    return Transfer((byte[])args[0], (byte[])args[1], (BigInteger)args[2]);
                }

                Runtime.Log("Operation not supported");
                return false;
            }
            catch (Exception ex)
            {
                // Logging any errors encountered (Hausa: Rubuta kowanne kuskure da aka samu)
                Runtime.Log("Error: " + ex.Message);
                return false;
            }
        }

        private static object GetName()
        {
            Runtime.Log("Fetching name of the token"); // Hausa: Samun sunan alamar
            return Contract.Call(ScriptHash, "name", new object[0]);
        }

        private static object GetTotalSupply()
        {
            Runtime.Log("Fetching total supply of the token"); // Hausa: Samun adadin gaba ɗaya na alamar
            return Contract.Call(ScriptHash, "totalSupply", new object[0]);
        }

        private static object GetBalanceOf(byte[] address)
        {
            Runtime.Log("Fetching balance for address: " + address.ToHexString()); // Hausa: Samun ma'auni ga adireshi
            return Contract.Call(ScriptHash, "balanceOf", new object[] { address });
        }

        private static object Transfer(byte[] from, byte[] to, BigInteger amount)
        {
            Runtime.Log("Initiating transfer"); // Hausa: Fara tura kudi
            return Contract.Call(ScriptHash, "transfer", new object[] { from, to, amount });
        }

        // Helper method for better readability (Hausa: Hanyar taimako don ingantacciyar fahimta)
        private static void ValidateAddress(byte[] address)
        {
            if (address.Length != 20)
            {
                throw new Exception("Invalid address length"); // Hausa: Tsawon adireshin bai dace ba
            }
        }

        // Logging utility for development (Hausa: Amfani da rubutun don ci gaba da aiki)
        private static void Log(string message)
        {
            Runtime.Notify(message); // Hausa: Nuna saƙo
        }
    }
}