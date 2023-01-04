using System.Numerics;

namespace ASFW.Graphics.OpenGL.Abstractions;

public class GLProgram : IDisposable
{
	private readonly IGlProvider gl;
	internal readonly uint id;

	private readonly Dictionary<string, int> uniformLocations = new();
	private readonly Dictionary<string, int> attribLocations = new();

	public GLProgram(IGlProvider gl, params GLShader[] shaderObjects)
	{
		this.gl = gl;

		id = gl.CreateProgram();
		foreach (var shaderObject in shaderObjects)
			gl.AttachShader(id, shaderObject.Id);
		gl.LinkProgram(id);
		if (gl.GetProgram(id, GlProgramParameterName.LinkStatus) == 0)
			throw new($"Failed to link shader program: {gl.GetProgramInfoLog(id)}.");

		foreach (var shaderObject in shaderObjects)
			gl.DetachShader(id, shaderObject.Id);
	}

	private int GetUniformLocation(string name)
	{
		if (uniformLocations.TryGetValue(name, out var loc))
			return loc;

		loc = gl.GetUniformLocation(id, name);
		uniformLocations.Add(name, loc);
		return loc;
	}

	public int GetAttribLocation(string name)
	{
		if (attribLocations.TryGetValue(name, out var loc))
			return loc;
		
		loc = gl.GetAttribLocation(id, name);
		attribLocations.Add(name, loc);
		return loc;
	}

	public void Use()
	{
		gl.UseProgram(id);
	}

	public void Uniform1i(string name, int v0)
	{
		gl.Uniform1i(GetUniformLocation(name), v0);
	}

	public void Uniform4f(string name, float v0, float v1, float v2, float v3)
	{
		gl.Uniform4f(GetUniformLocation(name), v0, v1, v2, v3);
	}

	public unsafe void UniformMatrix4fv(string name, bool transpose, in Matrix4x4 value)
	{
		fixed (Matrix4x4* valuePtr = &value)
			gl.UniformMatrix4fv(GetUniformLocation(name), 1, transpose, (float*)valuePtr);
	}

	public void Dispose()
	{
		GC.SuppressFinalize(this);
		gl.DeleteProgram(id);
	}
}
