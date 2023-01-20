using System.Runtime.InteropServices;

namespace ASFW.Graphics.Text.FreeType;

[StructLayout(LayoutKind.Sequential)]
internal unsafe struct FTGeneric
{
	public void* Data;
	public void* Finalizer;
}
