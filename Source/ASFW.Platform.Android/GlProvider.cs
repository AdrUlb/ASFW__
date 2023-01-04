using Android.Opengl;
using ASFW.Graphics.OpenGL;
using Java.Nio;

namespace ASFW.Platform.Android;

public unsafe class GlProvider : IGlProvider
{
	private static readonly int[] buffer1i = new int[1];

	public string? GetString(GlStringName name) => GLES20.GlGetString((int)name);

	public string? GetString(GlStringName name, uint index) => GLES20.GlGetString((int)name); // Hack, GLES does not have this function

	public void ClearColor(float red, float green, float blue, float alpha) => GLES20.GlClearColor(red, green, blue, alpha);

	public void Clear(GlClearMask mask) => GLES20.GlClear((int)mask);

	public void Viewport(int x, int y, uint width, uint height) => GLES20.GlViewport(x, y, (int)width, (int)height);

	public void GenBuffers(Span<uint> buffers)
	{
		fixed (void* ptr = buffers)
			GLES20.GlGenBuffers(buffers.Length, new Span<int>(ptr, buffers.Length).ToArray(), 0);
	}

	public uint GenBuffer()
	{
		GLES20.GlGenBuffers(1, buffer1i, 0);
		return (uint)buffer1i[0];
	}

	public void DeleteBuffers(ReadOnlySpan<uint> buffers)
	{
		fixed (void* ptr = buffers)
			GLES20.GlDeleteBuffers(buffers.Length, new Span<int>(ptr, buffers.Length).ToArray(), 0);
	}

	public void DeleteBuffer(uint buffer)
	{
		buffer1i[0] = (int)buffer;
		GLES20.GlDeleteBuffers(1, buffer1i, 0);
	}

	public void BufferData<T>(GlBufferTarget target, ReadOnlySpan<T> data, GlBufferUsage usage) where T : unmanaged
	{
		var size = data.Length * sizeof(T);

		fixed (void* ptr = data)
		{
			using var dataBuffer = ByteBuffer.AllocateDirect(size);
			dataBuffer.Order(ByteOrder.NativeOrder()!);
			dataBuffer.Put(new Span<byte>(ptr, size).ToArray());
			dataBuffer.Position(0);
			GLES20.GlBufferData((int)target, size, dataBuffer, (int)usage);
		}
	}

	public void BufferData(GlBufferTarget target, nuint size, GlBufferUsage usage)
	{
		GLES20.GlBufferData((int)target, (int)size, null, (int)usage);
	}

	public void BufferSubData<T>(GlBufferTarget target, nint offset, nuint size, ReadOnlySpan<T> data) where T : unmanaged
	{
		fixed (void* ptr = data)
		{
			using var dataBuffer = ByteBuffer.AllocateDirect((int)size);
			dataBuffer.Order(ByteOrder.NativeOrder()!);
			dataBuffer.Put(new Span<byte>(ptr, (int)size).ToArray());
			dataBuffer.Position(0);
			GLES20.GlBufferSubData((int)target, (int)offset, (int)size, dataBuffer);
		}
	}

	public void BindBuffer(GlBufferTarget target, uint buffer) => GLES20.GlBindBuffer((int)target, (int)buffer);

	public void GenVertexArrays(Span<uint> arrays)
	{
		fixed (void* ptr = arrays)
			GLES30.GlGenVertexArrays(arrays.Length, new Span<int>(ptr, arrays.Length).ToArray(), 0);
	}

	public uint GenVertexArray()
	{
		GLES30.GlGenVertexArrays(1, buffer1i, 0);
		return (uint)buffer1i[0];
	}

	public void DeleteVertexArrays(ReadOnlySpan<uint> arrays)
	{
		fixed (void* ptr = arrays)
			GLES30.GlDeleteVertexArrays(arrays.Length, new Span<int>(ptr, arrays.Length).ToArray(), 0);
	}

	public void DeleteVertexArray(uint array)
	{
		buffer1i[0] = (int)array;
		GLES30.GlDeleteVertexArrays(1, buffer1i, 0);
	}

	public void BindVertexArray(uint array) => GLES30.GlBindVertexArray((int)array);

	public void VertexAttribPointer(uint index, int size, GlVertexAttribPointerType type, bool normalized, uint stride, nuint pointer) =>
		GLES20.GlVertexAttribPointer((int)index, size, (int)type, normalized, (int)stride, (int)pointer);

	public void EnableVertexAttribArray(uint index) => GLES20.GlEnableVertexAttribArray((int)index);

	public void DrawArrays(GlDrawMode mode, int first, uint count) => GLES20.GlDrawArrays((int)mode, first, (int)count);

