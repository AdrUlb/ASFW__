using System.Runtime.InteropServices;
using ASFW.Extension.Audio.Players.ASound;
using ASFW.Extension.Audio.Sources;
using static ASFW.Extension.Audio.Players.ASound.Imports;

namespace ASFW.Extension.Audio.Players;

public unsafe class AlsaAudioPlayer : AudioPlayer
{
	public override string BackendName => "Advanced Linux Sound Architecture (ALSA)";

	private readonly nint pcm;
	private readonly byte* buffer;
	private readonly int bufferSize;

	private uint blocksQueued = 0;
	private bool writeBlocks = false;

	public override IAudioSource Source { get; }

	public bool Playing => blocksQueued != 0;

	public AlsaAudioPlayer(IAudioSource source)
	{
		Source = source;

		if (SndPcmOpen(out pcm, "default", SndPcmStream.Playback, 0) < 0)
			throw new("Failed to open PCM device.");

		var hwParamsSizeof = (int)SndPcmHwParamsSizeof();
		var hwParams = stackalloc byte[hwParamsSizeof];
		for (var i = 0; i < hwParamsSizeof; i++)
			hwParams[i] = 0;

		var rate = source.SampleRate;

		bufferSize = (int)(source.BitsPerSample / 8 * source.Channels * rate) / 200;

		SndPcmHwParamsAny(pcm, hwParams);
		SndPcmHwParamsSetAccess(pcm, hwParams, SndPcmAccess.RwInterleaved);
		SndPcmHwParamsSetFormat(pcm, hwParams, SndPcmFormat.Signed16LittleEndian);
		SndPcmHwParamsSetChannels(pcm, hwParams, 2);
		SndPcmHwParamsSetRateNear(pcm, hwParams, ref rate, null);
		SndPcmHwParamsSetPeriods(pcm, hwParams, 2, 0);
		SndPcmHwParamsSetBufferSize(pcm, hwParams, (ulong)bufferSize);

		SndPcmHwParams(pcm, hwParams);

		ulong periodSize = 0;
		SndPcmHwParamsGetPeriodSize(hwParams, ref periodSize, null);

		buffer = (byte*)Marshal.AllocHGlobal(bufferSize);
	}

	private void WriteNextBlock()
	{
		const long EPIPE = 32;

		blocksQueued++;
		if (!writeBlocks)
			return;

		var readSize = Source.GetNextBlock(new(buffer, bufferSize));

		if (readSize <= 0)
			return;

		var frames = (ulong)(readSize / Source.Channels / 2);
		while (SndPcmWriteI(pcm, buffer, frames) == -EPIPE)
			SndPcmPrepare(pcm);
	}

	public override void Play()
	{
		if (Playing)
			return;

		writeBlocks = true;

		new Thread(() =>
		{
			while (writeBlocks)
			{
				WriteNextBlock();
				blocksQueued--;
			}
		}).Start();
	}

	public override void Stop()
	{
		if (!Playing)
			return;

		writeBlocks = false;
		SpinWait.SpinUntil(() => !Playing);
	}

	~AlsaAudioPlayer() => Dispose();

	public override void Dispose()
	{
		GC.SuppressFinalize(this);
		Stop();
		Marshal.FreeHGlobal((nint)buffer);
		SndPcmDrain(pcm);
		SndPcmClose(pcm);
	}
}
