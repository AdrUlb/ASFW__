using System.Reflection;
using System.Runtime.InteropServices;

namespace ASFW.Extension.Text.FreeType;

internal unsafe static partial class FT
{
	private static partial class Imports
	{
		[LibraryImport("freetype", EntryPoint = "FT_Init_FreeType")]
		public static partial FTError Init_FreeType(out FTLibrary alibrary);

		[LibraryImport("freetype", EntryPoint = "FT_Done_FreeType")]
		public static partial FTError Done_FreeType(FTLibrary library);

		[LibraryImport("freetype", EntryPoint = "FT_New_Face", StringMarshalling = StringMarshalling.Utf8)]
		public static partial FTError New_Face(FTLibrary library, string filepathname, long face_index, out FTFaceRec* aface);

		[LibraryImport("freetype", EntryPoint = "FT_New_Memory_Face")]
		public static partial FTError New_Memory_Face(FTLibrary library, byte* file_base, long file_size, long face_index, out FTFaceRec* aface);

		[LibraryImport("freetype", EntryPoint = "FT_Done_Face")]
		public static partial FTError Done_Face(FTFaceRec* face);

		[LibraryImport("freetype", EntryPoint = "FT_Load_Char")]
		public static partial FTError Load_Char(FTFaceRec* face, ulong char_code, FTLoadFlags load_flags);

		[LibraryImport("freetype", EntryPoint = "FT_Set_Pixel_Sizes")]
		public static partial FTError Set_Pixel_Sizes(FTFaceRec* face, uint pixel_width, uint pixel_height);

		[LibraryImport("freetype", EntryPoint = "FT_Set_Char_Size")]
		public static partial FTError Set_Char_Size(FTFaceRec* face, long char_width, long char_height, uint horz_resolution, uint vert_resolution);
	}

	static FT()
	{
		nint freetype = 0;
		var l = new object();
		NativeLibrary.SetDllImportResolver(Assembly.GetExecutingAssembly(), (name, _, _) =>
		{
			if (name != "freetype")
				return 0;

			lock (l)
			{
				if (freetype != 0)
					return freetype;

				var ridOs = OperatingSystem.IsLinux() ? "linux"
					: OperatingSystem.IsWindows() ? "win"
					: throw new PlatformNotSupportedException("Operating system not supported.");

				var ridPlatform = RuntimeInformation.OSArchitecture switch
				{
					Architecture.X64 => "x64",
					_ => throw new PlatformNotSupportedException($"System architecture not supported ({RuntimeInformation.OSArchitecture}).")
				};

				var rid = $"{ridOs}-{ridPlatform}";

				name = ridOs switch
				{
					"linux" => "libfreetype.so",
					"win" => "freetype.dll",
					_ => throw new NotImplementedException()
				};

				var path = Path.Combine(AppContext.BaseDirectory, "NativeLibs", rid, name);

				if (!File.Exists(path))
					return 0;


				return freetype = NativeLibrary.Load(path);
			}
		});
		AppDomain.CurrentDomain.ProcessExit += (_, _) => NativeLibrary.Free(freetype);
	}

	public static FTError InitFreeType(out FTLibrary alibrary) => Imports.Init_FreeType(out alibrary);
	public static FTError DoneFreeType(FTLibrary library) => Imports.Done_FreeType(library);
	public static FTError NewFace(FTLibrary library, string filepathname, long face_index, out FTFaceRec* aface) => Imports.New_Face(library, filepathname, face_index, out aface);

	public static FTError NewMemoryFace(FTLibrary library, byte* file_base, long file_size, long face_index, out FTFaceRec* aface) =>
		Imports.New_Memory_Face(library, file_base, file_size, face_index, out aface);

	public static FTError DoneFace(FTFaceRec* face) => Imports.Done_Face(face);
	public static FTError LoadChar(FTFaceRec* face, ulong char_code, FTLoadFlags load_flags) => Imports.Load_Char(face, char_code, load_flags);
	public static FTError SetPixelSizes(FTFaceRec* face, uint pixel_width, uint pixel_height) => Imports.Set_Pixel_Sizes(face, pixel_width, pixel_height);

	public static FTError SetCharSize(FTFaceRec* face, long char_width, long char_height, uint horz_resolution, uint vert_resolution) =>
		Imports.Set_Char_Size(face, char_width, char_height, horz_resolution, vert_resolution);
}
