using System.Runtime.InteropServices;

namespace Crotchet.InputSimulator {
    // Token: 0x02000003 RID: 3
    [StructLayout(LayoutKind.Explicit)]
    internal struct MOUSEKEYBDHARDWAREINPUT {
        // Token: 0x04000004 RID: 4
        [FieldOffset(0)] public MOUSEINPUT Mouse;

        // Token: 0x04000005 RID: 5
        [FieldOffset(0)] public KEYBDINPUT Keyboard;

        // Token: 0x04000006 RID: 6
        [FieldOffset(0)] public HARDWAREINPUT Hardware;
    }
}