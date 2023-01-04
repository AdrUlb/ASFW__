using System.Diagnostics.CodeAnalysis;
using ASFW.Graphics;
using ASFW.Graphics.OpenGL;
using ASFW.Graphics.OpenGL.Abstractions;

namespace ASFW.Platform.Desktop;

public sealed class AssetLoader : IAssetLoader
{
	private readonly string bundle;

	public AssetLoader(string bundle)
	{
		this.bundle = bundle;
	}

	private string GetFullPath(string path) => Path.Combine(AppContext.BaseDirectory, "Assets", bundle, path.Replace('/', Path.DirectorySeparatorChar));

	public bool TryLoadAsStream(string path, [MaybeNullWhen(false)] out Stream result)
	{
		if (path.StartsWith('/'))
			throw new($"Asset path may not start with a slash ('{path}').");

		path = GetFullPath(path);
		if (!File.Exists(path))
		{
			result = null;
			return false;
		}

		result = File.OpenRead(path);
		return true;
	}
}
