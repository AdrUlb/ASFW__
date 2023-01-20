using System.Runtime.InteropServices;
using ASFW.Audio.PlayerBackends.Winmm;
using ASFW.Audio.Sources;
using static ASFW.Audio.PlayerBackends.Winmm.Imports;

namespace ASFW.Audio.PlayerBackends;

internal sealed unsafe class WinmmAudioPlayerBackendBackend : AudioPlayerBackend
{
	public override string BackendName => "Windows MultiMedia API (waveOut)";
	
	private readonly WaveFormatEx waveOutFormat;
	private readonly IntPtr hWaveOut;
	// ReSharper disable once NotAccessedField.Local
	private readonly Imports.WaveOutProc waveOutProc;

	private readonly byte* wavebuf;
	private readonly WaveHdr* wavehdr;
	private const uint blockCount = 3;
	private readonly uint blockSize;

	private uint currentBlock = 0;
	private uint blocksQueued = 0;
	private bool writeBlocks = false;
	private readonly object writeLock = new();

	public override IAudioSource Source { get; }
	public bool Playing => blocksQueued != 0;

	public WinmmAudioPlayerBackendBackend(IAudioSource source)
	{
		Source = source;

		waveOutFormat.FormatTag = WAVE_FORMAT_PCM;
		waveOutFormat.Channels = source.Channels;
		waveOutFormat.SamplesPerSec = source.SampleRate;
		waveOutFormat.AvgBytesPerSec = source.BytesPerSecond;
		waveOutFormat.BlockAlign = source.BlockAlign;
		waveOutFormat.BitsPerSample = source.BitsPerSample;
		waveOutFormat.Size = (ushort)sizeof(WaveFormatEx);

		if (WaveOutOpen(out hWaveOut, uint.MaxValue, in waveOutFormat, waveOutProc = WaveOutCallback, 0, CALLBACK_FUNCTION) != 0)
			throw new("Failed to open wave out device.");

		blockSize = (uint)(waveOutFormat.BitsPerSample / 8 * waveOutFormat.SamplesPerSec * waveOutFormat.Channels / 50);

		wavebuf = (byte*)Marshal.AllocHGlobal((nint)(blockSize * blockCount));
		wavehdr = (WaveHdr*)Marshal.AllocHGlobal((nint)(sizeof(WaveHdr) * blockCount));

		// Zero out the wave headers
		for (var i = 0; i < sizeof(WaveHdr) * blockCount; i++)
			((byte*)wavehdr)[i] = 0;

		// Prepare the wave headers (the documentation explicitly allows changing dwBufferLength to a smaller value before calling waveOutWrite)
		for (uint i = 0; i < blockCount; i++)
		{
			wavehdr[i].lpData = (nuint)wavebuf + (i * blockSize);
			wavehdr[i].dwBufferLength = blockSize;
			if (WaveOutPrepareHeader(hWaveOut, wavehdr + i, (uint)sizeof(WaveHdr)) != 0)
				throw new("Failed to prepare wave out header.");
		}
	}
	
	private void WriteNextBlock()
	{
		lock (writeLock)
		{
			if (!writeBlocks)
				return;

			var block = currentBlock++;
			currentBlock %= blockCount;

			var blockSpan = new Span<byte>(wavebuf + (block * blockSize), (int)blockSize);
			var readSize = Source.GetNextBlock(blockSpan);

			if (readSize <= 0)
				return;

			wavehdr[block].dwBufferLength = (uint)readSize;

			blocksQueued++;
			if (WaveOutWrite(hWaveOut, wavehdr + block, (uint)sizeof(WaveHdr)) != 0)
				throw new("Failed to write to wave out device.");
		}
	}

	public override void Play()
	{
		if (Playing)
			return;

		writeBlocks = true;

		for (var i = 0; i < blockCount; i++)
			WriteNextBlock();
	}

	public override void Stop()
	{
		if (!Playing)
			return;

		writeBlocks = false;
		SpinWait.SpinUntil(() => !Playing);
	}

	private void WaveOutCallback(IntPtr hwo, WaveOutMessage uMsg, IntPtr dwInstance, IntPtr dwParam1, IntPtr dwParam2)
	{
		if (uMsg != WaveOutMessage.Done)
			return;
		
		blocksQueued--;
		WriteNextBlock();
	}

	~WinmmAudioPlayerBackendBackend() => Dispose();
	
	public override void Dispose()
	{
		GC.SuppressFinalize(this);
		Stop();
		if (WaveOutClose(hWaveOut) != 0)
			throw new("Failed to close wave out.");

		for (var i = 0; i < blockCount; i++)
			_ = WaveOutUnprepareHeader(hWaveOut, wavehdr + i, (uint)sizeof(WaveHdr));

		Marshal.FreeHGlobal((nint)wavehdr);
		Marshal.FreeHGlobal((nint)wavebuf);	
	}
}
