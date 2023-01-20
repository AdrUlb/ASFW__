using System.Drawing;
using System.Numerics;
using System.Runtime.InteropServices;
using ASFW.Graphics.Text.FreeType;

namespace ASFW.Graphics.Text;

internal class FontCharMetrics
{
	public readonly int BitmapLeft;
	public readonly int BitmapTop;
	public readonly long AdvanceX;
	
	public FontCharMetrics(int bitmapLeft, int bitmapTop, long advanceX)
	{
		BitmapLeft = bitmapLeft;
		BitmapTop = bitmapTop;
		AdvanceX = advanceX;
	}
}

internal class FontChar
{
	public readonly TextureSection TextureSection;
	public readonly FontCharMetrics Metrics;

	public FontChar(TextureSection textureSection, FontCharMetrics metrics)
	{
		TextureSection = textureSection;
		Metrics = metrics;
	}
}

public unsafe class Font : IDisposable
{
	private readonly byte* buffer = null;
	internal readonly FTFaceRec* Face;

	public readonly uint Height;

	private Texture texture;
	private int nextCharStart = 0;
	private readonly Dictionary<char, FontCharMetrics> charMetrics = new();
	private readonly Dictionary<char, FontChar> chars = new();

	public Font(Stream stream, uint height)
	{
		Asfw.AssertInit();

		texture = new((int)height * 10, (int)height);

		Height = height;

		using var br = new BinaryReader(stream);
		var bufferSize = (int)(stream.Length - stream.Position);
		buffer = (byte*)Marshal.AllocHGlobal(bufferSize);
		var bufferSpan = new Span<byte>(buffer, bufferSize);
		if (stream.Read(bufferSpan) != bufferSize)
			throw new("Failed to read font data.");

		var ftError = FT.NewMemoryFace(Asfw.FtLibrary, buffer, bufferSize, 0, out Face);
		if (ftError != FTError.Ok)
			throw new($"Failed to create font face ({ftError}).");

		ftError = FT.SetPixelSizes(Face, 0, height);
		if (ftError != FTError.Ok)
			throw new($"Failed to set pixel size {ftError}.");
	}

	public Font(string filePath, uint height)
	{
		Asfw.AssertInit();

		texture = new((int)height * 10, (int)height);

		Height = height;

		if (FT.NewFace(Asfw.FtLibrary, filePath, 0, out Face) != FTError.Ok)
			throw new($"Failed to create font face from file '{filePath}'.");

		if (FT.SetPixelSizes(Face, 0, height) != FTError.Ok)
			throw new("Failed to set pixel size.");
	}

	internal FontChar GetFontChar(char c)
	{
		if (chars.TryGetValue(c, out var data))
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
			chars.Clear();
		}
		
		if (h > texture.Height)
		{
			var newTexture = new Texture(texture.Width, h);
        	texture.Dispose();
        	texture = newTexture;
        	chars.Clear();
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
		
		data = new(tex, new FontCharMetrics(Face->Glyph->BitmapLeft, Face->Glyph->BitmapTop, Face->Glyph->Advance.X));
		chars.Add(c, data);

		return data;
	}
	
	public Vector2 MeasureText(string text)
	{
		var minX = 0f;
		var maxX = 0f;
		var minY = 0f;
		var maxY = 0f;
		
		var position = new Vector2();
		var startX = position.X;

		foreach (var c in text)
		{
			switch (c)
			{
				case '\n':
					position.Y += (int)Face->Size->Metrics.Height >> 6;
					position.X = startX;
					continue;
			}

			var data = GetFontChar(c);
			var texSec = data.TextureSection;

			var pos = new Vector2(
				position.X + data.Metrics.BitmapLeft,
				position.Y - data.Metrics.BitmapTop + Height
			);

			var size = new Vector2(texSec.Width, texSec.Height);

			minX = Math.Min(minX, pos.X);
			minY = Math.Min(minY, pos.Y);
			maxX = Math.Max(maxX, pos.X + size.X - 1);
			maxY = Math.Max(maxY, pos.Y + size.Y - 1);
			
			position.X += data.Metrics.AdvanceX >> 6;
		}

		var width = maxX - minX + 1;
		var height = maxY - minY + 1;

		return new(width, height);
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
