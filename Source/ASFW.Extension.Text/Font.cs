using System.Drawing;
using System.Runtime.InteropServices;
using ASFW.Graphics;
using ASFW.Extension.Text.FreeType;

namespace ASFW.Extension.Text;

internal class FontCharData
{
	public readonly TextureSection TextureSection;
	public readonly int BitmapLeft;
	public readonly int BitmapTop;
	public readonly long AdvanceX;

	public FontCharData(TextureSection textureSection, int bitmapLeft, int bitmapTop, long advanceX)
	{
		TextureSection = textureSection;
		BitmapLeft = bitmapLeft;
		BitmapTop = bitmapTop;
		AdvanceX = advanceX;
	}
}

public unsafe class Font : IDisposable
{
	private readonly byte* buffer = null;
	internal readonly FTFaceRec* Face;

	public readonly uint Height;

	private Texture texture;
	private int nextCharStart = 0;
	private readonly Dictionary<char, FontCharData> dataCache = new();

	public Font(Stream stream, uint height)
	{
		TextAsfwExtension.AssertInit();

		texture = new((int)height * 10, (int)height);

		Height = height;

		using var br = new BinaryReader(stream);
		var bufferSize = (int)(stream.Length - stream.Position);
		buffer = (byte*)Marshal.AllocHGlobal(bufferSize);
		var bufferSpan = new Span<byte>(buffer, bufferSize);
		if (stream.Read(bufferSpan) != bufferSize)
			throw new("Failed to read font data.");

		var ftError = FT.NewMemoryFace(TextAsfwExtension.FtLibrary, buffer, bufferSize, 0, out Face);
		if (ftError != FTError.Ok)
			throw new($"Failed to create font face ({ftError}).");

		ftError = FT.SetPixelSizes(Face, 0, height);
		if (ftError != FTError.Ok)
			throw new($"Failed to set pixel size {ftError}.");
	}

	public Font(string filePath, uint height)
	{
		TextAsfwExtension.AssertInit();

		texture = new((int)height * 10, (int)height);

		Height = height;

		if (FT.NewFace(TextAsfwExtension.FtLibrary, filePath, 0, out Face) != FTError.Ok)
			throw new($"Failed to create font face from file '{filePath}'.");

		if (FT.SetPixelSizes(Face, 0, height) != FTError.Ok)
			throw new("Failed to set pixel size.");
	}

	internal FontCharData GetFontCharData(char c)
	{
		if (dataCache.TryGetValue(c, out var data))
			return data;

		FT.LoadChar(Face, c, FTLoadFlags.Render);

		var slot = Face->Glyph;
		var w = (int)slot->Bitmap.Width;
		var h = (int)slot->Bitmap.Rows;
		var p = slot->Bitmap.Pitch;

		var raw = new Span<byte>(slot->Bitmap.Buffer, p * h);

		if (nextCharStart + w > texture.Width)
		{
			var newTexture = new Texture(texture.Width * 2, texture.Height);
			texture.Dispose();
			texture = newTexture;
			dataCache.Clear();
		}
		
		if (h > texture.Height)
		{
			var newTexture = new Texture(texture.Width, h);
        	texture.Dispose();
        	texture = newTexture;
        	dataCache.Clear();
		}

		var tex = new TextureSection(texture, nextCharStart, 0, w, h);
		nextCharStart += w;
		
		texture.Lock();
		for (var y = 0; y < h; y++)
		{
			for (var x = 0; x < w; x++)
			{
				var rawI = x + (y * p);
				tex[x, y] = Color.FromArgb(raw[rawI], 255, 255, 255);
			}
		}
		texture.Unlock();

		data = new(tex, slot->BitmapLeft, slot->BitmapTop, slot->Advance.X);
		dataCache.Add(c, data);

		return data;
	}

	~Font() => Dispose();

	public void Dispose()
	{
		GC.SuppressFinalize(this);
		FT.DoneFace(Face);
		if (buffer != null)
			Marshal.FreeHGlobal((nint)buffer);
	}
}
