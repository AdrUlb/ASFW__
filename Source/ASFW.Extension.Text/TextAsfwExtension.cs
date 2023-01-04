using ASFW.Extension.Text.FreeType;

namespace ASFW.Extension.Text;

public class TextAsfwExtension : IAsfwExtension
{
	internal static FTLibrary FtLibrary { get; private set; }
	
	private static bool init = false;
	
	internal static void AssertInit()
	{
		if (!init)
			throw new("Assertion failed: ASFW Text extension was not initialized.");
	}

	protected override void OnInit()
	{
		if (FT.InitFreeType(out var ftLibrary) != FTError.Ok)
			throw new("Failed to initialize FreeType.");
		FtLibrary = ftLibrary;
		
		init = true;
	}

	protected override void OnQuit()
	{
		init = false;
		FT.DoneFreeType(FtLibrary);
	}
}
