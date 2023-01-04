using GLenum = System.UInt32;
using static ASFW.Graphics.OpenGL.Constants;

namespace ASFW.Graphics.OpenGL;

public enum GlTextureType : GLenum
{
	UnsignedByte = GL_UNSIGNED_BYTE,
	Byte = GL_BYTE,
	UnsignedShort = GL_UNSIGNED_SHORT,
	Short = GL_SHORT,
	UnsignedInt = GL_UNSIGNED_INT,
	Int = GL_INT,
	Float = GL_FLOAT,
	UnsignedByte332 = GL_UNSIGNED_BYTE_3_3_2,
	UnsignedByte233Rev = GL_UNSIGNED_BYTE_2_3_3_REV,
	UnsignedShort565 = GL_UNSIGNED_SHORT_5_6_5,
	UnsignedShort565Rev = GL_UNSIGNED_SHORT_5_6_5_REV,
	UnsignedShort4444 = GL_UNSIGNED_SHORT_4_4_4_4,
	UnsignedShort4444Rev = GL_UNSIGNED_SHORT_4_4_4_4_REV,
	UnsignedShort5551 = GL_UNSIGNED_SHORT_5_5_5_1,
	UnsignedShort1555Rev = GL_UNSIGNED_SHORT_1_5_5_5_REV,
	UnsignedInt8888 = GL_UNSIGNED_INT_8_8_8_8,
	UnsignedInt8888Rev = GL_UNSIGNED_INT_8_8_8_8_REV,
	UnsignedInt1010102 = GL_UNSIGNED_INT_10_10_10_2,
	UnsignedInt2101010Rev = GL_UNSIGNED_INT_2_10_10_10_REV
}
