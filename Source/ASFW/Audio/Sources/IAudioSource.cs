namespace ASFW.Audio.Sources;
public interface IAudioSource
{
	public string Name { get; }
	
	public ushort Channels { get; }
	public uint SampleRate { get; }
	public uint BytesPerSecond { get; }
	public ushort BlockAlign { get; }
	public ushort BitsPerSample { get; }

	public float Volume { get; set; }

	public int GetNextBlock(Span<byte> buffer, bool rewind = false);
}
