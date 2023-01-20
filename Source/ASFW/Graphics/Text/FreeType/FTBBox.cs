using System.Runtime.InteropServices;
using FT_Pos = System.Int64;

namespace ASFW.Graphics.Text.FreeType;

[StructLayout(LayoutKind.Sequential)]
internal struct FTBBox
{
	public FT_Pos XMin, YMin;
	public FT_Pos XMax, YMax;
}
