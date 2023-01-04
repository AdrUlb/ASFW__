using System.Runtime.InteropServices;

namespace ASFW.Platform.Desktop.GLFW;

[StructLayout(LayoutKind.Sequential)]
internal readonly struct Window
{
	public static readonly Window None = new(IntPtr.Zero);

	private readonly IntPtr handle;

	public override bool Equals(object? obj) => ((Window)obj!).handle == handle;

	public override int GetHashCode() => handle.GetHashCode();

	public static bool operator ==(Window win1, Window win2) => Equals(win1, win2);
	public static bool operator !=(Window win1, Window win2) => !Equals(win1, win2);

	private Window(IntPtr handle) => this.handle = handle;
}
