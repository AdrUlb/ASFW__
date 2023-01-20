using System.Buffers.Binary;
using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using System.IO.Compression;
using System.Text;

namespace ASFW.Graphics.Imaging;

internal static class PngParser
{
	private enum ColorType : byte
	{
		Grayscale = 0,
		Truecolor = 2,
		Indexed = 3,
		GrayscaleAlpha = 4,
		TruecolorAlpha = 6
	}

	private enum CompressionMethod : byte
	{
		Deflate = 0,
	}

	private enum FilterMethod : byte
	{
		Prediction = 0
	}

	private enum InterlaceMethod : byte
	{
		None = 0,
		Adam7 = 1
	}

	private enum PredictionFilterType : byte
	{
		None = 0,
		Sub = 1,
		Up = 2,
		Average = 3,
		Paeth = 4
	}

	private class IhdrData
	{
		public readonly uint Width;
		public readonly uint Height;
		public readonly byte BitDepth;
		public readonly ColorType ColorType;
		public readonly CompressionMethod CompressionMethod;
		public readonly FilterMethod FilterMethod;
		public readonly InterlaceMethod InterlaceMethod;

		public IhdrData(uint width, uint height, byte bitDepth, ColorType colorType, CompressionMethod compressionMethod, FilterMethod filterMethod, InterlaceMethod interlaceMethod)
		{
			Width = width;
			Height = height;
			BitDepth = bitDepth;
			ColorType = colorType;
			CompressionMethod = compressionMethod;
			FilterMethod = filterMethod;
			InterlaceMethod = interlaceMethod;
		}
	}

