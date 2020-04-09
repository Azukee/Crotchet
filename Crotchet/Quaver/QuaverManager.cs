using System;
using System.Diagnostics;
using System.Linq;
using Crotchet.Quaver.Exceptions;
using Crotchet.Quaver.Memory;
using Crotchet.Quaver.Memory.Objects;
using HoLLy.Memory.Scanning;
using HoLLy.Memory.Windows;

namespace Crotchet.Quaver
{
    public class QuaverManager
    {
        private Process dummyProcess;
        private HoLLy.Memory.CrossPlatform.Process quaverProcess;
        private QuaverGameObject quaverGame;
        private UIntPtr quaverGameBP;

        /// <summary>
        /// Creates a new QuaverManager instance, everything is handled from here.
        /// </summary>
        public QuaverManager()
        {
            dummyProcess = Process.GetProcessesByName("Quaver").FirstOrDefault();
            quaverProcess = HoLLy.Memory.CrossPlatform.Process.Open((uint) dummyProcess.Id);
            if (quaverProcess == null)
                throw new GameNotFoundException();
            
            ScanSignatures();
        }

        private void ScanSignatures()
        {
            Console.WriteLine($"    [X] 0x{dummyProcess.MainModule.BaseAddress.ToInt64():X8}");
            
            if (!quaverProcess.Scan(PatternByte.Parse(Signatures.QuaverGame.AoB), false, dummyProcess.MainModule.BaseAddress, out UIntPtr PlayerObjectResult))
                throw new SignatureScanReturnedNullException();
            
            var r1 = quaverProcess.ReadS64(PlayerObjectResult + Signatures.QuaverGame.Offset);
            var r2 = quaverProcess.ReadS64((UIntPtr) r1);
            quaverGameBP = (UIntPtr) r1;
            
            Console.WriteLine($"    [X] 0x{PlayerObjectResult:X8} | 0x{r1:X8} | 0x{r2:X8}");

            UpdatePlayerObject();
            
            UIntPtr discordRPCBaseAddress = UIntPtr.Zero;
            // Get Discord RPC
            foreach(ProcessModule module in dummyProcess.Modules)
                if (module.ModuleName == "discord-rpc.DLL")
                    discordRPCBaseAddress = new UIntPtr((ulong) module.BaseAddress);
                    
            Console.WriteLine($"    [X] 0x{discordRPCBaseAddress:X8}"); 
        }

        public void UpdatePlayerObject() =>
            quaverGame = new QuaverGameObject(quaverProcess, (UIntPtr) quaverProcess.ReadS64(quaverGameBP));

        public QuaverGameObject QuaverGame => quaverGame;
        public HoLLy.Memory.CrossPlatform.Process QuaverProcess => quaverProcess;
    }
}