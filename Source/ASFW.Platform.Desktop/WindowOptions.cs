using System.Drawing;

namespace ASFW.Platform.Desktop;

public struct WindowOptions
{
	public static readonly WindowOptions Default = new();
	
	public string Title = "ASFW Window";
	public Size Size = new(640, 480);
	public bool Resizable = true;

	public WindowOptions() { }
}
