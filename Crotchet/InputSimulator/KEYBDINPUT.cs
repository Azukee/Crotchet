using System;

namespace Crotchet.InputSimulator {
    // Token: 0x02000007 RID: 7
    internal struct KEYBDINPUT {
        // Token: 0x040000BC RID: 188
        public ushort Vk;

        // Token: 0x040000BD RID: 189
        public ushort Scan;

        // Token: 0x040000BE RID: 190
        public uint Flags;

        // Token: 0x040000BF RID: 191
        public uint Time;

        // Token: 0x040000C0 RID: 192
        public IntPtr ExtraInfo;
    }
}