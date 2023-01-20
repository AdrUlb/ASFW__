using System.Runtime.InteropServices;

namespace ASFW.Graphics.Text.FreeType;

[StructLayout(LayoutKind.Sequential)]
internal unsafe struct Outline
{
	public short NContours;
	public short NPoints;

	public FTVector* Points;
	public byte* Tags;
	public short* Contours;

	public int Flags;
}
