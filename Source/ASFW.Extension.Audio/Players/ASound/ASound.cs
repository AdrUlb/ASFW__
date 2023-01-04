using System.Runtime.InteropServices;

namespace ASFW.Extension.Audio.Players.ASound;

internal unsafe static partial class Imports
{
	private static partial class Native
	{
		[LibraryImport("asound", EntryPoint = "snd_pcm_open", StringMarshalling = StringMarshalling.Utf8)]
		public static partial int SndPcmOpen(out IntPtr pcm, string name, SndPcmStream stream, int mode);

		[LibraryImport("asound", EntryPoint = "snd_pcm_hw_params_any")]
		public static partial int SndPcmHwParamsAny(IntPtr pcm, byte* @params);
		
		[LibraryImport("asound", EntryPoint = "snd_pcm_hw_params_set_access")]
		public static partial int SndPcmHwParamsSetAccess(IntPtr pcm, byte* @params, SndPcmAccess access);

		[LibraryImport("asound", EntryPoint = "snd_pcm_hw_params_set_format")]
		public static partial int SndPcmHwParamsSetFormat(IntPtr pcm, byte* @params, SndPcmFormat val);

		[LibraryImport("asound", EntryPoint = "snd_pcm_hw_params_set_channels")]
		public static partial int SndPcmHwParamsSetChannels(IntPtr pcm, byte* @params, uint val);

		[LibraryImport("asound", EntryPoint = "snd_pcm_hw_params_set_rate_near")]
		public static partial int SndPcmHwParamsSetRateNear(IntPtr pcm, byte* @params, ref uint val, ref int dir);
		
		[LibraryImport("asound", EntryPoint = "snd_pcm_hw_params_set_rate_near")]
		public static partial int SndPcmHwParamsSetRateNear(IntPtr pcm, byte* @params, ref uint val, int* dir);
		
		[LibraryImport("asound", EntryPoint = "snd_pcm_hw_params_set_periods")]
		public static partial int SndPcmHwParamsSetPeriods(IntPtr pcm, byte* @params, uint val, int dir);
		
		[LibraryImport("asound", EntryPoint = "snd_pcm_hw_params_set_buffer_size")]
		public static partial int SndPcmHwParamsSetBufferSize(IntPtr pcm, byte* @params, ulong val);
		
		[LibraryImport("asound", EntryPoint = "snd_pcm_hw_params")]
		public static partial int SndPcmHwParams(IntPtr pcm, byte* @params);

		[LibraryImport("asound", EntryPoint = "snd_pcm_hw_params_get_period_size")]
		public static partial int SndPcmHwParamsGetPeriodSize(byte* @params, ref ulong val, ref int dir);

		[LibraryImport("asound", EntryPoint = "snd_pcm_hw_params_get_period_size")]
		public static partial int SndPcmHwParamsGetPeriodSize(byte* @params, ref ulong val, int* dir);
		
		[LibraryImport("asound", EntryPoint = "snd_pcm_hw_params_get_period_time")]
		public static partial int SndPcmHwParamsGetPeriodTime(byte* @params, ref uint val, ref int dir);

		[LibraryImport("asound", EntryPoint = "snd_pcm_hw_params_get_period_time")]
		public static partial int SndPcmHwParamsGetPeriodTime(byte* @params, ref uint val, int* dir);

		[LibraryImport("asound", EntryPoint = "snd_pcm_writei")]
		public static partial long SndPcmWriteI(IntPtr pcm, void* buffer, ulong size);

		[LibraryImport("asound", EntryPoint = "snd_pcm_prepare")]
		public static partial int SndPcmPrepare(IntPtr pcm);

		[LibraryImport("asound", EntryPoint = "snd_pcm_drain")]
		public static partial int SndPcmDrain(IntPtr pcm);

		[LibraryImport("asound", EntryPoint = "snd_pcm_close")]
		public static partial int SndPcmClose(IntPtr pcm);

		[LibraryImport("asound", EntryPoint = "snd_pcm_hw_params_sizeof")]
		public static partial nint SndPcmHwParamsSizeof();
	}

	public static int SndPcmOpen(out IntPtr pcm, string name, SndPcmStream stream, int mode) => Native.SndPcmOpen(out pcm, name, stream, mode);

	public static int SndPcmHwParamsAny(IntPtr pcm, byte* @params) => Native.SndPcmHwParamsAny(pcm, @params);
	public static int SndPcmHwParamsSetAccess(IntPtr pcm, byte* @params, SndPcmAccess access) => Native.SndPcmHwParamsSetAccess(pcm, @params, access);
	public static int SndPcmHwParamsSetFormat(IntPtr pcm, byte* @params, SndPcmFormat val) => Native.SndPcmHwParamsSetFormat(pcm, @params, val);
	public static int SndPcmHwParamsSetChannels(IntPtr pcm, byte* @params, uint val) => Native.SndPcmHwParamsSetChannels(pcm, @params, val);

	public static int SndPcmHwParamsSetRateNear(IntPtr pcm, byte* @params, ref uint val, ref int dir) =>
		Native.SndPcmHwParamsSetRateNear(pcm, @params, ref val, ref dir);

	public static int SndPcmHwParamsSetRateNear(IntPtr pcm, byte* @params, ref uint val, int* dir) =>
		Native.SndPcmHwParamsSetRateNear(pcm, @params, ref val, dir);

	public static int SndPcmHwParams(IntPtr pcm, byte* @params) => Native.SndPcmHwParams(pcm, @params);
	public static int SndPcmHwParamsGetPeriodSize(byte* @params, ref ulong val, ref int dir) => Native.SndPcmHwParamsGetPeriodSize(@params, ref val, ref dir);
	public static int SndPcmHwParamsGetPeriodSize(byte* @params, ref ulong val, int* dir) => Native.SndPcmHwParamsGetPeriodSize(@params, ref val, dir);
	public static int SndPcmHwParamsGetPeriodTime(byte* @params, ref uint val, ref int dir) => Native.SndPcmHwParamsGetPeriodTime(@params, ref val, ref dir);
	public static int SndPcmHwParamsGetPeriodTime(byte* @params, ref uint val, int* dir) => Native.SndPcmHwParamsGetPeriodTime(@params, ref val, dir);
	public static long SndPcmWriteI(IntPtr pcm, void* buffer, ulong size) => Native.SndPcmWriteI(pcm, buffer, size);
	public static int SndPcmPrepare(IntPtr pcm) => Native.SndPcmPrepare(pcm);
	public static int SndPcmDrain(IntPtr pcm) => Native.SndPcmDrain(pcm);
	public static int SndPcmClose(IntPtr pcm) => Native.SndPcmClose(pcm);
	public static nint SndPcmHwParamsSizeof() => Native.SndPcmHwParamsSizeof();

	public static int SndPcmHwParamsSetPeriods(IntPtr pcm, byte* @params, uint val, int dir) => Native.SndPcmHwParamsSetPeriods(pcm, @params, val, dir);

	public static int SndPcmHwParamsSetBufferSize(IntPtr pcm, byte* @params, ulong val) => Native.SndPcmHwParamsSetBufferSize(pcm, @params, val);
}
