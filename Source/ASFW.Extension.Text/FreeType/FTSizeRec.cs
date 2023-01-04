using System.Runtime.InteropServices;
using FT_Size_Internal = System.IntPtr;

namespace ASFW.Extension.Text.FreeType;

[StructLayout(LayoutKind.Sequential)]
internal unsafe struct FTSizeRec
{
	public FTFaceRec* Face;
	public FTGeneric Generic;
	public FTSizeMetrics Metrics;
	public FT_Size_Internal Internal;
}
