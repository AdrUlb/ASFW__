namespace ASFW.Graphics.OpenGL.Abstractions;

public class GLVertexArray : IDisposable
{
	private readonly IGlProvider gl;
	private readonly uint id;

	public GLVertexArray(IGlProvider gl)
	{
		this.gl = gl;
		id = gl.GenVertexArray();
	}

	public void Bind() => gl.BindVertexArray(id);
	public void Unbind() => gl.BindVertexArray(0);

	public void VertexAttribPointer(uint index, int size, GlVertexAttribPointerType type, bool normalized, uint stride, nuint pointer, GLBuffer arrayBuffer)
	{
		Bind();
		arrayBuffer.Bind(GlBufferTarget.ArrayBuffer);
		gl.VertexAttribPointer(index, size, type, normalized, stride, pointer);
		Unbind();
	}

	public void EnableVertexAttribArray(uint index)
	{
		Bind();
		gl.EnableVertexAttribArray(index);
		Unbind();
	}

	public void DrawArrays(GlDrawMode mode, int first, uint count)
	{
		Bind();
		gl.DrawArrays(mode, first, count);
		Unbind();
	}

	public void DrawElements(GlDrawMode mode, uint count, GlIndexType type, nuint indices, GLBuffer ebo)
	{
		Bind();
		ebo.Bind(GlBufferTarget.ElementArrayBuffer);
		gl.DrawElements(mode, count, type, indices);
	}

	public void Dispose()
	{
		Unbind();
		GC.SuppressFinalize(this);
		gl.DeleteVertexArray(id);
	}
}
