using System.Runtime.InteropServices;

namespace ASFW.Extension.Text.FreeType;

[StructLayout(LayoutKind.Sequential)]
internal unsafe struct FTFaceRec
{
	public long NumFaces;
	public long FaceIndex;

	public long FaceFlags;
	public long StyleFlags;

	public long NumGlyphs;

	public nuint FamilyName;
	public nuint StyleName;

	public int NumFixedSizes;
	public nuint AvailableSizes;

	public int NumCharmaps;
	public nuint Charmaps;

	public FTGeneric Generic;

	public FTBBox BBox;

	public ushort UnitsPerEM;
	public short Ascender;
	public short Descender;
	public short Height;

	public short MaxAdvanceWidth;
	public short MaxAdvanceHeight;

	public short UnderlinePosition;
	public short UnderlineThickness;

	public FTGlyphSlotRec* Glyph;
	public FTSizeRec* Size;
	public nuint Charmap;
}
