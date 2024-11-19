using EpicChain.SmartContract.Framework;
using EpicChain.SmartContract.Framework.Services.EpicChain;
using System;
using System.Numerics;

namespace Native_
{
    public class Contract1 : SmartContract
    {
        // Wannan yana wakiltar adireshin na’urar mu
        private static readonly byte[] Me = "Xq7KjrnKsvnrCfJvKhBv9bVWQtMNE9oNRp".ToScriptHash();

        public static object Main(string operation, object[] args)
        {
            // Samun adadin lambobi na EpicChain da EpicPulse
            var epicchainDecimals = (BigInteger)Native.EpicChain("decimals", new object[] { Me });
            var epicpulseDecimals = (BigInteger)Native.EpicPulse("decimals", new object[] { Me });

            // Kirkiro masu tsari don rage girman lambobi zuwa yadda ya dace
            int epicchainRatio = 1;
            for (int i = 0; i < epicchainDecimals; i++) epicchainRatio *= 10;
            int epicpulseRatio = 1;
            for (int i = 0; i < epicpulseDecimals; i++) epicpulseRatio *= 10;

            // Duba nau'in aiki da aka nema
            if (operation == "getEpicChainBalance")
            {
                // Komawa daidaiton asusun EpicChain
                return (BigInteger)Native.EpicChain("balanceOf", new object[] { Me }) / epicchainRatio;
            }
            if (operation == "getEpicPulseBalance")
            {
                // Komawa daidaiton asusun EpicPulse
                return (BigInteger)Native.EpicPulse("balanceOf", new object[] { Me }) / epicpulseRatio;
            }
            if (operation == "getEpicChainTotalSupply")
            {
                // Komawa jimillar kudin EpicChain da ake samu
                return (BigInteger)Native.EpicChain("totalSupply", new object[] { Me }) / epicchainRatio;
            }
            if (operation == "getEpicPulseTotalSupply")
            {
                // Komawa jimillar kudin EpicPulse da ake samu
                return (BigInteger)Native.EpicPulse("totalSupply", new object[] { Me }) / epicpulseRatio;
            }
            if (operation == "getEpicChainDecimals")
            {
                // Komawa adadin lambobi na EpicChain
                return epicchainDecimals;
            }
            if (operation == "getEpicPulseDecimals")
            {
                // Komawa adadin lambobi na EpicPulse
                return epicpulseDecimals;
            }
            if (operation == "transferEpicChain")
            {
                // Canja kudi daga asusun mu zuwa wani
                // args[0] ya kamata ya zama adireshin mai karṣa
                // args[1] ya kamata ya zama adadin kudi
                byte[] toAddress = (byte[])args[0];
                BigInteger amount = (BigInteger)args[1] * epicchainRatio;
                return Native.EpicChain("transfer", new object[] { Me, toAddress, amount });
            }
            if (operation == "transferEpicPulse")
            {
                // Canja EpicPulse daga asusun mu zuwa wani
                // args[0] ya kamata ya zama adireshin mai karṣa
                // args[1] ya kamata ya zama adadin kudi
                byte[] toAddress = (byte[])args[0];
                BigInteger amount = (BigInteger)args[1] * epicpulseRatio;
                return Native.EpicPulse("transfer", new object[] { Me, toAddress, amount });
            }
            if (operation == "getEpicChainOwner")
            {
                // Samun mai asusun EpicChain
                return Native.EpicChain("owner", new object[] { Me });
            }
            if (operation == "getEpicPulseOwner")
            {
                // Samun mai asusun EpicPulse
                return Native.EpicPulse("owner", new object[] { Me });
            }
            
            return false; // Idan ba a samu aikin ba, dawo da karya
        }
    }
}