namespace ASFW.Extension.Audio.Players.ASound;

internal enum SndPcmAccess
{
	
	MMapInterleaved = 0,
	MMapNonInterleaved,
	MMapComplex,
	RwInterleaved,
	RwNonInterleaved,
	LAST = RwNonInterleaved
}
