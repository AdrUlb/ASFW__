using System.Drawing;

namespace ASFW.Graphics.Imaging;

public sealed class Image
{
	private readonly Color[] pixels;

	public readonly int Width;
	public readonly int Height;

	public Image(Stream stream)
	{
		if (PngParser.TryParse(stream, out var p, out Width, out Height))
		{
			pixels = p;
			return;
		}

		throw new NotSupportedException("Image format not supported.");
	}

	public Color[] GetPixels()
	{
		return pixels.ToArray();
	}
}
