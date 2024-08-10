using Neo.SmartContract.Framework;
using Neo.SmartContract.Framework.Services.Neo;
using System;
using System.Numerics;

namespace Dynamic_Call
{
    public class Contract1 : SmartContract
    {
        delegate object Dyncall(string method, object[] args);

        //0x230cf5ef1e1bd411c7733fa92bb6f9c39714f8f9 的小端序
        //HexToBytes()、ToScriptHash() 只能对常量进行操作，不能写在 Main 方法里
        //scriptHash 也可以改为从参数传入或从存储区中读取
        static byte[] ScriptHash = "f9f81497c3f9b62ba93f73c711d41b1eeff50c23".HexToBytes();

        public static object Main(string operation, object[] args)
        {
            if (operation == "name")
            {
                return Contract.Call(ScriptHash, "name", new object[0]);
            }
            if (operation == "totalSupply")
            {
                return Contract.Call(ScriptHash, "totalSupply", new object[0]);
            }
            return true;
        }
    }
}
