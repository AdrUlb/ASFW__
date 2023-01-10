#if ANDROID
using Android.Media;
using ASFW.Extension.Audio.Sources;

namespace ASFW.Extension.Audio.Players;

internal sealed class AndroidAudioPlayer : AudioPlayer
{
	private readonly AudioTrack track;

	private bool writeBlocks = false;

	public override string BackendName => "Android AudioTrack";

	public override IAudioSource Source { get; }

	public bool Playing => track.PlayState == PlayState.Playing;

	private const int blockCount = 2;

	private int currentBlock = 0;
	private readonly int blockSize;
	private readonly byte[][] blocks;
	private readonly TimeSpan blockLength;

	private readonly Queue<int> blockQueue = new();

	public AndroidAudioPlayer(IAudioSource source)
	{
		Source = source;

		var channelOut = source.Channels switch
		{
			1 => ChannelOut.Mono,
			2 => ChannelOut.Stereo,
			_ => throw new NotSupportedException()
		};

		blockSize = AudioTrack.GetMinBufferSize((int)source.SampleRate, channelOut, Encoding.Pcm16bit);
		var bytesPerSample = source.BitsPerSample / (double)8 * source.Channels;
		var samplesPerBuffer = blockSize / bytesPerSample;
		var secondsPerBuffer = samplesPerBuffer / source.SampleRate;
		blockLength = TimeSpan.FromSeconds(secondsPerBuffer * 0.95);

		blocks = new byte[blockCount][];

		for (var i = 0; i < blocks.Length; i++)
			blocks[i] = new byte[blockSize];

		track = new AudioTrack.Builder()
			.SetAudioAttributes(new AudioAttributes.Builder()
				.SetUsage(AudioUsageKind.Media)!
				.SetContentType(AudioContentType.Music)!
				.Build()!)
			.SetAudioFormat(new AudioFormat.Builder()
				.SetEncoding(Encoding.Pcm16bit)!
				.SetSampleRate((int)source.SampleRate)!
				.SetChannelMask(channelOut)
				.Build()!)
			.SetBufferSizeInBytes(blockSize)
			.Build();
	}

	private bool PrepareNextBlock()
	{
		if (!writeBlocks)
			return false;

		var block = currentBlock++;
		currentBlock %= blockCount;

		var readSize = Source.GetNextBlock(blocks[block]);

		blockQueue.Enqueue(block);

		return readSize > 0;
	}

	private bool WriteNextBlock()
	{
		if (!writeBlocks)
			return false;

		track.Write(blocks[blockQueue.Dequeue()], 0, blockSize);

		return true;
	}

	public override void Play()
	{
		if (Playing)
			return;

		writeBlocks = true;

		new Thread(() =>
		{
			for (var i = 0; i < blockCount; i++)
				if (!PrepareNextBlock())
					break;

			if (!WriteNextBlock())
				return;

			track.Play();

			while (true)
			{
				if (!WriteNextBlock())
					break;
				if (!PrepareNextBlock())
					break;
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

	public override void Dispose()
	{
		GC.SuppressFinalize(this);
		Stop();
		track.Dispose();
	}
}
#endif
