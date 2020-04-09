using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;

namespace Crotchet.Quaver.Memory
{
    public class Memory
    {
        /// <summary>
        ///     The process that is being used.
        /// </summary>
        internal Process Process;

        /// <summary>
        ///     Returns a bool that states if the class was implemented successfuly.
        /// </summary>
        internal bool Successful;

        /// <summary>
        ///     Put in the process name as in the one that displays in task manager.
        /// </summary>
        internal Memory(string processName)
        {
            var pName = processName;
            if (processName.Substring(processName.Length - 4) == ".exe")
                pName = processName.Remove(processName.Length - 4);

            var p = Process.GetProcessesByName(pName);
            if (p.Length > 0) {
                Process = p[0];
                Successful = true;
            }
            else {
                Successful = false;
            }
        }

        internal Memory(Process process)
        {
            Process = process;
            Successful = true;
        }

        [DllImport("kernel32.dll")]
        private static extern bool ReadProcessMemory(
            int hProcess, int lpBaseAddress, byte[] lpBuffer, int dwSize, ref int lpNumberOfBytesRead);

        [DllImport("kernel32.dll")]
        private static extern bool ReadProcessMemory(
            int hProcess, long lpBaseAddress, byte[] lpBuffer, int dwSize, ref int lpNumberOfBytesRead);

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern bool WriteProcessMemory(
            int hProcess, int lpBaseAddress, byte[] lpBuffer, int dwSize, ref int lpNumberOfBytesWritten);

        /// <summary>
        ///     Returns the address of an array of bytes.
        ///     If didn't find or there was an error it returns -1.
        ///     * Use a space as a separator, or don't use a separator at all.
        ///     * Accepts only 2 character hex bytes, for example "FF 01 00 A1 10".
        /// </summary>
        internal int SignatureScan(
            string pattern, string mask, int offset, int startAddress, int endAddress, int bufferSize = 32768)
        {
            // Converting the string version of pattern to a byte array.
            bool spaceDelimited;
            if (pattern.Length == 0)
                return -1;
            if (pattern.Length > 2)
                spaceDelimited = pattern[2] == 0x20;
            else
                spaceDelimited = false;
            if (spaceDelimited) {
                if ((pattern.Length + 1) % 3 != 0)
                    return -1;
            }
            else if (pattern.Length % 2 != 0) {
                return -1;
            }

            var len = spaceDelimited ? (pattern.Length + 1) / 3 : pattern.Length / 2;
            var result = new byte[len];
            foreach (var c in pattern)
                if (!(c >= 48 && c <= 57 || c >= 65 && c <= 70 || c >= 97 && c <= 102) && c != 0x20)
                    return -1;
            for (var i = 0; i < len; i++)
                if (spaceDelimited)
                    result[i] = Convert.ToByte("" + pattern[i * 3] + pattern[i * 3 + 1], 16);
                else
                    result[i] = Convert.ToByte("" + pattern[i * 2] + pattern[i * 2 + 1], 16);

            // Scanning the process looking for the signature.
            return SignatureScan(result, mask, offset, startAddress, endAddress, bufferSize);
        }

        /// <summary>
        ///     Returns the address of an array of bytes.
        ///     If didn't find or there was an error it returns -1.
        /// </summary>
        internal int SignatureScan(
            byte[] pattern, string mask, int offset, int startAddress, int endAddress, int bufferSize = 32768)
        {
            if (pattern.Length != mask.Length)
                return -1;
            foreach (var c in mask)
                if (c != 'x' && c != '?')
                    return -1;

            var currentAddress = startAddress;
            byte[] readBytes;
            while (currentAddress < endAddress) {
                readBytes = ReadBytes(currentAddress, bufferSize);
                for (var i = 0; i < readBytes.Length - pattern.Length; i++)
                for (var x = 0; x < pattern.Length; x++)
                    if (mask[x] == 'x') {
                        if (readBytes[i + x] != pattern[x])
                            break;
                        if (x == pattern.Length - 1)
                            return currentAddress + i + offset;
                    }
                    else if (mask[x] == '?' && x == pattern.Length - 1) {
                        return currentAddress + i + offset;
                    }

                currentAddress += bufferSize;
            }

            return -1;
        }

