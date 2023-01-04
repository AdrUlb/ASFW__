using GLbitfield = System.UInt32;
using static ASFW.Graphics.OpenGL.Constants;

namespace ASFW.Graphics.OpenGL;

[Flags]
public enum GlClearMask : GLbitfield
{
	ColorBufferBit = GL_COLOR_BUFFER_BIT,
	DepthBufferBit = GL_DEPTH_BUFFER_BIT,
	StencilBufferBit = GL_STENCIL_BUFFER_BIT
}
