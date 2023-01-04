using ASFW.Graphics.OpenGL;

namespace ASFW;

public static class Asfw
{
	private static bool init = false;
	internal static IGlProvider Gl = null!;
	internal static IAssetLoader AssetLoader = null!;

	private static IAsfwPlatform platform = null!;
	private static readonly List<IAsfwExtension> extensions = new();
	
	internal static void AssertInit()
	{
		if (!init)
			throw new("Assertion failed: ASFW was not initialized.");
	}
	
	internal static void AssertNotInit()
	{
		if (init)
			throw new("Assertion failed: ASFW was initialized.");
	}

	public static void EnableExtension<T>() where T : IAsfwExtension
	{
		AssertNotInit();

		if (extensions.Any(ext => ext.GetType() == typeof(T)))
			return;
		
		extensions.Add(Activator.CreateInstance<T>());
	}
	
	public static void Init<T>() where T : IAsfwPlatform
	{
		if (init)
			return;

		init = true;

		platform = Activator.CreateInstance<T>();
		platform.OnInit();
		AssetLoader = platform.CreateAssetLoader("ASFW");
		Gl = platform.GetGlProvider();

		foreach (var ext in extensions)
			ext.OnInit();
	}

	public static void Quit()
	{
		if (!init)
			return;

		foreach (var ext in extensions)
			ext.OnQuit();

		platform.OnQuit();
		
		init = false;
	}

	public static IAssetLoader CreateAssetLoader(string bundle) => platform.CreateAssetLoader(bundle);

	public static void DoEvents()
	{
		platform.OnEvents();
	}
}