	public uint CreateShader(GlShaderType shaderType) => (uint)GLES20.GlCreateShader((int)shaderType);

	public void DeleteShader(uint shader) => GLES20.GlDeleteShader((int)shader);

	public void ShaderSource(uint shader, string str) => GLES20.GlShaderSource((int)shader, str);

	public void CompileShader(uint shader) => GLES20.GlCompileShader((int)shader);

	public int GetShader(uint shader, GlShaderParameterName pname)
	{
		GLES20.GlGetShaderiv((int)shader, (int)pname, buffer1i, 0);
		return buffer1i[0];
	}

	public string GetShaderInfoLog(uint shader) => GLES20.GlGetShaderInfoLog((int)shader) ?? "Failed to retrieve shader info log.";

	public uint CreateProgram() => (uint)GLES20.GlCreateProgram();

	public void DeleteProgram(uint program) => GLES20.GlDeleteProgram((int)program);

	public void AttachShader(uint program, uint shader) => GLES20.GlAttachShader((int)program, (int)shader);

	public void DetachShader(uint program, uint shader) => GLES20.GlDetachShader((int)program, (int)shader);

	public void LinkProgram(uint program) => GLES20.GlLinkProgram((int)program);

	public int GetProgram(uint program, GlProgramParameterName pname)
	{
		GLES20.GlGetProgramiv((int)program, (int)pname, buffer1i, 0);
		return buffer1i[0];
	}

	public string GetProgramInfoLog(uint program) => GLES20.GlGetProgramInfoLog((int)program) ?? "Failed to retrieve info log.";

	public void UseProgram(uint program) => GLES20.GlUseProgram((int)program);

	public void DrawElements(GlDrawMode mode, uint count, GlIndexType type, nuint indices) => GLES20.GlDrawElements((int)mode, (int)count, (int)type, (int)indices);

	public int GetUniformLocation(uint program, string name) => GLES20.GlGetUniformLocation((int)program, name);

	public void Uniform1f(int location, float v0) => GLES20.GlUniform1f(location, v0);

	public void Uniform2f(int location, float v0, float v1) => GLES20.GlUniform2f(location, v0, v1);

	public void Uniform3f(int location, float v0, float v1, float v2) => GLES20.GlUniform3f(location, v0, v1, v2);

	public void Uniform4f(int location, float v0, float v1, float v2, float v3) => GLES20.GlUniform4f(location, v0, v1, v2, v3);

	public void Uniform1i(int location, int v0) => GLES20.GlUniform1i(location, v0);

	public void Uniform2i(int location, int v0, int v1) => GLES20.GlUniform2i(location, v0, v1);

	public void Uniform3i(int location, int v0, int v1, int v2)
	{
		throw new NotImplementedException();
	}

	public void Uniform4i(int location, int v0, int v1, int v2, int v3)
	{
		throw new NotImplementedException();
	}

	public void Uniform1ui(int location, uint v0)
	{
		throw new NotImplementedException();
	}

	public void Uniform2ui(int location, uint v0, uint v1)
	{
		throw new NotImplementedException();
	}

	public void Uniform3ui(int location, uint v0, uint v1, uint v2)
	{
		throw new NotImplementedException();
	}

	public void Uniform4ui(int location, uint v0, uint v1, uint v2, uint v3)
	{
		throw new NotImplementedException();
	}

	public void UniformMatrix2fv(int location, uint count, bool transpose, float* value)
	{
		throw new NotImplementedException();
	}

	public void UniformMatrix3fv(int location, uint count, bool transpose, float* value)
	{
		throw new NotImplementedException();
	}

	public void UniformMatrix4fv(int location, uint count, bool transpose, float* value)
	{
		const int floatsPerMat = 4 * 4;
		var floatCount = floatsPerMat * (int)count;
		var buffer = new float[floatCount];
		for (var i = 0; i < floatCount; i++)
			buffer[i] = value[i];
		GLES20.GlUniformMatrix4fv(location, (int)count, transpose, buffer, 0);
	}

	public void UniformMatrix2x3fv(int location, uint count, bool transpose, float* value)
	{
		throw new NotImplementedException();
	}

	public void UniformMatrix3x2fv(int location, uint count, bool transpose, float* value)
	{
		throw new NotImplementedException();
	}

	public void UniformMatrix2x4fv(int location, uint count, bool transpose, float* value)
	{
		throw new NotImplementedException();
	}

	public void UniformMatrix4x2fv(int location, uint count, bool transpose, float* value)
	{
		throw new NotImplementedException();
	}

	public void UniformMatrix3x4fv(int location, uint count, bool transpose, float* value)
	{
		throw new NotImplementedException();
	}

