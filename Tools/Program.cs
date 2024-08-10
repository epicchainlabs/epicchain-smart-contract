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
            var sh = "XrTATFdLEVbbcAwfiQZF7qqLHMfWPa3XxA".ToScriptHash();
            var be = sh.ToString();
            var le = be.Remove(0, 2).HexToBytes().Reverse().ToArray().ToHexString();

            Console.WriteLine(be);  //0xe4b0b6fa65a399d7233827502b178ece1912cdd4
            Console.WriteLine(le);  //d4cd1219ce8e172b50273823d799a365fab6b0e4

            var sc = "0x230cf5ef1e1bd411c7733fa92bb6f9c39714f8f9".Remove(0, 2).HexToBytes().Reverse().ToArray().ToHexString();
            Console.WriteLine(sc);
            Console.ReadLine();
        }
    }
}
