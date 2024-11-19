using EpicChain.SmartContract.Framework;
using EpicChain.SmartContract.Framework.Services.EpicChain;
using EpicChain.SmartContract.Framework.Services.System;
using System;
using System.ComponentModel;
using System.Numerics;

namespace XEP5
{
    // Wannan yana nuna cewa wannan SmartContract yana da adana bayanai (HasStorage)
    [Features(ContractFeatures.HasStorage)]
    public class XEP5 : SmartContract
    {
        // Wannan yana bayyana taron "transfer", wanda zai tashi idan an yi musayar kudi
        [DisplayName("transfer")]
        public static event Action<byte[], byte[], BigInteger> Transferred;

        // Jimlar adadin kudi (totalSupply) an saita shi zuwa wani darajar dindindin
        private static readonly BigInteger TotalSupplyValue = 10000000000000000;

        // Adireshin mai haƙƙin mallakar wannan contract, an saita shi zuwa 20-byte script hash
        private static readonly byte[] Owner = "XrTATFdLEVbbcAwfiQZF7qqLHMfWPa3XxA".ToScriptHash(); 

        // Wannan yana nuna ko an dakatar da aiki na contract ko a'a
        private static bool isPaused = false;

        // Wannan adireshin yana nuna wanda aka amince da shi don gudanar da musayar kudi
        private static readonly byte[] ApprovedAddress = "XrTATFdLEVbbcAwfiQZF7qqLHMfWPa3XxA".ToScriptHash(); 

        // Wannan shi ne babban aikin da zai gudanar da dukkan abubuwan da ake bukata
        public static object Main(string method, object[] args)
        {
            // Wannan yana tabbatar da cewa an kira aikin daidai daga wanda aka amince da shi
            if (Runtime.Trigger == TriggerType.Verification)
            {
                // Tabbatar da cewa mai kiran yana da izinin aiwatar da aikin
                return Runtime.CheckWitness(Owner);
            }
            else if (Runtime.Trigger == TriggerType.Application)
            {
                // Samun adireshin wanda ya kira wannan aikin
                var callscript = ExecutionEngine.CallingScriptHash;

                // Idan an kira aikin "deploy", yana nufin ana girka contract
                if (method == "deploy") return Deploy();

                // Idan an kira aikin "balanceOf", yana nufin ana neman bayanin adadin kudin wani asusu
                if (method == "balanceOf") return BalanceOf((byte[])args[0]);

                // Idan an kira aikin "decimals", yana dawo da yawan digits na kudi
                if (method == "decimals") return Decimals();

                // Idan an kira aikin "name", yana dawo da sunan kudi
                if (method == "name") return Name();

                // Idan an kira aikin "symbol", yana dawo da alamar kudi
                if (method == "symbol") return Symbol();

                // Idan an kira aikin "supportedStandards", yana dawo da wasu tsarin da wannan contract ke goyon baya
                if (method == "supportedStandards") return SupportedStandards();

                // Idan an kira aikin "totalSupply", yana dawo da jimlar kudi na wannan token
                if (method == "totalSupply") return TotalSupply();

                // Idan an kira aikin "transfer", yana gudanar da musayar kudi tsakanin adiresoshi
                if (method == "transfer") return Transfer((byte[])args[0], (byte[])args[1], (BigInteger)args[2], callscript);

                // Idan an kira aikin "pause", yana dakatar da aikin contract
                if (method == "pause") return Pause();
            }
            return false;
        }

        // Wannan yana girka contract, yana saita jimlar kudi da adiresoshin mai mallakar contract
        [DisplayName("deploy")]
        public static bool Deploy()
        {
            // Idan har an riga an girka contract, ba za a sake girka ba
            if (TotalSupply() != 0) return false;

            // Wannan yana adana bayanan "totalSupply" cikin wani StorageMap
            StorageMap contract = Storage.CurrentContext.CreateMap(nameof(contract));
            contract.Put("totalSupply", TotalSupplyValue);

            // Wannan yana adana kudaden mai mallakar contract a cikin StorageMap
            StorageMap asset = Storage.CurrentContext.CreateMap(nameof(asset));
            asset.Put(Owner, TotalSupplyValue);

            // Ana tura taron "Transferred" don sanar da kowane musayar kudi
            Transferred(null, Owner, TotalSupplyValue);
            return true;
        }

