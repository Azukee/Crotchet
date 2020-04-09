namespace HoLLy.Memory.CrossPlatform
{
    public class IMemoryRegion
    {
        internal ulong Start { get; }
        internal ulong End { get; }
        
        bool IsReadable { get; }
        bool IsWriteable { get; }
        bool IsExecutable { get; }
        internal bool IsMapped { get; }

        string PermissionString => (IsReadable ? "R" : "") + (IsWriteable ? "W" : "") + (IsExecutable ? "X" : "");
    }
}