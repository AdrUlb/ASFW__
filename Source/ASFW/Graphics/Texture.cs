using ASFW.Imaging;
using System.Drawing;
using ASFW.Graphics.OpenGL;
using ASFW.Graphics.OpenGL.Abstractions;

namespace ASFW.Graphics;

public sealed class Texture : IDisposable
{
	private readonly Color[] pixels;

	public readonly int Width;
	public readonly int Height;

	private bool locked = false;
	
	public Texture(Stream stream)
	{
		Asfw.AssertInit();

		var image = new Image(stream);
		Width = image.Width;
		Height = image.Height;
		pixels = image.GetPixels();
	}

	public Texture(int width, int height)
	{
		Width = width;
		Height = height;

		pixels = new Color[width * height];
	}

	public Texture(IReadOnlyCollection<Color> pixels, int width, int height)
	{
		if (pixels.Count != width * height)
			throw new InvalidOperationException();

		Width = width;
		Height = height;

		this.pixels = pixels.ToArray();
	}

	public void Lock()
	{
		if (locked)
			return;
		
		locked = true;
	}

	public void Unlock()
	{
		if (!locked)
			return;
		
		foreach (var dict in Renderer.TextureCache)
		{
			if (!dict.Value.ContainsKey(this))
				continue;
			
			if (!Renderer.TextureCache[dict.Key].TryRemove(this, out var glTex))
				continue;
				
			glTex.Dispose();
		}

		locked = false;
	}

	public Color this[int x, int y]
	{
		get
		{
			if (x < 0 || y < 0 || x >= Width || y >= Height)
				throw new IndexOutOfRangeException();

			return pixels[x + (y * Width)];
		}

		set
		{
			if (!locked)
				return;
			
			if (x < 0 || y < 0 || x >= Width || y >= Height)
				throw new IndexOutOfRangeException();

			pixels[x + (y * Width)] = value;
		}
	}

	internal GLTexture GetGlTexture()
	{
		var texture = new GLTexture(Asfw.Gl, GlTextureTarget.Texture2D);

		texture.Bind(0);

		Asfw.Gl.TexImage2D<byte>(GlTextureTarget.Texture2D, 0, GlTextureInternalFormat.Rgba8, (uint)Width, (uint)Height, 0, GlTextureFormat.Rgba, GlTextureType.UnsignedByte, null);

		texture.Unbind(0);

		UpdateGlTexture(texture);

		texture.Bind(0);

		Asfw.Gl.GenerateMipmap(GlTextureTarget.Texture2D);
		Asfw.Gl.TexParameteri(GlTextureTarget.Texture2D, GlTextureParameterName.WrapS, (int)GlTextureWrap.ClampToEdge);
		Asfw.Gl.TexParameteri(GlTextureTarget.Texture2D, GlTextureParameterName.WrapT, (int)GlTextureWrap.ClampToEdge);
		Asfw.Gl.TexParameter(GlTextureTarget.Texture2D, GlTextureMinFilter.Linear);
		Asfw.Gl.TexParameter(GlTextureTarget.Texture2D, GlTextureMagFilter.Nearest);

		return texture;
	}

	internal void UpdateGlTexture(GLTexture texture)
	{
		texture.Bind(0);

		Span<byte> data = stackalloc byte[Width * 4];

		for (var y = 0; y < Height; y++)
		{
			for (var x = 0; x < Width; x++)
			{
				var texI = x * 4;
				var pixel = pixels[x + (y * Width)];
				data[texI + 0] = pixel.R;
				data[texI + 1] = pixel.G;
				data[texI + 2] = pixel.B;
				data[texI + 3] = pixel.A;
			}

			Asfw.Gl.TexSubImage2D<byte>(GlTextureTarget.Texture2D, 0, 0, y, (uint)Width, 1, GlTextureFormat.Rgba, GlTextureType.UnsignedByte, data);
		}
	}

	public void Dispose()
	{
		Unlock();
		
		foreach (var dict in Renderer.TextureCache.Values)
		{
			foreach (var kv in dict.Where(kv => kv.Key == this))
			{
				kv.Value.Dispose();
				dict.TryRemove(this, out _);
				break;
			}
		}
	}
}
