namespace ASFW.Graphics.OpenGL.Abstractions;

public class GLBuffer : IDisposable
{
	private readonly IGlProvider gl;
	private readonly uint id;

	public GLBuffer(IGlProvider gl)
	{
		this.gl = gl;

		id = gl.GenBuffer();
	}

	public void Bind(GlBufferTarget target)
	{
		gl.BindBuffer(target, id);
	}

	public void Dispose()
	{
		GC.SuppressFinalize(this);
		gl.DeleteBuffer(id);
	}
}
