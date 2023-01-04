using GLenum = System.UInt32;
using static ASFW.Graphics.OpenGL.Constants;

namespace ASFW.Graphics.OpenGL;

public enum GlTextureParameterName : GLenum
{
	BaseLevel = GL_TEXTURE_BASE_LEVEL,
	CompareFunc = GL_TEXTURE_COMPARE_FUNC,
	CompareMode = GL_TEXTURE_COMPARE_MODE,
	LodBias = GL_TEXTURE_LOD_BIAS,
	MinFilter = GL_TEXTURE_MIN_FILTER,
	MagFilter = GL_TEXTURE_MAG_FILTER,
	MinLod = GL_TEXTURE_MIN_LOD,
	MaxLod = GL_TEXTURE_MAX_LOD,
	MaxLevel = GL_TEXTURE_MAX_LEVEL,
	SwizzleR = GL_TEXTURE_SWIZZLE_R,
	SwizzleG = GL_TEXTURE_SWIZZLE_G,
	SwizzleB = GL_TEXTURE_SWIZZLE_B,
	SwizzleA = GL_TEXTURE_SWIZZLE_A,
	WrapS = GL_TEXTURE_WRAP_S,
	WrapT = GL_TEXTURE_WRAP_T,
	WrapR = GL_TEXTURE_WRAP_R
}
