using System.Diagnostics.CodeAnalysis;

namespace ASFW.Platform.Android;

public class AssetLoader : IAssetLoader
{
	private readonly string bundle;
	private readonly global::Android.Content.Res.AssetManager androidAssetManager;

	public AssetLoader(string bundle, global::Android.Content.Res.AssetManager androidAssetManager)
	{
		this.bundle = bundle;
		this.androidAssetManager = androidAssetManager;
	}

	private string GetFullPath(string path) => Path.Combine(bundle, path.Replace('/', Path.DirectorySeparatorChar));

	public bool TryLoadAsStream(string path, [MaybeNullWhen(false)] out Stream result)
	{
		if (path.StartsWith('/'))
			throw new($"Asset path may not start with a slash ('{path}').");

		path = GetFullPath(path);

		using var assetStream = androidAssetManager.Open(path);
		result = new MemoryStream();
		assetStream.CopyTo(result);
		result.Position = 0;
		return true;
	}
}
