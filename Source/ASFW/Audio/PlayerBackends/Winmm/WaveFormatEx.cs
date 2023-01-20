using System.Runtime.InteropServices;

namespace ASFW.Audio.PlayerBackends.Winmm;

[StructLayout(LayoutKind.Sequential)]
internal struct WaveFormatEx
{
	public ushort FormatTag;
	public ushort Channels;
	public uint SamplesPerSec;
	public uint AvgBytesPerSec;
	public ushort BlockAlign;
	public ushort BitsPerSample;
	public ushort Size;
}
