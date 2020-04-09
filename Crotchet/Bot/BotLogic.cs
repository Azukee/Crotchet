using System;
using System.Threading;
using Crotchet.Quaver.Chart;
using Crotchet.Quaver.Chart.Models;
using Crotchet.Quaver.Chart.Objects;

namespace Crotchet.Bot
{
    public class BotLogic
    {
        public static bool NewPlayerObjectParsed = false;
        public static bool MapParsed = false;
        public static QuaverChart CurrentChart = null;

        public static int NoteIndex = 0;
        
        public static void DoPlay()
        {
            while (true) {
                if (Program.qm.QuaverGame.CurrentScreen().IsPlaying) {
                    if (NewPlayerObjectParsed == false) {
                        // wait for GC to clear up
                        Thread.Sleep(1750);
                        //Program.qm.UpdatePlayerObject();
                        NewPlayerObjectParsed = true;
                    } else if (MapParsed == false) {
                        CurrentChart = new QuaverChart();
                        CurrentChart.Parse(Program.qm.QuaverGame.CurrentScreen().CurrentMap());
                        MapParsed = true;
                    } else { // everything done, let's start playing uwu
                        double ct = Program.qm.QuaverGame.CurrentScreen().GameplayAudioTiming().Time;
                        if (ct >= CurrentChart.HitObjects[NoteIndex].StartTime) {
                            // todo: IsBackground = true takes away A LOT OF cpu cycles, try to see if I can make something better
                            var vk = CurrentChart.HitObjects[NoteIndex].Lane.ToVirtualKeycode();
                            var dr = CurrentChart.HitObjects[NoteIndex].EndTime;
                            new Thread(() => {
                                InputSimulator.InputSimulator.SimulateKeyDown(vk);
                                Thread.Sleep(dr);
                                InputSimulator.InputSimulator.SimulateKeyUp(vk);
                            }){IsBackground = true}.Start();
                            NoteIndex++;
                        }
                    }
                    Thread.Sleep(1);
                } else { // we safe our poor cpu's of some cycles here 
                    NewPlayerObjectParsed = false;
                    MapParsed = false;
                    NoteIndex = 0;
                    Console.WriteLine("No map detected... Idling until a map appears in memory...");
                    Thread.Sleep(2000);
                }
            }
        }
    }
}