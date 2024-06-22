using System.Runtime.InteropServices;

namespace rogalik.Framework.Map.Tile;

[StructLayout(LayoutKind.Explicit)] 
public struct Data
{
    [FieldOffset(0)] public int value; // absolute value
    [FieldOffset(0)] public Kind kind; // byte
    [FieldOffset(1)] public byte subKind;
    [FieldOffset(2)] public short attrs;
}
