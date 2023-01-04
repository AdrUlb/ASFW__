using ASFW.Graphics.OpenGL;

namespace ASFW;

public interface IAsfwPlatform
{
	internal protected void OnInit();
	internal protected void OnQuit();
	internal protected void OnEvents();
	internal protected IGlProvider GetGlProvider();
	internal protected IAssetLoader CreateAssetLoader(string bundle);
}
