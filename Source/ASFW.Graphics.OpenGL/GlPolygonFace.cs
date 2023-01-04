using GLenum = System.UInt32;
using static ASFW.Graphics.OpenGL.Constants;

namespace ASFW.Graphics.OpenGL;

public enum GlPolygonFace : GLenum
{
	Front = GL_FRONT,
	Back = GL_BACK,
	FrontAndBack = GL_FRONT_AND_BACK
}