        /// <summary>
        ///     Returns an array of bytes taken from the address given, with a specified length.
        /// </summary>
        internal byte[] ReadBytes(int address, int count)
        {
            var bytesRead = 0;
            var buffer = new byte[count];
            ReadProcessMemory((int) Process.Handle, address, buffer, buffer.Length, ref bytesRead);
            return buffer;
        }

        /// <summary>
        ///     Returns the base address of a module.
        ///     If fails to find, will return 0.
        ///     * Remember to include '.dll' at the end of the Module Name.
        /// </summary>
        internal int GetModuleBaseAddress(string moduleName)
        {
            foreach (ProcessModule pm in Process.Modules)
                if (pm.ModuleName == moduleName)
                    return (int) pm.BaseAddress;
            return 0;
        }

        /// <summary>
        ///     Returns the end address of a module.
        ///     If fails to find, will return 0.
        ///     * Remember to include '.dll' at the end of the Module Name.
        /// </summary>
        internal int GetModuleEndAddress(string moduleName)
        {
            foreach (ProcessModule pm in Process.Modules)
                if (pm.ModuleName == moduleName)
                    return (int) pm.BaseAddress + pm.ModuleMemorySize;
            return 0;
        }

        /// <summary>
        ///     Returns the short value from given address.
        /// </summary>
        internal short ReadInt16(int address)
        {
            var bytesRead = 0;
            var buffer = new byte[2];
            ReadProcessMemory((int) Process.Handle, address, buffer, buffer.Length, ref bytesRead);
            return BitConverter.ToInt16(buffer, 0);
        }

        /// <summary>
        ///     Returns the int value from given address.
        /// </summary>
        internal int ReadInt32(int address)
        {
            var bytesRead = 0;
            var buffer = new byte[4];
            ReadProcessMemory((int) Process.Handle, address, buffer, buffer.Length, ref bytesRead);
            return BitConverter.ToInt32(buffer, 0);
        }

        /// <summary>
        ///     Returns the long value from given address.
        /// </summary>
        internal long ReadInt64(int address)
        {
            var bytesRead = 0;
            var buffer = new byte[8];
            ReadProcessMemory((int) Process.Handle, address, buffer, buffer.Length, ref bytesRead);
            return BitConverter.ToInt64(buffer, 0);
        }

        /// <summary>
        ///     Returns the float value from given address.
        /// </summary>
        internal float ReadFloat(int address)
        {
            var bytesRead = 0;
            var buffer = new byte[4];
            ReadProcessMemory((int) Process.Handle, address, buffer, buffer.Length, ref bytesRead);
            return BitConverter.ToSingle(buffer, 0);
        }

        /// <summary>
        ///     Writes the float value to a given address.
        /// </summary>
        internal void WriteFloat(int address, float f)
        {
            var bytesWrite = 0;
            var buffer = BitConverter.GetBytes(f);
            WriteProcessMemory((int) Process.Handle, address, buffer, buffer.Length, ref bytesWrite);
        }

        /// <summary>
        ///     Writes the byte to a given address.
        /// </summary>
        internal void WriteByte(int address, byte b)
        {
            var bytesWrite = 0;
            byte[] buffer = {b};
            WriteProcessMemory((int) Process.Handle, address, buffer, buffer.Length, ref bytesWrite);
        }

        /// <summary>
        ///     Writes the bytes to a given address.
        /// </summary>
        internal void WriteBytes(int address, byte[] b)
        {
            var bytesWrite = 0;
            var buffer = b;
            WriteProcessMemory((int) Process.Handle, address, buffer, buffer.Length, ref bytesWrite);
        }

        /// <summary>
        ///     Returns the double value from given address.
        /// </summary>
        internal double ReadDouble(int address)
        {
            var bytesRead = 0;
            var buffer = new byte[8];
            ReadProcessMemory((int) Process.Handle, address, buffer, buffer.Length, ref bytesRead);
            return BitConverter.ToDouble(buffer, 0);
        }

