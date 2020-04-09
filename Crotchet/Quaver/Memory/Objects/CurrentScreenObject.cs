using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using HoLLy.Memory.CrossPlatform;

namespace Crotchet.Quaver.Memory.Objects
{
    public class CurrentScreenObject
    {
        private Process process;

        private UIntPtr Addr;
        
        public CurrentScreenObject(Process proc, UIntPtr addr)
        {
            process = proc;
            Addr = addr;
        }

        public string CurrentMap()
        {
            UIntPtr bmPtr = (UIntPtr) process.ReadS64(Addr + 0x1C0);
                
            int stringLength = process.ReadS32(bmPtr + 0x08) * 2;
            string bmFolder = process.ReadString(bmPtr + 0x0C, stringLength, Encoding.Unicode);
            bmFolder = bmFolder.Replace(bmFolder.Split('/')[^1], "");
                
            // get md5 hash of map from memory
            // you could theoretically hardcode this value, since md5 hashes are all the same length
            UIntPtr md5Ptr = (UIntPtr) process.ReadS64(Addr + 0x70);
                
            int md5Length = process.ReadS32(md5Ptr + 0x08) * 2;
            string md5Hash = process.ReadString(md5Ptr + 0x0C, md5Length, Encoding.Unicode);

            string quaFile = "";
                
            string[] filesInBeatmapFolder = Directory.GetFiles(bmFolder);
            foreach(string file in filesInBeatmapFolder) {
                using var md5 = MD5.Create();
                using var stream = File.OpenRead(file);
                if (BitConverter.ToString(md5.ComputeHash(stream)).Replace("-", "").ToLower() == md5Hash)
                    quaFile = file;
            }

            return quaFile;
        }
        
        public GameplayAudioTimingObject GameplayAudioTiming() => new GameplayAudioTimingObject(process, (UIntPtr) process.ReadS64(Addr + 0x48));
        public bool IsPlaying => process.ReadS32(Addr + 0xE8) == 2;
    }
}