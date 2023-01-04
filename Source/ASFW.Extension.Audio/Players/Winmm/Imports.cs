using System.Runtime.InteropServices;

namespace ASFW.Extension.Audio.Players.Winmm;

internal unsafe static partial class Imports
{
	public const uint CALLBACK_FUNCTION = 0x00030000;
	public const ushort WAVE_FORMAT_PCM = 1;

	public delegate void WaveOutProc(IntPtr hwo, WaveOutMessage uMsg, IntPtr dwInstance, IntPtr dwParam1, IntPtr dwParam2);

	private static partial class Native
	{
		[LibraryImport("winmm", EntryPoint = "waveOutOpen")]
		public static partial uint WaveOutOpen(out IntPtr hWaveOut, uint uDeviceID, in WaveFormatEx lpFormat, WaveOutProc dwCallback, IntPtr dwInstance, uint fdwOpen);

		[LibraryImport("winmm", EntryPoint = "waveOutClose")]
		public static partial uint WaveOutClose(IntPtr hWaveOut);

		[LibraryImport("winmm", EntryPoint = "waveOutPrepareHeader")]
		public static partial uint WaveOutPrepareHeader(IntPtr hwo, WaveHdr* pwh, uint cbwh);

		[LibraryImport("winmm", EntryPoint = "waveOutUnprepareHeader")]
		public static partial uint WaveOutUnprepareHeader(IntPtr hwo, WaveHdr* pwh, uint cbwh);

		[LibraryImport("winmm", EntryPoint = "waveOutWrite")]
		public static partial uint WaveOutWrite(IntPtr hwo, WaveHdr* pwh, uint cbwh);
	}

	public static uint WaveOutOpen(out IntPtr hWaveOut, uint uDeviceID, in WaveFormatEx lpFormat, WaveOutProc dwCallback, IntPtr dwInstance, uint fdwOpen) => Native.WaveOutOpen(out hWaveOut, uDeviceID, lpFormat, dwCallback, dwInstance, fdwOpen);
	public static uint WaveOutClose(IntPtr hWaveOut) => Native.WaveOutClose(hWaveOut);
	public static uint WaveOutPrepareHeader(IntPtr hwo, WaveHdr* pwh, uint cbwh) => Native.WaveOutPrepareHeader(hwo, pwh, cbwh);
	public static uint WaveOutUnprepareHeader(IntPtr hwo, WaveHdr* pwh, uint cbwh) => Native.WaveOutUnprepareHeader(hwo, pwh, cbwh);
	public static uint WaveOutWrite(IntPtr hwo, WaveHdr* pwh, uint cbwh) => Native.WaveOutWrite(hwo, pwh, cbwh);
}
