using System;
using System.Runtime.InteropServices;
using System.Threading;
using Crotchet.Bot;
using Crotchet.Quaver;
using Crotchet.Quaver.Chart;

namespace Crotchet
{
    internal class Program
    {
        public static QuaverManager qm;
        public static Thread bt;
        
        public static unsafe void Main(string[] args)
        {
            Console.Title = "こんいちは。";
            
            Console.WriteLine("Creating QuaverObject...");
            qm = new QuaverManager();
            TypedReference tr = __makeref(qm);
            IntPtr ptr = **(IntPtr**)(&tr);
            
            Console.WriteLine("  [x] QuaverManager qm - 0x" + $"{ptr:X8}");
            Console.WriteLine("  [x] TimerObject timerObject - 0x" + $"{qm.QuaverGame.Address.ToUInt64():X8}");
            
            Console.WriteLine("Starting Bot Thread...");
            bt = new Thread(BotLogic.DoPlay);
            bt.Start();

            Console.ReadKey();
        }
    }
}