using System.Diagnostics.CodeAnalysis;
using ASFW.Extension.Audio.Sources;

namespace ASFW.Extension.Audio.Players;

public abstract class AudioPlayer : IDisposable
{
	public abstract string BackendName { get; }

	public abstract IAudioSource Source { get; }

	public static bool TryOpen(Stream stream, [MaybeNullWhen(false)] out AudioPlayer result)
	{
		IAudioSource? source = null;

		if (WavPcmAudioSource.TryOpen(stream, out var wavPcmSource))
			source = wavPcmSource;

		if (source == null)
		{
			result = null;
			return false;
		}

#if ANDROID
		result = new AndroidAudioPlayer(source);
		return true;
#else
		if (OperatingSystem.IsWindows())
			result = new WinmmAudioPlayer(source);
		else if (OperatingSystem.IsLinux())
			result = new AlsaAudioPlayer(source);
		else
			result = null;
		return result != null;
#endif
	}

	public abstract void Play();
	public abstract void Stop();

	public abstract void Dispose();
}