	public void UniformMatrix4x3fv(int location, uint count, bool transpose, float* value)
	{
		throw new NotImplementedException();
	}

	public void PolygonMode(GlPolygonFace face, GlPolygonModes mode)
	{
		// Does not exist
	}

	public void GenTextures(Span<uint> textures)
	{
		fixed (void* ptr = textures)
			GLES20.GlGenTextures(textures.Length, new Span<int>(ptr, textures.Length).ToArray(), 0);
	}

	public uint GenTexture()
	{
		GLES20.GlGenTextures(1, buffer1i, 0);
		return (uint)buffer1i[0];
	}

	public void BindTexture(GlTextureTarget target, uint texture) => GLES20.GlBindTexture((int)target, (int)texture);

	public void TexParameterf(GlTextureTarget target, GlTextureParameterName pname, float param) => GLES20.GlTexParameterf((int)target, (int)pname, param);

	public void TexParameteri(GlTextureTarget target, GlTextureParameterName pname, int param) => GLES20.GlTexParameteri((int)target, (int)pname, param);

	public void TexParameter(GlTextureTarget target, GlTextureMinFilter textureMinFilter) =>
		GLES20.GlTexParameteri((int)target, GLES20.GlTextureMinFilter, (int)textureMinFilter);

	public void TexParameter(GlTextureTarget target, GlTextureMagFilter textureMagFilter) =>
		GLES20.GlTexParameteri((int)target, GLES20.GlTextureMagFilter, (int)textureMagFilter);

	public void ActiveTexture(uint unit) => GLES20.GlActiveTexture((int)unit);

	public void TexImage2D<T>(GlTextureTarget target, int level, GlTextureInternalFormat internalFormat, uint width, uint height, int border, GlTextureFormat format,
		GlTextureType type,
		ReadOnlySpan<T> data) where T : unmanaged
	{
		var size = data.Length * sizeof(T);
		using var buffer = ByteBuffer.AllocateDirect(size * 10);
		buffer.Order(ByteOrder.NativeOrder()!);

		fixed (void* ptr = data)
			buffer.Put(new Span<byte>(ptr, size).ToArray());
		GLES20.GlTexImage2D((int)target, level, (int)internalFormat, (int)width, (int)height, border, (int)format, (int)type, data.Length == 0 ? null : buffer);
	}

	public void TexSubImage2D<T>(GlTextureTarget target, int level, int xoffset, int yoffset, uint width, uint height, GlTextureFormat format, GlTextureType type,
		ReadOnlySpan<T> data) where T : unmanaged
	{
		var size = data.Length * sizeof(T);
		using var buffer = ByteBuffer.AllocateDirect(size * 10);
		buffer.Order(ByteOrder.NativeOrder()!);
		fixed (void* ptr = data)
			buffer.Put(new Span<byte>(ptr, size).ToArray());
		buffer.Position(0);
		GLES20.GlTexSubImage2D((int)target, level, xoffset, yoffset, (int)width, (int)height, (int)format, (int)type, data.Length == 0 ? null : buffer);
	}

	public void DeleteTextures(ReadOnlySpan<uint> textures)
	{
		fixed (void* ptr = textures)
			GLES20.GlDeleteTextures(textures.Length, new Span<int>(ptr, textures.Length).ToArray(), 0);
	}

	public void DeleteTexture(uint texture)
	{
		buffer1i[0] = (int)texture;
		GLES20.GlDeleteTextures(1, buffer1i, 0);
	}

	public void GenerateMipmap(GlTextureTarget target) => GLES20.GlGenerateMipmap((int)target);

	public void Enable(GlCapability cap) => GLES20.GlEnable((int)cap);

	public void Disable(GlCapability cap) => GLES20.GlDisable((int)cap);

	public void BlendFunc(GlBlendFactor sfactor, GlBlendFactor dfactor) => GLES20.GlBlendFunc((int)sfactor, (int)dfactor);

	public void TexImage3D<T>(GlTextureTarget target, int level, GlTextureInternalFormat internalFormat, uint width, uint height, uint depth, int border, GlTextureFormat format,
		GlTextureType type, ReadOnlySpan<T> data) where T : unmanaged
	{
		throw new NotImplementedException();
	}

	public void TexSubImage3D<T>(GlTextureTarget target, int level, int xoffset, int yoffset, int zoffset, uint width, uint height, uint depth, GlTextureFormat format,
		GlTextureType type,
		ReadOnlySpan<T> data) where T : unmanaged
	{
		throw new NotImplementedException();
	}

	public int GetAttribLocation(uint program, string name) => GLES20.GlGetAttribLocation((int)program, name);
}
