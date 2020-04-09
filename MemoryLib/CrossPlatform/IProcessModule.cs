using System.IO;

namespace HoLLy.Memory.CrossPlatform
{
    public class IProcessModule
    {
        string FullPath { get; }
        ulong BaseAddress { get; }

        public string FileName => Path.GetFileName(FullPath);
    }
}