using System.Runtime.InteropServices;
using FT_Fixed = System.Int64;
using FT_Pos = System.Int64;
using FT_Glyph_Format = System.Int32;
using FT_Slot_Internal = System.IntPtr;

namespace ASFW.Graphics.Text.FreeType;

[StructLayout(LayoutKind.Sequential)]
internal unsafe struct FTGlyphSlotRec
{
	public FTLibrary Library;
	public FTFaceRec* Face;
	public FTGlyphSlotRec* Next;
	public uint GlyphIndex;
	public FTGeneric Generic;

	public FTGlyphMetrics Metrics;
	public FT_Fixed LinearHoriAdvance;
	public FT_Fixed LinearVertAdvance;
	public FTVector Advance;

	public FT_Glyph_Format Format;

	public FTBitmap Bitmap;
	public int BitmapLeft;
	public int BitmapTop;

	public Outline Outline;

	public uint NumSubglyphs;
	public nuint Subglyphs;

	public void* ControlData;
	public long ControlLen;

	public FT_Pos LsbDelta;
	public FT_Pos RsbDelta;

	public void* Other;

	public FT_Slot_Internal Internal;
}
