using ASFW.Graphics.OpenGL;
using ASFW.Platform.Desktop.GLFW;

namespace ASFW.Platform.Desktop;

public sealed class DesktopAsfwPlatform : IAsfwPlatform
{
	private static readonly List<Window> windows = new();
	private static readonly object windowsLock = new();
	internal static readonly GlProvider Gl = new();
	
	private static bool init = false;

	internal static void AssertInit()
	{
		if (!init)
			throw new("Assertion failed: ASFW Text extension was not initialized.");
	}

	public void OnInit()
	{
		if (!Glfw.Init())
		{
			var error = Glfw.GetError(out var description);
			throw new($"Failed to initialize GLFW: [{error}] {description}.");
		}
		
		init = true;
	}

	public void OnQuit()
	{
		init = false;
		lock (windowsLock)
			foreach (var win in windows.ToArray())
				win.Dispose();

		Glfw.Terminate();
	}

	public void OnEvents()
	{
		Glfw.PollEvents();
	}
	
	public IGlProvider GetGlProvider() => Gl;
	public IAssetLoader CreateAssetLoader(string bundle) => new AssetLoader(bundle);

	internal static void RegisterWindow(Window window)
	{
		lock (windowsLock)
			windows.Add(window);
	}

	internal static void UnregisterWindow(Window window)
	{
		lock (windowsLock)
			windows.Remove(window);
	}
	
}
