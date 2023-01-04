using System.Runtime.InteropServices;

namespace ASFW.Extension.Text.FreeType;

[StructLayout(LayoutKind.Sequential)]
internal unsafe struct FTBitmap
{
	public uint Rows;
	public uint Width;
	public int Pitch;
	public byte* Buffer;
	public ushort NumGrays;
	public byte PixelMode;
	public byte PaletteMode;
	public void* Palette;
}
