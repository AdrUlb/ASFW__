using GLint = System.UInt32;
using static ASFW.Graphics.OpenGL.Constants;

namespace ASFW.Graphics.OpenGL;

public enum GlTextureMagFilter : GLint
{
	Nearest = GL_NEAREST,
	Linear = GL_LINEAR,
}
