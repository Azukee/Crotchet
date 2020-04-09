using System;
using HoLLy.Memory.CrossPlatform;

namespace Crotchet.Quaver.Memory.Objects
{
    public class GameplayAudioTimingObject
    {
        private Process process;

        private UIntPtr Addr { get; }
        
        public GameplayAudioTimingObject(Process proc, UIntPtr addr)
        {
            process = proc;
            Addr = addr;
        }

        public double Time => process.ReadF64(Addr + 0x10);
    }
}