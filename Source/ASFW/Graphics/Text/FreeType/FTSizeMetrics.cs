using System.Runtime.InteropServices;
using FT_Fixed = System.Int64;
using FT_Pos = System.Int64;

namespace ASFW.Graphics.Text.FreeType;

[StructLayout(LayoutKind.Sequential)]
internal struct FTSizeMetrics
{
	public ushort XPPEM;
	public ushort YPPEM;

	public FT_Fixed XScale;
	public FT_Fixed YScale;

	public FT_Pos Ascender;
	public FT_Pos Descender;
	public FT_Pos Height;
	public FT_Pos MaxAdvance;
}
