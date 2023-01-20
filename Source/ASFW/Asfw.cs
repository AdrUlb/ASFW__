using ASFW.Graphics.OpenGL;
using ASFW.Graphics.Text.FreeType;

namespace ASFW;

public static class Asfw
{
	private static bool init = false;
	internal static IGlProvider Gl = null!;
	internal static IAssetLoader AssetLoader = null!;
	internal static FTLibrary FtLibrary;

	private static IAsfwPlatform platform = null!;
	
	internal static void AssertInit()
	{
		if (!init)
			throw new("Assertion failed: ASFW was not initialized.");
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

		if (FT.InitFreeType(out FtLibrary) != FTError.Ok)
			throw new("Failed to initialize FreeType.");
	}

	public static void Quit()
	{
		if (!init)
			return;

		FT.DoneFreeType(FtLibrary);

		platform.OnQuit();
		
		
		init = false;
	}

	public static IAssetLoader CreateAssetLoader(string bundle) => platform.CreateAssetLoader(bundle);

	public static void DoEvents()
	{
		platform.OnEvents();
	}
}
