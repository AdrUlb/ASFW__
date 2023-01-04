using System.Runtime.InteropServices;
using FT_Pos = System.Int64;

namespace ASFW.Extension.Text.FreeType;

[StructLayout(LayoutKind.Sequential)]
internal struct FTVector
{
	public FT_Pos X;
	public FT_Pos Y;
}
