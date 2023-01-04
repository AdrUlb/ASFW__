using ASFW.Graphics.OpenGL;

namespace ASFW.Platform.Android;

public class AndroidAsfwPlatform : IAsfwPlatform
{
	internal static readonly GlProvider Gl = new();
	internal static global::Android.Content.Res.AssetManager AndroidAssetManager = null!;

	public static void SetAndroidAssetManager(global::Android.Content.Res.AssetManager androidAssetManager)
	{
		AndroidAssetManager = androidAssetManager;
	}
	
	public void OnInit()
	{
		if (AndroidAssetManager == null)
			throw new InvalidOperationException();
	}

	public void OnQuit()
	{
		
	}

	public void OnEvents()
	{
		
	}

	public IGlProvider GetGlProvider() => Gl;
	public IAssetLoader CreateAssetLoader(string bundle) => new AssetLoader(bundle, AndroidAssetManager);
}
