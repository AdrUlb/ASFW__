using GLenum = System.UInt32;
using static ASFW.Graphics.OpenGL.Constants;

namespace ASFW.Graphics.OpenGL;

public enum GlPolygonModes : GLenum
{
	Point = GL_POINT,
	Line = GL_LINE,
	Fill = GL_FILL
}