        /// <summary>
        ///     Returns the whole string from given address.
        /// </summary>
        internal string ReadString(int address)
        {
            var bytes = new List<byte>();
            byte lastByte;
            while (true) {
                lastByte = ReadBytes(address + bytes.Count, 1)[0];
                if (lastByte != 0)
                    bytes.Add(lastByte);
                else
                    break;
            }

            return Encoding.UTF8.GetString(bytes.ToArray());
        }

        /// <summary>
        ///     Returns the string from given address, with a given length.
        /// </summary>
        internal string ReadString(int address, int length) => Encoding.UTF8.GetString(ReadBytes(address, length));

        /// <summary>
        ///     Returns the byte from given address.
        /// </summary>
        internal byte ReadByte(int address) => ReadBytes(address, 1)[0];

        public double ReadDouble(long address)
        {
            var bytesRead = 0;
            var buffer = new byte[8];
            ReadProcessMemory((int) Process.Handle, address, buffer, buffer.Length, ref bytesRead);
            return BitConverter.ToDouble(buffer, 0);
        }
        internal byte[] ReadBytes(long address, int count)
        {
            var bytesRead = 0;
            var buffer = new byte[count];
            ReadProcessMemory((int) Process.Handle, address, buffer, buffer.Length, ref bytesRead);
            return buffer;
        }
        internal long SignatureScan(
            byte[] pattern, string mask, int offset, long startAddress, long endAddress, int bufferSize = 32768)
        {
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            if (pattern.Length != mask.Length) {
                stopwatch.Stop();
                return -1;
            }foreach (var c in mask)
                if (c != 'x' && c != '?') {
                    stopwatch.Stop();
                    return -1;
                }
            var currentAddress = startAddress;
            byte[] readBytes;
            while (currentAddress < endAddress) {
                readBytes = ReadBytes(currentAddress, bufferSize);
                for (var i = 0; i < readBytes.Length - pattern.Length; i++)
                for (var x = 0; x < pattern.Length; x++)
                    if (mask[x] == 'x') {
                        if (readBytes[i + x] != pattern[x])
                            break;
                        if (x == pattern.Length - 1) {
                            stopwatch.Stop();
                            Console.WriteLine("Time elapsed: {0}", stopwatch.Elapsed);
                            return currentAddress + i + offset;
                        }
                    }
                    else if (mask[x] == '?' && x == pattern.Length - 1) {
                        stopwatch.Stop();
                        Console.WriteLine("Time elapsed: {0}", stopwatch.Elapsed);
                        return currentAddress + i + offset;
                    }

                currentAddress += bufferSize;
            }

            return -1;
        }
        
        public long SignatureScan(string pattern, string mask, int offset, long startAddress, long endAddress, int bufferSize = 32768)
        {
            bool spaceDelimited;
            if (pattern.Length == 0)
                return -1;
            if (pattern.Length > 2)
                spaceDelimited = pattern[2] == 0x20;
            else
                spaceDelimited = false;
            if (spaceDelimited) {
                if ((pattern.Length + 1) % 3 != 0)
                    return -1;
            }
            else if (pattern.Length % 2 != 0) {
                return -1;
            }

            var len = spaceDelimited ? (pattern.Length + 1) / 3 : pattern.Length / 2;
            var result = new byte[len];
            foreach (var c in pattern)
                if (!(c >= 48 && c <= 57 || c >= 65 && c <= 70 || c >= 97 && c <= 102) && c != 0x20)
                    return -1;
            for (var i = 0; i < len; i++)
                if (spaceDelimited)
                    result[i] = Convert.ToByte("" + pattern[i * 3] + pattern[i * 3 + 1], 16);
                else
                    result[i] = Convert.ToByte("" + pattern[i * 2] + pattern[i * 2 + 1], 16);

            // Scanning the process looking for the signature.
            return SignatureScan(result, mask, offset, startAddress, endAddress, bufferSize);
        }

        public long ReadInt64(long readInt64)
        {
            var bytesRead = 0;
            var buffer = new byte[8];
            ReadProcessMemory((int) Process.Handle, readInt64, buffer, buffer.Length, ref bytesRead);
            return BitConverter.ToInt64(buffer, 0);
        }
    }
}