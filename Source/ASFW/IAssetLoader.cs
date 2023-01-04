using System.Diagnostics.CodeAnalysis;
using ASFW.Graphics;
using ASFW.Graphics.OpenGL;
using ASFW.Graphics.OpenGL.Abstractions;

namespace ASFW;

public interface IAssetLoader
{
	public bool TryLoadAsStream(string path, [MaybeNullWhen(false)] out Stream result);
	
	public bool TryLoadAsString(string path, [MaybeNullWhen(false)] out string result)
	{
		if (!TryLoadAsStream(path, out var stream))
		{
			result = null;
			return false;
		}

		using var sr = new StreamReader(stream);
		result = sr.ReadToEnd();

		return true;
	}

	public GLProgram LoadShaderProgram(string name)
	{
		var basePath = Path.Combine("Shaders", name);
		var vertexPath = Path.Combine(basePath, "Vertex.glsl");
		var fragmentPath = Path.Combine(basePath, "Fragment.glsl");

		var shaders = new List<GLShader>();
		if (TryLoadAsString(vertexPath, out var vertexSource))
			shaders.Add(new(Asfw.Gl, GlShaderType.VertexShader, vertexSource));
		
		if (TryLoadAsString(fragmentPath, out var fragmentSource))
			shaders.Add(new(Asfw.Gl, GlShaderType.FragmentShader, fragmentSource));
		
		var program = new GLProgram(Asfw.Gl, shaders.ToArray());
		
		foreach (var shader in shaders)
			shader.Dispose();
		
		return program;
	}

	public Texture? LoadTexture(string name)
	{
		var basePath = Path.Combine("Textures", name);

		foreach (var ext in new[] { "png" })
		{
			if (!TryLoadAsStream($"{basePath}.{ext}", out var stream))
				continue;

			var tex = new Texture(stream);
			stream.Dispose();
			return tex;
		}

		return null;
	}
}