        // Wannan yana dawowa da adadin kudin da aka adana a asusun mai kiran
        [DisplayName("balanceOf")]
        public static BigInteger BalanceOf(byte[] account)
        {
            // Tabbatar da cewa adireshin asusun yana da tsawon 20 bytes
            if (account.Length != 20)
                throw new InvalidOperationException("The parameter account SHOULD be 20-byte addresses.");

            // Samun bayanan adadin kudi daga StorageMap
            StorageMap asset = Storage.CurrentContext.CreateMap(nameof(asset));
            return asset.Get(account).TryToBigInteger();
        }

        // Wannan yana dawowa da yawan digits na kudi (decimal places)
        [DisplayName("decimals")]
        public static byte Decimals() => 8;

        // Wannan yana duba ko wani adireshin yana da izinin karɓar kudi (payable)
        private static bool IsPayable(byte[] to)
        {
            // Duba idan an sami contract a adireshin "to" ko kuma yana da izinin karɓa
            var c = Blockchain.GetContract(to);
            return c == null || c.IsPayable;
        }

        // Wannan yana dawowa da sunan wannan token
        [DisplayName("name")]
        public static string Name() => "MyToken"; // sunan token

        // Wannan yana dawowa da alamar wannan token
        [DisplayName("symbol")]
        public static string Symbol() => "MYT"; // alamar token

        // Wannan yana dawowa da jerin tsarin da wannan contract ke goyon baya
        [DisplayName("supportedStandards")]
        public static string[] SupportedStandards() => new string[] { "XEP-5", "XEP-7", "XEP-10" };

        // Wannan yana dawowa da jimlar kudi (total supply) na token
        [DisplayName("totalSupply")]
        public static BigInteger TotalSupply()
        {
            StorageMap contract = Storage.CurrentContext.CreateMap(nameof(contract));
            return contract.Get("totalSupply").TryToBigInteger();
        }

        #if DEBUG
        // Wannan yana aiki ne kawai don ƙirƙirar ABI (Application Binary Interface)
        [DisplayName("transfer")] 
        public static bool Transfer(byte[] from, byte[] to, BigInteger amount) => true;
        #endif

        // Wannan yana aiwatar da musayar kudi daga mai aikawa zuwa mai karɓa
        private static bool Transfer(byte[] from, byte[] to, BigInteger amount, byte[] callscript)
        {
            // Tabbatar da cewa adireshin daga da zuwa sunada tsawon 20 bytes
            if (from.Length != 20 || to.Length != 20)
                throw new InvalidOperationException("The parameters from and to SHOULD be 20-byte addresses.");
            
            // Tabbatar da cewa adadin kudin da ake son turawa yana daidai
            if (amount <= 0)
                throw new InvalidOperationException("The parameter amount MUST be greater than 0.");
            
            // Tabbatar da cewa adireshin mai karɓa yana da izinin karɓar kudi
            if (!IsPayable(to))
                return false;

            // Tabbatar da cewa wanda ya tura yana da izinin yin hakan
            if (!Runtime.CheckWitness(from) && from.TryToBigInteger() != callscript.TryToBigInteger())
                return false;

            // Samun adadin kudin daga asusun mai aikawa
            StorageMap asset = Storage.CurrentContext.CreateMap(nameof(asset));
            var fromAmount = asset.Get(from).TryToBigInteger();

            // Tabbatar da cewa mai aikawa yana da isasshen kudi
            if (fromAmount < amount)
                return false;

            // Idan adireshin mai aikawa da mai karɓa suna daidai, babu buƙatar musayar
            if (from == to)
                return true;

            // Rage adadin kudi daga asusun mai aikawa
            if (fromAmount == amount)
                asset.Delete(from);
            else
                asset.Put(from, fromAmount - amount);

            // Ƙara adadin kudi zuwa asusun mai karɓa
            var toAmount = asset.Get(to).TryToBigInteger();
            asset.Put(to, toAmount + amount);

            // Tura taron "Transferred" don sanar da kowane musayar
            Transferred(from, to, amount);
            return true;
        }

        // Ƙarin aikin don juyawa daga byte[] zuwa BigInteger
        public static class Helper
        {
            public static BigInteger TryToBigInteger(this byte[] value)
            {
                return value?.ToBigInteger() ?? 0;
            }
        }
    }
}
