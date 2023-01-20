using System.Diagnostics.CodeAnalysis;
using ASFW.Audio.Sources;

namespace ASFW.Audio.PlayerBackends;

public abstract class AudioPlayerBackend : IDisposable
{
	public abstract string BackendName { get; }

	public abstract IAudioSource Source { get; }

	public static bool TryCreate(Stream stream, [MaybeNullWhen(false)] out AudioPlayerBackend result)
	{
		IAudioSource? source = null;

		if (WavPcmAudioSource.TryOpen(stream, out var wavPcmSource))
			source = wavPcmSource;

		if (source != null)
			return TryCreate(source, out result);
		
		result = null;
		return false;
	}
	
	public static bool TryCreate(IAudioSource source, [MaybeNullWhen(false)] out AudioPlayerBackend result)
	{
#if ANDROID
		result = new AndroidAudioPlayerBackend(source);
		return true;
#else
		if (OperatingSystem.IsWindows())
			result = new WinmmAudioPlayerBackendBackend(source);
		else if (OperatingSystem.IsLinux())
			result = new AlsaAudioPlayerBackend(source);
		else
			result = null;
		return result != null;
#endif
	}

	public abstract void Play();
	public abstract void Stop();

	public abstract void Dispose();
}
