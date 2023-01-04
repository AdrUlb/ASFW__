using GLenum = System.UInt32;
using static ASFW.Graphics.OpenGL.Constants;

namespace ASFW.Graphics.OpenGL;

public enum GlShaderType : GLenum
{
	VertexShader = GL_VERTEX_SHADER,
	GeometryShader = GL_GEOMETRY_SHADER,
	FragmentShader = GL_FRAGMENT_SHADER
}
