namespace Crotchet.Quaver.Memory
{
    public class Signatures
    {
        public static Signature QuaverGame = new Signature {
            AoB = "44 8B 8D 74 FF FF FF E8",
            Mask = "xxxxxxxx",
            Offset = 0x15
        };
        
        public static Signature PlayerObject = new Signature {
            // 74 19 49 BB ?? ?? ?? ?? ?? ?? ?? ?? 48 B8 ?? ?? ?? ?? ?? ?? ?? ?? 39 09 FF 10 90 48 
            AoB = "39 09 FF 10 85 C0 74 31",
            Mask = "xxxxxxxx",
            Offset = 0xA
        };
    }

    public class Signature
    {
        public string AoB;
        public string Mask;
        public int Offset;
    }
}