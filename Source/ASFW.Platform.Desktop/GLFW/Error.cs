namespace ASFW.Platform.Desktop.GLFW;

internal enum Error
{
	NoError = 0,
	NotInitialized = 0x00010001,
	NoCurrentContext = 0x00010002,
	InvalidEnum = 0x00010003,
	InvalidValue = 0x00010004,
	OutOfMemory = 0x00010005,
	ApiUnavailable = 0x00010006,
	VersionUnavailable = 0x00010007,
	PlatformError = 0x00010008,
	FormatUnavailable = 0x00010009,
	NoWindowContext = 0x0001000A,
	CursorUnavailable = 0x0001000B,
	FeatureUnavailable = 0x0001000C,
	FeatureUnimplemented = 0x0001000D,
	PlatformUnavailable = 0x0001000E
}
