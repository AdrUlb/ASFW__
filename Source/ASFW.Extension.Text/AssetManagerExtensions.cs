namespace ASFW.Extension.Text;

public static class AssetManagerExtensions
{
	public static Font? LoadFont(this IAssetLoader assetLoader, string name, string variant, uint height)
	{
		var basePath = Path.Combine("Fonts", name);
		var filePath = Path.Combine(basePath, $"{name}-{variant}.ttf");

		if (!assetLoader.TryLoadAsStream(filePath, out var stream))
			return null;

		var font = new Font(stream, height);
		
		stream.Dispose();

		return font;
	}
}
