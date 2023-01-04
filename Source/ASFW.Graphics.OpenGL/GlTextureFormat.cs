using GLenum = System.UInt32;
using static ASFW.Graphics.OpenGL.Constants;

namespace ASFW.Graphics.OpenGL;

public enum GlTextureFormat : GLenum
{
	Red = GL_RED,
	Rg = GL_RG,
	Rgb = GL_RGB,
	Bgr = GL_BGR,
	Rgba = GL_RGBA,
	Bgra = GL_BGRA
}
