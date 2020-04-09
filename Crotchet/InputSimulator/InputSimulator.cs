using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace Crotchet.InputSimulator {
    // Token: 0x02000009 RID: 9
    public static class InputSimulator {
        // Token: 0x06000002 RID: 2
        [DllImport("user32.dll", SetLastError = true)]
        private static extern short GetAsyncKeyState(ushort virtualKeyCode);

        // Token: 0x06000003 RID: 3
        [DllImport("user32.dll", SetLastError = true)]
        private static extern short GetKeyState(ushort virtualKeyCode);

        // Token: 0x06000004 RID: 4
        [DllImport("user32.dll")]
        private static extern IntPtr GetMessageExtraInfo();

        // Token: 0x06000006 RID: 6 RVA: 0x00002068 File Offset: 0x00000268
        public static bool IsKeyDown(VirtualKeyCode keyCode) {
            short keyState = GetKeyState((ushort) keyCode);
            return keyState < 0;
        }

        // Token: 0x06000005 RID: 5 RVA: 0x00002050 File Offset: 0x00000250
        public static bool IsKeyDownAsync(VirtualKeyCode keyCode) {
            short asyncKeyState = GetAsyncKeyState((ushort) keyCode);
            return asyncKeyState < 0;
        }

        // Token: 0x06000007 RID: 7 RVA: 0x00002080 File Offset: 0x00000280
        public static bool IsTogglingKeyInEffect(VirtualKeyCode keyCode) {
            short keyState = GetKeyState((ushort) keyCode);
            return (keyState & 1) == 1;
        }

        // Token: 0x06000001 RID: 1
        [DllImport("user32.dll", SetLastError = true)]
        private static extern uint SendInput(uint numberOfInputs, INPUT[] inputs, int sizeOfInputStructure);

        // Token: 0x06000008 RID: 8 RVA: 0x0000209C File Offset: 0x0000029C
        public static void SimulateKeyDown(VirtualKeyCode keyCode) {
            INPUT iNPUT = default(INPUT);
            iNPUT.Type = 1u;
            iNPUT.Data.Keyboard = default(KEYBDINPUT);
            iNPUT.Data.Keyboard.Vk = 0;
            iNPUT.Data.Keyboard.Scan = (ushort) MapVirtualKey((ushort)keyCode, 0x00);
            iNPUT.Data.Keyboard.Flags = (uint) (KeyEventF.KeyDown | KeyEventF.Scancode);
            iNPUT.Data.Keyboard.Time = 0u;
            iNPUT.Data.Keyboard.ExtraInfo = IntPtr.Zero;
            if (SendInput(1u, new[] {iNPUT}, Marshal.SizeOf(typeof (INPUT))) == 0u)
                throw new Exception(string.Format("The key down simulation for {0} was not successful.", keyCode));
        }

        // Token: 0x0600000A RID: 10 RVA: 0x00002244 File Offset: 0x00000444
        public static void SimulateKeyPress(VirtualKeyCode keyCode) {
            INPUT iNPUT = default(INPUT);
            iNPUT.Type = 1u;
            iNPUT.Data.Keyboard = default(KEYBDINPUT);
            iNPUT.Data.Keyboard.Vk = (ushort) keyCode;
            iNPUT.Data.Keyboard.Scan = 0;
            iNPUT.Data.Keyboard.Flags = 0u;
            iNPUT.Data.Keyboard.Time = 0u;
            iNPUT.Data.Keyboard.ExtraInfo = IntPtr.Zero;
            INPUT iNPUT2 = default(INPUT);
            iNPUT2.Type = 1u;
            iNPUT2.Data.Keyboard = default(KEYBDINPUT);
            iNPUT2.Data.Keyboard.Vk = (ushort) keyCode;
            iNPUT2.Data.Keyboard.Scan = 0;
            iNPUT2.Data.Keyboard.Flags = 2u;
            iNPUT2.Data.Keyboard.Time = 0u;
            iNPUT2.Data.Keyboard.ExtraInfo = IntPtr.Zero;
            if (SendInput(2u, new[] {iNPUT, iNPUT2}, Marshal.SizeOf(typeof (INPUT))) == 0u)
                throw new Exception(string.Format("The key press simulation for {0} was not successful.", keyCode));
        }

        // Token: 0x06000009 RID: 9 RVA: 0x00002170 File Offset: 0x00000370
        public static void SimulateKeyUp(VirtualKeyCode keyCode) {
            INPUT iNPUT = default(INPUT);
            iNPUT.Type = 1u;
            iNPUT.Data.Keyboard = default(KEYBDINPUT);
            iNPUT.Data.Keyboard.Vk = 0;
            iNPUT.Data.Keyboard.Scan =  (ushort) MapVirtualKey((ushort)keyCode, 0x00);;
            iNPUT.Data.Keyboard.Flags = (uint) (KeyEventF.KeyUp | KeyEventF.Scancode);
            iNPUT.Data.Keyboard.Time = 0u;
            iNPUT.Data.Keyboard.ExtraInfo = IntPtr.Zero;
            if (SendInput(1u, new[] {iNPUT}, Marshal.SizeOf(typeof (INPUT))) == 0u)
                throw new Exception(string.Format("The key up simulation for {0} was not successful.", keyCode));
        }

        // Token: 0x0600000C RID: 12 RVA: 0x0000258D File Offset: 0x0000078D
        public static void SimulateModifiedKeyStroke(VirtualKeyCode modifierKeyCode, VirtualKeyCode keyCode) {
            SimulateKeyDown(modifierKeyCode);
            SimulateKeyPress(keyCode);
            SimulateKeyUp(modifierKeyCode);
        }

        // Token: 0x0600000D RID: 13 RVA: 0x000025B4 File Offset: 0x000007B4
        public static void SimulateModifiedKeyStroke(IEnumerable<VirtualKeyCode> modifierKeyCodes, VirtualKeyCode keyCode) {
            if (modifierKeyCodes != null)
                modifierKeyCodes.ToList().ForEach(delegate(VirtualKeyCode x) { SimulateKeyDown(x); });
            SimulateKeyPress(keyCode);
            if (modifierKeyCodes != null)
                modifierKeyCodes.Reverse().ToList().ForEach(delegate(VirtualKeyCode x) { SimulateKeyUp(x); });
        }

        // Token: 0x0600000E RID: 14 RVA: 0x0000262A File Offset: 0x0000082A
        public static void SimulateModifiedKeyStroke(VirtualKeyCode modifierKey, IEnumerable<VirtualKeyCode> keyCodes) {
            SimulateKeyDown(modifierKey);
            if (keyCodes != null)
                keyCodes.ToList().ForEach(delegate(VirtualKeyCode x) { SimulateKeyPress(x); });
            SimulateKeyUp(modifierKey);
        }

        // Token: 0x0600000F RID: 15 RVA: 0x0000267C File Offset: 0x0000087C
        public static void SimulateModifiedKeyStroke(IEnumerable<VirtualKeyCode> modifierKeyCodes, IEnumerable<VirtualKeyCode> keyCodes) {
            if (modifierKeyCodes != null)
                modifierKeyCodes.ToList().ForEach(delegate(VirtualKeyCode x) { SimulateKeyDown(x); });
            if (keyCodes != null)
                keyCodes.ToList().ForEach(delegate(VirtualKeyCode x) { SimulateKeyPress(x); });
            if (modifierKeyCodes != null)
                modifierKeyCodes.Reverse().ToList().ForEach(delegate(VirtualKeyCode x) { SimulateKeyUp(x); });
        }

        // Token: 0x0600000B RID: 11 RVA: 0x000023A4 File Offset: 0x000005A4
        public static void SimulateTextEntry(string text) {
            if (text.Length > 2147483647L)
                throw new ArgumentException(string.Format("The text parameter is too long. It must be less than {0} characters.", 2147483647u), "text");
            byte[] bytes = Encoding.ASCII.GetBytes(text);
            int num = bytes.Length;
            INPUT[] array = new INPUT[num*2];
            for (int i = 0; i < num; i++) {
                ushort num2 = bytes[i];
                INPUT iNPUT = default(INPUT);
                iNPUT.Type = 1u;
                iNPUT.Data.Keyboard = default(KEYBDINPUT);
                iNPUT.Data.Keyboard.Vk = 0;
                iNPUT.Data.Keyboard.Scan = num2;
                iNPUT.Data.Keyboard.Flags = 4u;
                iNPUT.Data.Keyboard.Time = 0u;
                iNPUT.Data.Keyboard.ExtraInfo = IntPtr.Zero;
                INPUT iNPUT2 = default(INPUT);
                iNPUT2.Type = 1u;
                iNPUT2.Data.Keyboard = default(KEYBDINPUT);
                iNPUT2.Data.Keyboard.Vk = 0;
                iNPUT2.Data.Keyboard.Scan = num2;
                iNPUT2.Data.Keyboard.Flags = 6u;
                iNPUT2.Data.Keyboard.Time = 0u;
                iNPUT2.Data.Keyboard.ExtraInfo = IntPtr.Zero;
                if ((num2 & 65280) == 57344) {
                    iNPUT.Data.Keyboard.Flags = iNPUT.Data.Keyboard.Flags | 1u;
                    iNPUT2.Data.Keyboard.Flags = iNPUT2.Data.Keyboard.Flags | 1u;
                }
                array[2*i] = iNPUT;
                array[2*i + 1] = iNPUT2;
            }
            SendInput((uint) (num*2), array, Marshal.SizeOf(typeof (INPUT)));
        }
        [DllImport("user32.dll")]
        static extern uint MapVirtualKey(uint uCode, uint uMapType);
        
        [Flags]
        private enum KeyEventF
        {
            KeyDown = 0x0000,
            ExtendedKey = 0x0001,
            KeyUp = 0x0002,
            Unicode = 0x0004,
            Scancode = 0x0008,
        }
    }
}