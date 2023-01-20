namespace ASFW.Audio.PlayerBackends.ASound;

internal enum SndPcmAccess
{
	
	MMapInterleaved = 0,
	MMapNonInterleaved,
	MMapComplex,
	RwInterleaved,
	RwNonInterleaved,
	LAST = RwNonInterleaved
}
