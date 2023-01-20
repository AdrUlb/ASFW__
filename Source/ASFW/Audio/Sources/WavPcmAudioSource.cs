using System.Buffers.Binary;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;

namespace ASFW.Audio.Sources;

internal class WavPcmAudioSource : IAudioSource
{
	public string Name => "PCM in RIFF/WAVE container";
	
	private enum Format : ushort
	{
		PCM = 1
	}

	[StructLayout(LayoutKind.Sequential, Pack = 1)]
	private readonly struct FmtChunk
	{
		public readonly Format Format;
		public readonly ushort Channels;
		public readonly uint SampleRate;
		public readonly uint BytesPerSecond;
		public readonly ushort BlockAlign;
		public readonly ushort BitsPerSample;
	}

	private readonly Stream stream;
	private readonly FmtChunk fmt;
	private readonly uint length;
	private uint position;

	public ushort Channels => fmt.Channels;
	public uint SampleRate => fmt.SampleRate;
	public uint BytesPerSecond => fmt.BytesPerSecond;
	public ushort BlockAlign => fmt.BlockAlign;
	public ushort BitsPerSample => fmt.BitsPerSample;

	public float Volume { get; set; }

	private WavPcmAudioSource(Stream stream, uint length, FmtChunk fmt)
	{
		this.stream = stream;
		this.length = length;
		this.fmt = fmt;
	}

	public unsafe static bool TryOpen(Stream stream, [MaybeNullWhen(false)] out WavPcmAudioSource result)
	{
		result = null;

		var riffHeader = "RIFF"u8;
		var waveHeader = "WAVE"u8;
		var waveFmtChunkHeader = "fmt "u8;
		var waveDataChunkHeader = "data"u8;

		Span<byte> buf4 = stackalloc byte[4];

		stream.ReadExactly(buf4);
		if (!buf4.SequenceEqual(riffHeader))
			return false;

		stream.ReadExactly(buf4);
		var riffSize = BinaryPrimitives.ReadUInt32LittleEndian(buf4);
		var riffOffset = stream.Position;

		if (stream.Length < riffSize + riffOffset)
			return false;

		stream.ReadExactly(buf4);
		if (!buf4.SequenceEqual(waveHeader))
			return false;

		var fmt = new FmtChunk();
		uint length = 0;

		while (stream.Position < riffSize + riffOffset)
		{
			stream.ReadExactly(buf4);

			if (buf4.SequenceEqual(waveFmtChunkHeader))
			{
				stream.ReadExactly(buf4);
				stream.ReadExactly(new Span<byte>(&fmt, sizeof(FmtChunk)));

				if (fmt.Format != Format.PCM)
					return false;
			}
			else if (buf4.SequenceEqual(waveDataChunkHeader))
			{
				stream.ReadExactly(buf4);
				length = BinaryPrimitives.ReadUInt32LittleEndian(buf4);
				break;
			}
			else
			{
				stream.ReadExactly(buf4);
				var chunkSize = BinaryPrimitives.ReadUInt32LittleEndian(buf4);
				stream.Seek(chunkSize, SeekOrigin.Current);
			}
		}

		result = new(stream, length, fmt);
		return true;
	}

	private readonly object readLock = new();
	
	public int GetNextBlock(Span<byte> buffer, bool rewind = false)
	{
		lock (readLock)
		{
			var bytesToRead = Math.Min(buffer.Length, length - position);
			var bufferSub = buffer[..(int)bytesToRead];
			var bytesRead = stream.Read(bufferSub);

			for (var i = 0; i < bytesRead / 2; i++)
			{
				var lsb = buffer[i * 2];
				var msb = buffer[i * 2 + 1];

				var mul = Volume * Volume;
				var s = (short)((msb << 8) | lsb);
				s = (short)(s * mul);

				lsb = (byte)(s & 0xFF);
				msb = (byte)(s >> 8);

				buffer[i * 2] = lsb;
				buffer[i * 2 + 1] = msb;
			}

			if (rewind)
				stream.Seek(-bytesRead, SeekOrigin.Current);
			else
				position += (uint)bytesRead;

			return bytesRead;
		}
	}
}
