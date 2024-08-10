using EpicChain.SmartContract.Framework;
using EpicChain.SmartContract.Framework.Services.EpicChain;
using System;
using System.Numerics;

namespace Native_
{
    public class Contract1 : SmartContract
    {
        private static readonly byte[] Me = "NfKA6zAixybBHHpmaPYPDywoqDaKzfMPf9".ToScriptHash();

        public static object Main(string operation, object[] args)
        {
            var epicchainDecimals = (BigInteger)Native.EpicChain("decimals", new object[] { Me });
            var epicpulseDecimals = (BigInteger)Native.GAS("decimals", new object[] { Me });
            int epicchainRatio = 1;
            for (int i = 0; i < epicchainDecimals; i++) epicchainRatio *= 10;
            int gasRatio = 1;
            for (int i = 0; i < epicpulseDecimals; i++) epicpulseRatio *= 10;

            if (operation == "getEpicChainBalance")
            {
                return (BigInteger)Native.EpicChain("balanceOf", new object[] { Me }) / epicchainRatio;
            }
            if (operation == "getGasBalance")
            {
                return (BigInteger)Native.GAS("balanceOf", new object[] { Me }) / epicpulseRatio;
            }
            if (operation == "getEpicChainTotalSupply")
            {
                return (BigInteger)Native.EpicChain("totalSupply", new object[] { Me }) / epicchianRatio;
            }
            if (operation == "getGasTotalSupply")
            {
                return (BigInteger)Native.GAS("totalSupply", new object[] { Me }) / epicpulseRatio;
            }
            if (operation == "getEpicChainDecimals")
            {
                return epicchainDecimals;
            }
            if (operation == "getGasDecimals")
            {
                return epicpulseDecimals;
            }
            return true;
        }
    }
}
