using System.Runtime.InteropServices;
using FT_Pos = System.Int64;

namespace ASFW.Graphics.Text.FreeType;

[StructLayout(LayoutKind.Sequential)]
internal struct FTGlyphMetrics
{
	public FT_Pos Width;
	public FT_Pos Height;

	public FT_Pos HoriBearingX;
	public FT_Pos HoriBearingY;
	public FT_Pos HoriAdvance;

	public FT_Pos VertBearingX;
	public FT_Pos VertBearingY;
	public FT_Pos VertAdvance;
}
