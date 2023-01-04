namespace ASFW.Graphics.OpenGL.Abstractions;

public class GLShader : IDisposable
{
	private readonly IGlProvider gl;
	internal readonly uint Id;

	public GLShader(IGlProvider gl, GlShaderType type, string source)
	{
		this.gl = gl;

		Id = gl.CreateShader(type);
		gl.ShaderSource(Id, source);
		gl.CompileShader(Id);
		if (gl.GetShader(Id, GlShaderParameterName.CompileStatus) == 0)
			throw new($"Failed to compile {type switch
			{
				GlShaderType.VertexShader => "vertex shader",
				GlShaderType.FragmentShader => "fragment shader",
				GlShaderType.GeometryShader => "geometry shader",
				_ => "shader of unknown type"
			}}:\n{source}\n{gl.GetShaderInfoLog(Id)}.");
	}

	public void Dispose()
	{
		GC.SuppressFinalize(this);
		gl.DeleteShader(Id);
	}
}
