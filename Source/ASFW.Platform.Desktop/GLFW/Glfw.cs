using System.Reflection;
using System.Runtime.InteropServices;

namespace ASFW.Platform.Desktop.GLFW;

internal static partial class Glfw
{
	public delegate void WindowCloseCallback(Window window);
	public delegate void FramebufferSizeCallback(Window window, int width, int height);
	public delegate void MouseButtonCallback(Window window, int button, MouseButtonAction action, ModifierKeys mods);
	public delegate void CursorPosCallback(Window window, double xpos, double ypos);
	public delegate void KeyCallback(Window window, int key, int scancode, int action, int mods);

	static Glfw()
	{
		nint glfw = 0;
		var l = new object();
		NativeLibrary.SetDllImportResolver(Assembly.GetExecutingAssembly(), (name, _, _) =>
		{
			if (name != "glfw")
				return 0;

			lock (l)
			{
				if (glfw != 0)
					return glfw;

				var ridOs = 0 switch
				{
					_ when RuntimeInformation.IsOSPlatform(OSPlatform.Linux) => "linux",
					_ when RuntimeInformation.IsOSPlatform(OSPlatform.Windows) => "win",
					_ => throw new PlatformNotSupportedException("Operating system not supported.")
				};

				var ridPlatform = RuntimeInformation.OSArchitecture switch
				{
					Architecture.X64 => "x64",
					_ => throw new PlatformNotSupportedException("System architecture not supported.")
				};

				var rid = $"{ridOs}-{ridPlatform}";

				name = ridOs switch
				{
					"linux" => "libglfw.so",
					"win" => "glfw3.dll",
					_ => throw new NotImplementedException()
				};

				var path = Path.Combine(AppContext.BaseDirectory, "NativeLibs", rid, name);

				if (!File.Exists(path))
					return 0;

				return glfw = NativeLibrary.Load(path);
			}
		});
		AppDomain.CurrentDomain.ProcessExit += (_, _) => NativeLibrary.Free(glfw);
	}

	[LibraryImport("glfw", EntryPoint = "glfwInit")]
	[return: MarshalAs(UnmanagedType.U1)]
	public static partial bool Init();

	[LibraryImport("glfw", EntryPoint = "glfwTerminate")]
	public static partial void Terminate();

	[LibraryImport("glfw", EntryPoint = "glfwGetError", StringMarshalling = StringMarshalling.Utf8)]
	public static partial Error GetError(out string description);

	[LibraryImport("glfw", EntryPoint = "glfwCreateWindow", StringMarshalling = StringMarshalling.Utf8)]
	public static partial Window CreateWindow(int width, int height, string title, Monitor monitor, Window share);

	[LibraryImport("glfw", EntryPoint = "glfwDestroyWindow")]
	public static partial void DestroyWindow(Window handle);

	[LibraryImport("glfw", EntryPoint = "glfwPollEvents")]
	public static partial void PollEvents();

	[LibraryImport("glfw", EntryPoint = "glfwSetWindowCloseCallback")]
	public static partial WindowCloseCallback SetWindowCloseCallback(Window handle, WindowCloseCallback cbfun);

	[LibraryImport("glfw", EntryPoint = "glfwWindowHint")]
	public static partial void WindowHint(Hint hint, int value);

	public static void WindowHint(Hint hint, bool value) => WindowHint(hint, value ? 1 : 0);
	public static void WindowHint(Hint hint, ClientApi value) => WindowHint(hint, (int)value);
	public static void WindowHint(Hint hint, OpenGlProfile value) => WindowHint(hint, (int)value);
	public static void WindowHint(Hint hint, ContextCreationApi value) => WindowHint(hint, (int)value);

	[LibraryImport("glfw", EntryPoint = "glfwWindowHintString", StringMarshalling = StringMarshalling.Utf8)]
	public static partial void WindowHintString(Hint hint, string value);

	[LibraryImport("glfw", EntryPoint = "glfwMakeContextCurrent")]
	public static partial void MakeContextCurrent(Window handle);

	[LibraryImport("glfw", EntryPoint = "glfwGetProcAddress", StringMarshalling = StringMarshalling.Utf8)]
	public static partial IntPtr GetProcAddress(string procname);

	[LibraryImport("glfw", EntryPoint = "glfwSwapBuffers")]
	public static partial void SwapBuffers(Window window);

	[LibraryImport("glfw", EntryPoint = "glfwSwapInterval")]
	public static partial void SwapInterval(int interval);

	[LibraryImport("glfw", EntryPoint = "glfwSetFramebufferSizeCallback")]
	public static partial FramebufferSizeCallback SetFramebufferSizeCallback(Window window, FramebufferSizeCallback callback);

	[LibraryImport("glfw", EntryPoint = "glfwGetWindowSize")]
	public static partial void GetWindowSize(Window handle, out int width, out int height);

	[LibraryImport("glfw", EntryPoint = "glfwGetFramebufferSize")]
	public static partial void GetFramebufferSize(Window handle, out int width, out int height);

	[LibraryImport("glfw", EntryPoint = "glfwSetMouseButtonCallback")]
	public static partial MouseButtonCallback SetMouseButtonCallback(Window window, MouseButtonCallback callback);

	[LibraryImport("glfw", EntryPoint = "glfwSetWindowTitle", StringMarshalling = StringMarshalling.Utf8)]
	public static partial void SetWindowTitle(Window window, string title);

	[LibraryImport("glfw", EntryPoint = "glfwGetWindowAttrib")]
	public static partial int GetWindowAttrib(Window window, WindowAttribute attrib);

	[LibraryImport("glfw", EntryPoint = "glfwShowWindow")]
	public static partial void ShowWindow(Window window);

	[LibraryImport("glfw", EntryPoint = "glfwHideWindow")]
	public static partial void HideWindow(Window window);

	[LibraryImport("glfw", EntryPoint = "glfwSetCursorPosCallback")]
	public static partial CursorPosCallback SetCursorPosCallback(Window window, CursorPosCallback callback);

	[LibraryImport("glfw", EntryPoint = "glfwSetKeyCallback")]
	public static partial KeyCallback SetKeyCallback(Window window, KeyCallback callback);

	[LibraryImport("glfw", EntryPoint = "glfwGetMonitorContentScale")]
	public static partial void GetMonitorContentScale(Monitor monitor, out float xscale, out float yscale);

	[LibraryImport("glfw", EntryPoint = "glfwGetPrimaryMonitor")]
	public static partial Monitor GetPrimaryMonitor();


#if !AOT || AOT_WINDOWS
	[LibraryImport("glfw", EntryPoint = "glfwGetWin32Window")]
	public static partial IntPtr GetWin32Window(Window window);
#endif
}
