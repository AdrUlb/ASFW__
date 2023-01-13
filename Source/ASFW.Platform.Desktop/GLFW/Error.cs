namespace ASFW.Platform.Desktop.GLFW;

internal enum Error
{
	/// <summary>
	/// No error has occurred.
	/// </summary>
	NoError = 0,
	/// <summary>
	/// GLFW has not been initialized.
	/// </summary>
	NotInitialized = 0x00010001,
	/// <summary>
	/// No context is current for this thread.
	/// </summary>
	NoCurrentContext = 0x00010002,
	/// <summary>
	/// One of the arguments to the function was an invalid enum value.
	/// </summary>
	InvalidEnum = 0x00010003,
	/// <summary>
	/// One of the arguments to the function was an invalid value.
	/// </summary>
	InvalidValue = 0x00010004,
	/// <summary>
	/// A memory allocation failed.
	/// </summary>
	OutOfMemory = 0x00010005,
	/// <summary>
	/// GLFW could not find support for the requested API on the system.
	/// </summary>
	ApiUnavailable = 0x00010006,
	/// <summary>
	/// The requested OpenGL or OpenGL ES version is not available.
	/// </summary>
	VersionUnavailable = 0x00010007,
	/// <summary>
	/// A platform-specific error occurred that does not match any of the more specific categories.
	/// </summary>
	PlatformError = 0x00010008,
	/// <summary>
	/// The requested format is not supported or available.
	/// </summary>
	FormatUnavailable = 0x00010009,
	/// <summary>
	/// The specified window does not have an OpenGL or OpenGL ES context.
	/// </summary>
	NoWindowContext = 0x0001000A,
	/// <summary>
	/// The specified cursor shape is not available.
	/// </summary>
	CursorUnavailable = 0x0001000B,
	/// <summary>
	/// The requested feature is not provided by the platform.
	/// </summary>
	FeatureUnavailable = 0x0001000C,
	/// <summary>
	/// The requested feature is not implemented for the platform.
	/// </summary>
	FeatureUnimplemented = 0x0001000D,
	/// <summary>
	/// Platform unavailable or no matching platform was found.
	/// </summary>
	PlatformUnavailable = 0x0001000E
}
