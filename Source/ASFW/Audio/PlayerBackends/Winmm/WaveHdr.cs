using System.Runtime.InteropServices;

namespace ASFW.Audio.PlayerBackends.Winmm;


[StructLayout(LayoutKind.Sequential)]
public struct WaveHdr
{
	public nuint lpData;
	public uint dwBufferLength;
	public uint dwBytesRecorded;
	public IntPtr dwUser;
	public uint dwFlags;
	public uint dwLoops;
	private readonly IntPtr lpNext;
	private readonly IntPtr reserved;
}