	public static bool TryParse(Stream stream, [MaybeNullWhen(false)] out Color[] pixels, out int width, out int height)
	{
		ReadOnlySpan<byte> pngMagic = stackalloc byte[] { 0x89, 0x50, 0x4E, 0x47, 0x0D, 0x0A, 0x1A, 0x0A };

		Span<byte> magic = stackalloc byte[pngMagic.Length];

		if (stream.Read(magic) != pngMagic.Length || !magic.SequenceEqual(pngMagic))
		{
			pixels = null;
			width = -1;
			height = -1;
			return false;
		}

		Span<byte> buf4 = stackalloc byte[4];

		IhdrData? ihdrData = null;

		var idatOffsets = new List<long>();

		var iend = false;

		while (!iend)
		{
			if (stream.Read(buf4) != 4)
				throw new("Unexpected end of file.");
			var chunkSize = BinaryPrimitives.ReadUInt32BigEndian(buf4);
			
			if (stream.Read(buf4) != 4)
				throw new("Unexpected end of file.");
			var chunkName = Encoding.ASCII.GetString(buf4);

			switch (chunkName)
			{
				case "IHDR":
					{
						if (chunkSize != 13)
							throw new("PNG chunk 'IHDR' must be 13 bytes in size.");

						if (stream.Read(buf4) != 4)
							throw new("Unexpected end of file.");
						var w = BinaryPrimitives.ReadUInt32BigEndian(buf4);
						
						if (stream.Read(buf4) != 4)
							throw new("Unexpected end of file.");
						var h = BinaryPrimitives.ReadUInt32BigEndian(buf4);
						
						var bitDepth = stream.ReadByte();
						var colorType = stream.ReadByte();
						var compressionMethod = stream.ReadByte();
						var filterMethod = stream.ReadByte();
						var interlaceMethod = stream.ReadByte();
						if (bitDepth == -1 || colorType == -1 || compressionMethod == -1 || filterMethod == -1 || interlaceMethod == -1)
							throw new("Unexpected end of PNG file in 'IHDR' chunk.");
						ihdrData = new(w, h, (byte)bitDepth, (ColorType)colorType, (CompressionMethod)compressionMethod, (FilterMethod)filterMethod, (InterlaceMethod)interlaceMethod);
						break;
					}
				case "IDAT":
					idatOffsets.Add(stream.Position);
					stream.Seek(chunkSize, SeekOrigin.Current);
					break;
				case "IEND":
					if (chunkSize != 0)
						throw new("PNG chunk 'IEND' must have a size of 0.");
					iend = true;
					break;
				default:
					if (char.IsUpper(chunkName[0]))
						Console.WriteLine($"WARNING: skipping critical PNG chunk '{chunkName}' as support for it is not implemented.");
					stream.Seek(chunkSize, SeekOrigin.Current);
					break;
			}

			// Skip CRC
			stream.Seek(4, SeekOrigin.Current);
		}

		if (ihdrData == null)
			throw new("PNG file is missing the required critical chunk 'IHDR'.");

		if (ihdrData.BitDepth != 8)
			throw new NotImplementedException($"PNG bit depth of {ihdrData.BitDepth} not supported.");

		if (ihdrData.ColorType != ColorType.TruecolorAlpha)
			throw new NotImplementedException($"PNG color type {ihdrData.ColorType} not supported.");

		if (ihdrData.CompressionMethod != CompressionMethod.Deflate)
			throw new NotImplementedException($"PNG compression method {ihdrData.CompressionMethod} not supported.");

		if (ihdrData.FilterMethod != FilterMethod.Prediction)
			throw new NotImplementedException($"PNG filter method {ihdrData.FilterMethod} not supported.");

		if (ihdrData.InterlaceMethod != InterlaceMethod.None)
			throw new NotImplementedException($"PNG interlace method {ihdrData.InterlaceMethod} not supported.");

		if (idatOffsets.Count != 1)
			throw new NotImplementedException("PNG images with multiple IDAT chunks are not supported.");

		width = (int)ihdrData.Width;
		height = (int)ihdrData.Height;
		pixels = new Color[width * height];

		stream.Position = idatOffsets[0];
		var decStream = new ZLibStream(stream, CompressionMode.Decompress);

		Span<byte> bufLine = stackalloc byte[width * 4];
		Span<byte> bufPrevLine = stackalloc byte[width * 4];
		for (var y = 0; y < height; y++)
		{
			var filter = (PredictionFilterType)decStream.ReadByte();

			var read = 0;

			while (read < bufLine.Length)
				read += decStream.Read(bufLine[read..]);

			for (var x = 0; x < width * 4; x++)
			{
				switch (filter)
				{
                    case PredictionFilterType.Sub when x >= 4:
						bufLine[x] += bufLine[x - 4];
						break;
					case PredictionFilterType.Up:
						bufLine[x] += bufPrevLine[x];
						break;
					case PredictionFilterType.Average:
						{
							var l = x >= 4 ? bufLine[x - 4] : 0;
							var u = bufPrevLine[x];
							var p = (l + u) / 2;
							bufLine[x] = (byte)(bufLine[x] + p);
							break;
						}
					case PredictionFilterType.Paeth:
						{
							var a = x >= 4 ? bufLine[x - 4] : (byte)0;
							var b = bufPrevLine[x];
							var c = x >= 4 ? bufPrevLine[x - 4] : (byte)0;

							var p = a + b - c;
							var pa = Math.Abs(p - a);
							var pb = Math.Abs(p - b);
							var pc = Math.Abs(p - c);

							if (pa <= pb && pa <= pc)
							{
								bufLine[x] += a;
								continue;
							}

							if (pb <= pc)
							{
								bufLine[x] += b;
								continue;
							}

							bufLine[x] += c;
							break;
						}
                }
            }
			
			for (var x = 0; x < width; x++)
				pixels[x + (y * width)] = Color.FromArgb(bufLine[(x * 4) + 3], bufLine[(x * 4) + 0], bufPrevLine[(x * 4) + 1], bufPrevLine[(x * 4) + 2]);

			var temp = bufLine;
			bufLine = bufPrevLine;
			bufPrevLine = temp;
		}

		return true;
	}
}
