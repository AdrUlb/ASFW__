using System.Drawing;
using System.Numerics;
using System.Runtime.InteropServices;
using ASFW.Graphics;
using ASFW.Graphics.OpenGL;
using ASFW.Platform.Desktop.GLFW;
using Monitor = ASFW.Platform.Desktop.GLFW.Monitor;

namespace ASFW.Platform.Desktop;

public abstract class Window : IRenderContext, IDisposable
{
	private readonly Monitor monitor;
	private readonly GLFW.Window glfwWindow;

	private readonly Glfw.WindowCloseCallback closeCallback;
	private readonly Glfw.FramebufferSizeCallback fbSizeCallback;
	private readonly Glfw.MouseButtonCallback mouseButtonCallback;
	private readonly Glfw.CursorPosCallback cursorPosCallback;
	private readonly Glfw.KeyCallback keyCallback;
	private readonly Glfw.CursorEnterCallback cursorEnterCallback;

	private readonly Renderer renderer;

	public readonly string GLRenderer;
	public readonly string GLVendor;
	public readonly string GLVersion;
	public readonly string GLSLVersion;

	public bool IsRunning { get; private set; } = false;

	private readonly Thread renderThread;

	private bool resized = false;

	private string title;

	private bool isDisposed = false;

	public Size Size
	{
		get
		{
			Glfw.GetWindowSize(glfwWindow, out var w, out var h);
			return new(w, h);
		}
	}

	public Size FramebufferSize
	{
		get
		{
			Glfw.GetFramebufferSize(glfwWindow, out var w, out var h);
			return new(w, h);
		}
	}

	public Vector2 RecommendedScale
	{
		get
		{
			Glfw.GetMonitorContentScale(monitor, out var xscale, out var yscale);
			return new(xscale, yscale);
		}
	}

	public string Title
	{
		get => title;

		set
		{
			title = value;
			Glfw.SetWindowTitle(glfwWindow, title);
		}
	}

	public bool Visible
	{
		get => Glfw.GetWindowAttrib(glfwWindow, WindowAttribute.Visible) != 0;

		set
		{
			if (value)
				Glfw.ShowWindow(glfwWindow);
			else
				Glfw.HideWindow(glfwWindow);
		}
	}

	public bool IsMouseInside { get; private set; }
	public Vector2 MousePosition { get; private set; }

	protected Window(WindowOptions options)
	{
		DesktopASFWPlatform.AssertInit();
		Glfw.WindowHint(Hint.Visible, false);
		Glfw.WindowHint(Hint.ClientApi, ClientApi.OpenGl);
		Glfw.WindowHint(Hint.OpenGlProfile, OpenGlProfile.Core);
		Glfw.WindowHint(Hint.OpenGlForwardCompat, true);
		Glfw.WindowHint(Hint.ContextVersionMajor, 3);
		Glfw.WindowHint(Hint.ContextVersionMinor, 3);

		monitor = Glfw.GetPrimaryMonitor();
		glfwWindow = Glfw.CreateWindow(options.Size.Width, options.Size.Height, title = options.Title, Monitor.None, GLFW.Window.None);

		if (glfwWindow == GLFW.Window.None)
		{
			Glfw.WindowHint(Hint.ContextCreationApi, ContextCreationApi.Native);
			glfwWindow = Glfw.CreateWindow(options.Size.Width, options.Size.Height, options.Title, Monitor.None, GLFW.Window.None);
		}

		Glfw.MakeContextCurrent(glfwWindow);
		renderer = new(this);

		Glfw.SwapInterval(0);
		Glfw.SetWindowCloseCallback(glfwWindow, closeCallback = _ => DoClose());
		Glfw.SetFramebufferSizeCallback(glfwWindow, fbSizeCallback = (_, _, _) => resized = true);
		Glfw.SetMouseButtonCallback(glfwWindow, mouseButtonCallback = (_, button, action, mods) =>
		{
			switch (action)
			{
				case MouseButtonAction.Press:
					OnMouseDown(button, (ModifierKeys)mods);
					break;
				case MouseButtonAction.Release:
					OnMouseUp(button, (ModifierKeys)mods);
					break;
			}
		});
		Glfw.SetCursorPosCallback(glfwWindow, cursorPosCallback = (_, xpos, ypos) =>
		{
			var pos = new Vector2((float)xpos, (float)ypos);
			MousePosition = pos;
			OnMouseMove(pos);
		});
		Glfw.SetKeyCallback(glfwWindow, keyCallback = (_, key, scancode, action, mods) =>
		{
			switch (action)
			{
				case 0: // Release
					OnKeyUp((KeyboardKey)key, (ModifierKeys)mods);
					break;
				case 1: // Press
					OnKeyDown((KeyboardKey)key, (ModifierKeys)mods);
					break;
				case 2: // Repeat
					break;
			}
		});
		Glfw.SetCursorEnterCallback(glfwWindow, cursorEnterCallback = (_, entered) =>
		{
			IsMouseInside = entered != 0;
		});

		GLRenderer = DesktopASFWPlatform.Gl.GetString(GlStringName.Renderer);
		GLVendor = DesktopASFWPlatform.Gl.GetString(GlStringName.Vendor);
		GLVersion = DesktopASFWPlatform.Gl.GetString(GlStringName.Version);
		GLSLVersion = DesktopASFWPlatform.Gl.GetString(GlStringName.ShadingLanguageVersion);

		Glfw.MakeContextCurrent(GLFW.Window.None);

		renderThread = new(() =>
		{
			Glfw.MakeContextCurrent(glfwWindow);

			while (IsRunning)
			{
				DoRender();
				if (!resized)
					continue;

				resized = false;
				renderer.UpdateViewportAndProjection();
			}

			Glfw.MakeContextCurrent(GLFW.Window.None);
		});

		DesktopASFWPlatform.RegisterWindow(this);
	}

