using System.Runtime.InteropServices;

namespace ASFW.Platform.Desktop.GLFW;

[StructLayout(LayoutKind.Sequential)]
internal readonly struct Monitor
{
	public static readonly Monitor None = new(0);

	private readonly IntPtr handle;

	private Monitor(IntPtr handle) => this.handle = handle;
}
