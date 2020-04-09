using System;

namespace Crotchet.InputSimulator {
    // Token: 0x0200000A RID: 10
    internal struct MOUSEINPUT {
        // Token: 0x040000CB RID: 203
        public int X;

        // Token: 0x040000CC RID: 204
        public int Y;

        // Token: 0x040000CD RID: 205
        public uint MouseData;

        // Token: 0x040000CE RID: 206
        public uint Flags;

        // Token: 0x040000CF RID: 207
        public uint Time;

        // Token: 0x040000D0 RID: 208
        public IntPtr ExtraInfo;
    }
}