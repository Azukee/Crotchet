namespace Crotchet.InputSimulator {
    // Token: 0x0200000B RID: 11
    public enum MouseFlag : uint {
        // Token: 0x040000D2 RID: 210
        MOVE = 1u,
        // Token: 0x040000D3 RID: 211
        LEFTDOWN,
        // Token: 0x040000D4 RID: 212
        LEFTUP = 4u,
        // Token: 0x040000D5 RID: 213
        RIGHTDOWN = 8u,
        // Token: 0x040000D6 RID: 214
        RIGHTUP = 16u,
        // Token: 0x040000D7 RID: 215
        MIDDLEDOWN = 32u,
        // Token: 0x040000D8 RID: 216
        MIDDLEUP = 64u,
        // Token: 0x040000D9 RID: 217
        XDOWN = 128u,
        // Token: 0x040000DA RID: 218
        XUP = 256u,
        // Token: 0x040000DB RID: 219
        WHEEL = 2048u,
        // Token: 0x040000DC RID: 220
        VIRTUALDESK = 16384u,
        // Token: 0x040000DD RID: 221
        ABSOLUTE = 32768u
    }
}