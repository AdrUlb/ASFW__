using System.Runtime.InteropServices;

namespace ASFW.Graphics.Text.FreeType;

[StructLayout(LayoutKind.Sequential)]
internal readonly struct FTLibrary
{
	public static readonly FTLibrary None = new(IntPtr.Zero);

	private readonly IntPtr handle;

	public override bool Equals(object? obj) => ((FTLibrary)obj!).handle == handle;

	public override int GetHashCode() => handle.GetHashCode();

	public static bool operator ==(FTLibrary win1, FTLibrary win2) => Equals(win1, win2);
	public static bool operator !=(FTLibrary win1, FTLibrary win2) => !Equals(win1, win2);

	private FTLibrary(IntPtr handle) => this.handle = handle;
}