	protected Window() : this(WindowOptions.Default) { }

	public bool TrySetDarkMode(bool enabled)
	{
#if !AOTBUILD || AOTBUILD_WINDOWS
		[DllImport("dwmapi.dll")]
		static extern int DwmSetWindowAttribute(IntPtr hwnd, int dwAttribute, ref int pvAttribute, int cbAttribute);

		const int dwmaUseImmersiveDarkModePre18985 = 19;
		const int dwmaUseImmersiveDarkMode = 20;

		if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
			return false;

		if (Environment.OSVersion.Version.Major < 10)
			return false;

		if (Environment.OSVersion.Version.Build < 17763)
			return false;

		var attrib = Environment.OSVersion.Version.Build < 18985 ? dwmaUseImmersiveDarkModePre18985 : dwmaUseImmersiveDarkMode;
		var value = enabled ? 1 : 0;
		return DwmSetWindowAttribute(Glfw.GetWin32Window(glfwWindow), attrib, ref value, sizeof(int)) == 0;
#else
		return false;
#endif
	}

	protected virtual void OnClose()
	{
		Stop();
	}

	protected virtual void OnRender(Renderer renderer) { }

	protected virtual void OnMouseDown(int button, ModifierKeys modifiers) { }

	protected virtual void OnMouseUp(int button, ModifierKeys modifiers) { }

	protected virtual void OnKeyDown(KeyboardKey key, ModifierKeys modifiers) { }
	protected virtual void OnKeyUp(KeyboardKey key, ModifierKeys modifiers) { }

	protected virtual void OnMouseMove(Vector2 position) { }

	internal void DoClose() => OnClose();

	internal void DoRender()
	{
		OnRender(renderer);
		renderer.CommitBatch();
		Glfw.SwapBuffers(glfwWindow);
	}

	public void Start()
	{
		if (IsRunning)
			return;

		IsRunning = true;
		renderThread.Start();
	}

	public void Stop()
	{
		if (!IsRunning)
			return;

		IsRunning = false;

		while (renderThread.IsAlive)
			Thread.Sleep(1);
	}

	~Window() => Dispose();

	public virtual void Dispose()
	{
		if (isDisposed)
			return;

		Stop();

		GC.SuppressFinalize(this);

		DesktopASFWPlatform.UnregisterWindow(this);

		Glfw.MakeContextCurrent(glfwWindow);
		renderer.Dispose();
		Glfw.MakeContextCurrent(GLFW.Window.None);

		Glfw.DestroyWindow(glfwWindow);

		isDisposed = true;
	}
}
