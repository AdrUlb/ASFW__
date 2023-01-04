namespace ASFW.Graphics.OpenGL.Abstractions;

public class GLTexture : IDisposable
{
	private readonly IGlProvider gl;
	private readonly uint id;
	private readonly GlTextureTarget target;

	public GLTexture(IGlProvider gl, GlTextureTarget target)
	{
		this.gl = gl;
		this.target = target;

		id = gl.GenTexture();
	}

	public void Bind(uint unit)
	{
		gl.ActiveTexture(unit);
		gl.BindTexture(target, id);
	}

	public void Unbind(uint unit)
	{
		gl.ActiveTexture(unit);
		gl.BindTexture(target, 0);
	}

	public void Dispose()
	{
		GC.SuppressFinalize(this);
		gl.DeleteTexture(id);
	}
}
