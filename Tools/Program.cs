using EpicChain;
using EpicChain.Wallets;
using System;
using System.Linq;

namespace Tools
{
    class Program
    {
        static void Main(string[] args)
        {
            // Misalin amfani da adireshin wallet
            var sh = "XrTATFdLEVbbcAwfiQZF7qqLHMfWPa3XxA".ToScriptHash();
            var be = sh.ToString();
            var le = be.Remove(0, 2).HexToBytes().Reverse().ToArray().ToHexString();

            // Nuna ScriptHash da Hex da aka juyar
            Console.WriteLine(be);  // Misali: 0xe4b0b6fa65a399d7233827502b178ece1912cdd4
            Console.WriteLine(le);  // Misali: d4cd1219ce8e172b50273823d799a365fab6b0e4

            // Misali na wani smart contract
            var sc = "0x230cf5ef1e1bd411c7733fa92bb6f9c39714f8f9".Remove(0, 2).HexToBytes().Reverse().ToArray().ToHexString();
            Console.WriteLine(sc);

            // Zabi wani adireshin wallet da zai duba
            string addressToCheck = "XrTATFdLEVbbcAwfiQZF7qqLHMfWPa3XxA";
            bool isValid = IsValidAddress(addressToCheck);
            Console.WriteLine($"Adireshin {addressToCheck} yana daidai: {isValid}");

            // Samun ma'aunin wallet
            decimal balance = GetWalletBalance(addressToCheck);
            Console.WriteLine($"Ma'aunin wallet na {addressToCheck} shine: {balance} XPR");

            // Gwada aika kudi daga wallet guda daya zuwa wani
            string senderAddress = "XrTATFdLEVbbcAwfiQZF7qqLHMfWPa3XxA";
            string receiverAddress = "XrTATFdLEVbbcAwfiQZF7qqLHMfWPa3XxA";
            decimal amount = 10.5M;
            bool transferSuccess = SimulateTransfer(senderAddress, receiverAddress, amount);
            Console.WriteLine($"Aika kudi daga {senderAddress} zuwa {receiverAddress} tare da {amount} XPR: {transferSuccess}");

            // Nuna Hex encoding da decoding
            string text = "Hello EpicChain";
            string hexEncoded = StringToHex(text);
            Console.WriteLine($"Encoded Hex: {hexEncoded}");
            string decodedString = HexToString(hexEncoded);
            Console.WriteLine($"Decoded String: {decodedString}");

            Console.ReadLine();
        }

        // Aiki: Duba ko adireshin yana daidai (valid address)
        public static bool IsValidAddress(string address)
        {
            try
            {
                var scriptHash = address.ToScriptHash();
                // Idan scriptHash ba a fuskanci kowanne kuskure ba, za a ce yana daidai
                return scriptHash != null;
            }
            catch (Exception)
            {
                return false;  // Idan an samu kuskure, ba valid address ba ne
            }
        }

        // Aiki: Samun ma'aunin wallet
        public static decimal GetWalletBalance(string address)
        {
            // Za mu iya amfani da EpicChain blockchain API ko wani hanya don duba balance
            // A nan, kawai za mu dawo da misalin darajar balance
            return 100.0M; // Misali ne kawai
        }

        // Aiki: Gwada aika kudi daga wallet guda zuwa wani
        public static bool SimulateTransfer(string senderAddress, string receiverAddress, decimal amount)
        {
            // A nan, za mu iya yin gwajin canja wuri tare da duba adadin da aka aika
            // Kamar yadda aka saba, wannan na iya zama hanya ta test don karantarwa.
            Console.WriteLine($"Aika {amount} XPR daga {senderAddress} zuwa {receiverAddress}");
            return true;  // Aika kudi ya yi nasara
        }

        // Aiki: Hex encoding (String zuwa Hex)
        public static string StringToHex(string str)
        {
            return string.Join("", str.Select(c => ((int)c).ToString("x2")));
        }

        // Aiki: Hex decoding (Hex zuwa String)
        public static string HexToString(string hex)
        {
            var bytes = Enumerable.Range(0, hex.Length)
                                  .Where(x => x % 2 == 0)
                                  .Select(x => Convert.ToByte(hex.Substring(x, 2), 16))
                                  .ToArray();
            return System.Text.Encoding.UTF8.GetString(bytes);
        }
    }
}
