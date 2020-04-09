using System;
using HoLLy.Memory.CrossPlatform;

namespace Crotchet.Quaver.Memory.Objects
{
    public class QuaverGameObject
    {
        private Process process;

        private UIntPtr Addr;
        
        public QuaverGameObject(Process proc, UIntPtr addr)
        {
            process = proc;
            Addr = addr;
        }
        
        public CurrentScreenObject CurrentScreen() => new CurrentScreenObject(process, (UIntPtr) process.ReadS64(Addr + 0x120));
        public UIntPtr Address => Addr;
    }
}