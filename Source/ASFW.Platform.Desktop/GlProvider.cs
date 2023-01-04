using System.Runtime.InteropServices;
using System.Text;
using ASFW.Graphics.OpenGL;
using ASFW.Platform.Desktop.GLFW;
using GLboolean = System.Boolean;
using GLchar = System.Byte;
using GLenum = System.UInt32;
using GLfloat = System.Single;
using GLint = System.Int32;
using GLintptr = System.IntPtr;
using GLsizei = System.UInt32;
using GLsizeiptr = System.UIntPtr;
using GLuint = System.UInt32;
using static ASFW.Graphics.OpenGL.Constants;

namespace ASFW.Platform.Desktop;

public sealed unsafe class GlProvider : IGlProvider
{
	public delegate IntPtr GetProcAddress(string procname);

	private delegate IntPtr ProcGetString(GlStringName name);
	private delegate IntPtr ProcGetStringi(GlStringName name, GLuint index);
	private delegate void ProcClearColor(GLfloat red, GLfloat green, GLfloat blue, GLfloat alpha);
	private delegate void ProcClear(GlClearMask mask);
	private delegate void ProcViewport(GLint x, GLint y, GLsizei width, GLsizei height);
	private delegate void ProcGenBuffers(GLsizei n, GLuint* buffers);
	private delegate void ProcDeleteBuffers(GLsizei n, GLuint* buffers);
	private delegate void ProcBufferData(GlBufferTarget target, GLsizeiptr size, void* data, GlBufferUsage usage);
	private delegate void ProcBufferSubData(GlBufferTarget target, GLintptr offset, GLsizeiptr size, void* data);
	private delegate void ProcBindBuffer(GlBufferTarget target, GLuint buffer);
	private delegate void ProcGenVertexArrays(GLsizei n, GLuint* arrays);
	private delegate void ProcDeleteVertexArrays(GLsizei n, GLuint* arrays);
	private delegate void ProcBindVertexArray(GLuint array);
	private delegate void ProcVertexAttribPointer(GLuint index, GLint size, GlVertexAttribPointerType type, GLboolean normalized, GLsizei stride, void* pointer);
	private delegate void ProcEnableVertexAttribArray(GLuint index);
	private delegate void ProcDrawArrays(GlDrawMode mode, GLint first, GLsizei count);
	private delegate void ProcGetShaderiv(GLuint shader, GlShaderParameterName pname, GLint* param);
	private delegate GLuint ProcCreateShader(GlShaderType shaderType);
	private delegate void ProcDeleteShader(GLuint shader);
	private delegate void ProcShaderSource(GLuint shader, GLsizei count, GLchar** str, GLint* length);
	private delegate void ProcCompileShader(GLuint shader);
	private delegate void ProcGetShaderInfoLog(GLuint shader, GLsizei maxLength, GLsizei* length, GLchar* infoLog);
	private delegate GLuint ProcCreateProgram();
	private delegate void ProcDeleteProgram(GLuint program);
	private delegate void ProcAttachShader(GLuint program, GLuint shader);
	private delegate void ProcDetachShader(GLuint program, GLuint shader);
	private delegate void ProcLinkProgram(GLuint program);
	private delegate void ProcGetProgramiv(GLuint program, GlProgramParameterName pname, GLint* param);
	private delegate void ProcGetProgramInfoLog(GLuint program, GLsizei maxLength, GLsizei* length, GLchar* infoLog);
	private delegate void ProcUseProgram(GLuint program);
	private delegate void ProcDrawElements(GlDrawMode mode, GLsizei count, GlIndexType type, void* indices);
	private delegate GLint ProcGetUniformLocation(GLuint program, [MarshalAs(UnmanagedType.LPStr)] string name);
	private delegate void ProcUniform1f(GLint location, GLfloat v0);
	private delegate void ProcUniform2f(GLint location, GLfloat v0, GLfloat v1);
	private delegate void ProcUniform3f(GLint location, GLfloat v0, GLfloat v1, GLfloat v2);
	private delegate void ProcUniform4f(GLint location, GLfloat v0, GLfloat v1, GLfloat v2, GLfloat v3);
	private delegate void ProcUniform1i(GLint location, GLint v0);
	private delegate void ProcUniform2i(GLint location, GLint v0, GLint v1);
	private delegate void ProcUniform3i(GLint location, GLint v0, GLint v1, GLint v2);
	private delegate void ProcUniform4i(GLint location, GLint v0, GLint v1, GLint v2, GLint v3);
	private delegate void ProcUniform1ui(GLint location, GLuint v0);
	private delegate void ProcUniform2ui(GLint location, GLuint v0, GLuint v1);
	private delegate void ProcUniform3ui(GLint location, GLuint v0, GLuint v1, GLuint v2);
	private delegate void ProcUniform4ui(GLint location, GLuint v0, GLuint v1, GLuint v2, GLuint v3);
	private delegate void ProcUniformMatrix2fv(GLint location, GLsizei count, GLboolean transpose, GLfloat* value);
	private delegate void ProcUniformMatrix3fv(GLint location, GLsizei count, GLboolean transpose, GLfloat* value);
	private delegate void ProcUniformMatrix4fv(GLint location, GLsizei count, GLboolean transpose, GLfloat* value);
	private delegate void ProcUniformMatrix2x3fv(GLint location, GLsizei count, GLboolean transpose, GLfloat* value);
	private delegate void ProcUniformMatrix3x2fv(GLint location, GLsizei count, GLboolean transpose, GLfloat* value);
	private delegate void ProcUniformMatrix2x4fv(GLint location, GLsizei count, GLboolean transpose, GLfloat* value);
	private delegate void ProcUniformMatrix4x2fv(GLint location, GLsizei count, GLboolean transpose, GLfloat* value);
	private delegate void ProcUniformMatrix3x4fv(GLint location, GLsizei count, GLboolean transpose, GLfloat* value);
	private delegate void ProcUniformMatrix4x3fv(GLint location, GLsizei count, GLboolean transpose, GLfloat* value);
	private delegate void ProcPolygonMode(GlPolygonFace face, GlPolygonModes mode);
	private delegate void ProcGenTextures(GLsizei n, GLuint* textures);
	private delegate void ProcBindTexture(GlTextureTarget target, GLuint texture);
	private delegate void ProcTexParameterf(GlTextureTarget target, GlTextureParameterName pname, GLfloat param);
	private delegate void ProcTexParameteri(GlTextureTarget target, GlTextureParameterName pname, GLint param);
	private delegate void ProcActiveTexture(GLenum texture);

	private delegate void ProcTexImage2D(GlTextureTarget target, GLint level, GlTextureInternalFormat internalFormat, GLsizei width, GLsizei height, GLint border,
		GlTextureFormat format, GlTextureType type, void* data);

	private delegate void ProcTexSubImage2D(GlTextureTarget target, GLint level, GLint xoffset, GLint yoffset, GLsizei width, GLsizei height, GlTextureFormat format,
		GlTextureType type, void* data);

	private delegate void ProcDeleteTextures(GLsizei n, GLuint* textures);
	private delegate void ProcGenerateMipmap(GlTextureTarget target);
	private delegate void ProcEnable(GlCapability cap);
	private delegate void ProcDisable(GlCapability cap);
	private delegate void ProcBlendFunc(GlBlendFactor sfactor, GlBlendFactor dfactor);

	private delegate void ProcTexImage3D(GlTextureTarget target, GLint level, GlTextureInternalFormat internalFormat, GLsizei width, GLsizei height, GLsizei depth, GLint border,
		GlTextureFormat format, GlTextureType type, void* data);

	private delegate void ProcTexSubImage3D(GlTextureTarget target, GLint level, GLint xoffset, GLint yoffset, GLint zoffset, GLsizei width, GLsizei height, GLsizei depth,
		GlTextureFormat format, GlTextureType type, void* data);

	private delegate GLint ProcGetAttribLocation(GLuint program, [MarshalAs(UnmanagedType.LPStr)] string name);

	private readonly Dictionary<string, Delegate> procs = new();

	private readonly object getProcLock = new();

	private T GetProc<T>(string procname) where T : Delegate
	{
		lock (getProcLock)
		{
			if (procs.TryGetValue(procname, out var value))
				return (T)value;

			value = Marshal.GetDelegateForFunctionPointer<T>(Glfw.GetProcAddress(procname));
			procs.Add(procname, value);

			return (T)value;
		}
	}

	public string GetString(GlStringName name) => Marshal.PtrToStringAnsi(GetProc<ProcGetString>("glGetString")(name)) ?? "";
	public string GetString(GlStringName name, GLuint index) => Marshal.PtrToStringAnsi(GetProc<ProcGetStringi>("glGetStringi")(name, index)) ?? "";
	public void ClearColor(GLfloat red, GLfloat green, GLfloat blue, GLfloat alpha) => GetProc<ProcClearColor>("glClearColor")(red, green, blue, alpha);
	public void Clear(GlClearMask mask) => GetProc<ProcClear>("glClear")(mask);
	public void Viewport(GLint x, GLint y, GLsizei width, GLsizei height) => GetProc<ProcViewport>("glViewport")(x, y, width, height);

	public void GenBuffers(Span<GLuint> buffers)
	{
		fixed (GLuint* buffersPtr = buffers)
			GetProc<ProcGenBuffers>("glGenBuffers")((GLsizei)buffers.Length, buffersPtr);
	}

	public GLuint GenBuffer()
	{
		GLuint buffer;
		GetProc<ProcGenBuffers>("glGenBuffers")(1, &buffer);
		return buffer;
	}

	public void DeleteBuffers(ReadOnlySpan<GLuint> buffers)
	{
		fixed (GLuint* buffersPtr = buffers)
			GetProc<ProcDeleteBuffers>("glDeleteBuffers")((GLsizei)buffers.Length, buffersPtr);
	}

	public void DeleteBuffer(GLuint buffer) => GetProc<ProcDeleteBuffers>("glDeleteBuffers")(1, &buffer);

	public void BufferData<T>(GlBufferTarget target, ReadOnlySpan<T> data, GlBufferUsage usage) where T : unmanaged
	{
		fixed (T* dataPtr = data)
			GetProc<ProcBufferData>("glBufferData")(target, (nuint)sizeof(T) * (nuint)data.Length, dataPtr, usage);
	}

	public void BufferData(GlBufferTarget target, GLsizeiptr size, GlBufferUsage usage) => GetProc<ProcBufferData>("glBufferData")(target, size, null, usage);

	public void BufferSubData<T>(GlBufferTarget target, nint offset, nuint size, ReadOnlySpan<T> data) where T : unmanaged
	{
		fixed (T* dataPtr = data)
			GetProc<ProcBufferSubData>("glBufferSubData")(target, offset, size, dataPtr);
	}

	public void BindBuffer(GlBufferTarget target, GLuint buffer) => GetProc<ProcBindBuffer>("glBindBuffer")(target, buffer);

	public void GenVertexArrays(Span<GLuint> arrays)
	{
		fixed (GLuint* arraysPtr = arrays)
			GetProc<ProcGenVertexArrays>("glGenVertexArrays")((GLsizei)arrays.Length, arraysPtr);
	}

	public GLuint GenVertexArray()
	{
		GLuint array;
		GetProc<ProcGenVertexArrays>("glGenVertexArrays")(1, &array);
		return array;
	}

	public void DeleteVertexArrays(ReadOnlySpan<GLuint> arrays)
	{
		fixed (GLuint* arraysPtr = arrays)
			GetProc<ProcDeleteVertexArrays>("glDeleteVertexArrays")((GLsizei)arrays.Length, arraysPtr);
	}

	public void DeleteVertexArray(GLuint array) => GetProc<ProcDeleteVertexArrays>("glDeleteVertexArrays")(1, &array);
	public void BindVertexArray(GLuint arrays) => GetProc<ProcBindVertexArray>("glBindVertexArray")(arrays);

	public void VertexAttribPointer(GLuint index, GLint size, GlVertexAttribPointerType type, GLboolean normalized, GLsizei stride, nuint pointer) =>
		GetProc<ProcVertexAttribPointer>("glVertexAttribPointer")(index, size, type, normalized, stride, (void*)pointer);

	public void EnableVertexAttribArray(GLuint index) => GetProc<ProcEnableVertexAttribArray>("glEnableVertexAttribArray")(index);
	public void DrawArrays(GlDrawMode mode, GLint first, GLsizei count) => GetProc<ProcDrawArrays>("glDrawArrays")(mode, first, count);

	public GLuint CreateShader(GlShaderType shaderType) => GetProc<ProcCreateShader>("glCreateShader")(shaderType);

	public void DeleteShader(GLuint shader) => GetProc<ProcDeleteShader>("glDeleteShader")(shader);

	public void ShaderSource(GLuint shader, string str)
	{
		Span<GLchar> strBytes = stackalloc GLchar[str.Length];
		Encoding.ASCII.GetBytes(str, strBytes);
		var length = str.Length;
		fixed (GLchar* strPtr = strBytes)
			GetProc<ProcShaderSource>("glShaderSource")(shader, 1, &strPtr, &length);
	}

	public void CompileShader(GLuint shader) => GetProc<ProcCompileShader>("glCompileShader")(shader);

	public GLint GetShader(GLuint shader, GlShaderParameterName pname)
	{
		GLint param;
		GetProc<ProcGetShaderiv>("glGetShaderiv")(shader, pname, &param);
		return param;
	}

	public string GetShaderInfoLog(GLuint shader)
	{
		var maxLength = GetShader(shader, GlShaderParameterName.InfoLogLength);
		Span<GLchar> infoLog = stackalloc GLchar[maxLength];
		GLsizei length;
		fixed (GLchar* infoLogPtr = infoLog)
			GetProc<ProcGetShaderInfoLog>("glGetShaderInfoLog")(shader, (uint)maxLength, &length, infoLogPtr);
		return Encoding.ASCII.GetString(infoLog);
	}

	public GLuint CreateProgram() => GetProc<ProcCreateProgram>("glCreateProgram")();
	public void DeleteProgram(GLuint program) => GetProc<ProcDeleteProgram>("glDeleteProgram")(program);
	public void AttachShader(GLuint program, GLuint shader) => GetProc<ProcAttachShader>("glAttachShader")(program, shader);
	public void DetachShader(GLuint program, GLuint shader) => GetProc<ProcDetachShader>("glDetachShader")(program, shader);
	public void LinkProgram(GLuint program) => GetProc<ProcLinkProgram>("glLinkProgram")(program);

	public GLint GetProgram(GLuint program, GlProgramParameterName pname)
	{
		GLint param;
		GetProc<ProcGetProgramiv>("glGetProgramiv")(program, pname, &param);
		return param;
	}

	public string GetProgramInfoLog(GLuint program)
	{
		var maxLength = GetProgram(program, GlProgramParameterName.InfoLogLength);
		Span<GLchar> infoLog = stackalloc GLchar[maxLength];
		GLsizei length;
		fixed (GLchar* infoLogPtr = infoLog)
			GetProc<ProcGetProgramInfoLog>("glGetProgramInfoLog")(program, (uint)maxLength, &length, infoLogPtr);
		return Encoding.ASCII.GetString(infoLog);
	}

	public void UseProgram(GLuint program) => GetProc<ProcUseProgram>("glUseProgram")(program);

	public void DrawElements(GlDrawMode mode, GLsizei count, GlIndexType type, nuint indices) =>
		GetProc<ProcDrawElements>("glDrawElements")(mode, count, type, (void*)indices);

	public GLint GetUniformLocation(GLuint program, string name) => GetProc<ProcGetUniformLocation>("glGetUniformLocation")(program, name);

	public void Uniform1f(GLint location, GLfloat v0) => GetProc<ProcUniform1f>("glUniform1f")(location, v0);
	public void Uniform2f(GLint location, GLfloat v0, GLfloat v1) => GetProc<ProcUniform2f>("glUniform2f")(location, v0, v1);
	public void Uniform3f(GLint location, GLfloat v0, GLfloat v1, GLfloat v2) => GetProc<ProcUniform3f>("glUniform3f")(location, v0, v1, v2);
	public void Uniform4f(GLint location, GLfloat v0, GLfloat v1, GLfloat v2, GLfloat v3) => GetProc<ProcUniform4f>("glUniform4f")(location, v0, v1, v2, v3);
	public void Uniform1i(GLint location, GLint v0) => GetProc<ProcUniform1i>("glUniform1i")(location, v0);
	public void Uniform2i(GLint location, GLint v0, GLint v1) => GetProc<ProcUniform2i>("glUniform2i")(location, v0, v1);
	public void Uniform3i(GLint location, GLint v0, GLint v1, GLint v2) => GetProc<ProcUniform3i>("glUniform3i")(location, v0, v1, v2);
	public void Uniform4i(GLint location, GLint v0, GLint v1, GLint v2, GLint v3) => GetProc<ProcUniform4i>("glUniform4i")(location, v0, v1, v2, v3);
	public void Uniform1ui(GLint location, GLuint v0) => GetProc<ProcUniform1ui>("glUniform1ui")(location, v0);
	public void Uniform2ui(GLint location, GLuint v0, GLuint v1) => GetProc<ProcUniform2ui>("glUniform2ui")(location, v0, v1);
	public void Uniform3ui(GLint location, GLuint v0, GLuint v1, GLuint v2) => GetProc<ProcUniform3ui>("glUniform3ui")(location, v0, v1, v2);
	public void Uniform4ui(GLint location, GLuint v0, GLuint v1, GLuint v2, GLuint v3) => GetProc<ProcUniform4ui>("glUniform4ui")(location, v0, v1, v2, v3);

	public void UniformMatrix2fv(GLint location, GLsizei count, GLboolean transpose, GLfloat* value) =>
		GetProc<ProcUniformMatrix2fv>("glUniformMatrix2fv")(location, count, transpose, value);

	public void UniformMatrix3fv(GLint location, GLsizei count, GLboolean transpose, GLfloat* value) =>
		GetProc<ProcUniformMatrix3fv>("glUniformMatrix3fv")(location, count, transpose, value);

	public void UniformMatrix4fv(GLint location, GLsizei count, GLboolean transpose, GLfloat* value) =>
		GetProc<ProcUniformMatrix4fv>("glUniformMatrix4fv")(location, count, transpose, value);

	public void UniformMatrix2x3fv(GLint location, GLsizei count, GLboolean transpose, GLfloat* value) =>
		GetProc<ProcUniformMatrix2x3fv>("glUniformMatrix2x3fv")(location, count, transpose, value);

	public void UniformMatrix3x2fv(GLint location, GLsizei count, GLboolean transpose, GLfloat* value) =>
		GetProc<ProcUniformMatrix3x2fv>("glUniformMatrix3x2fv")(location, count, transpose, value);

	public void UniformMatrix2x4fv(GLint location, GLsizei count, GLboolean transpose, GLfloat* value) =>
		GetProc<ProcUniformMatrix2x4fv>("glUniformMatrix2x4fv")(location, count, transpose, value);

	public void UniformMatrix4x2fv(GLint location, GLsizei count, GLboolean transpose, GLfloat* value) =>
		GetProc<ProcUniformMatrix4x2fv>("glUniformMatrix4x2fv")(location, count, transpose, value);

	public void UniformMatrix3x4fv(GLint location, GLsizei count, GLboolean transpose, GLfloat* value) =>
		GetProc<ProcUniformMatrix3x4fv>("glUniformMatrix3x4fv")(location, count, transpose, value);

	public void UniformMatrix4x3fv(GLint location, GLsizei count, GLboolean transpose, GLfloat* value) =>
		GetProc<ProcUniformMatrix4x3fv>("glUniformMatrix4x3fv")(location, count, transpose, value);

	public void PolygonMode(GlPolygonFace face, GlPolygonModes mode) => GetProc<ProcPolygonMode>("glPolygonMode")(face, mode);

	public void GenTextures(Span<GLuint> textures)
	{
		fixed (GLuint* texturesPtr = textures)
			GetProc<ProcGenTextures>("glGenTextures")((GLsizei)textures.Length, texturesPtr);
	}

	public GLuint GenTexture()
	{
		GLuint texture;
		GetProc<ProcGenTextures>("glGenTextures")(1, &texture);
		return texture;
	}

	public void BindTexture(GlTextureTarget target, GLuint texture) => GetProc<ProcBindTexture>("glBindTexture")(target, texture);
	public void TexParameterf(GlTextureTarget target, GlTextureParameterName pname, GLfloat param) => GetProc<ProcTexParameterf>("glTexParameterf")(target, pname, param);
	public void TexParameteri(GlTextureTarget target, GlTextureParameterName pname, GLint param) => GetProc<ProcTexParameteri>("glTexParameteri")(target, pname, param);

	public void TexParameter(GlTextureTarget target, GlTextureMinFilter textureMinFilter) => TexParameteri(target, GlTextureParameterName.MinFilter, (GLint)textureMinFilter);
	public void TexParameter(GlTextureTarget target, GlTextureMagFilter textureMagFilter) => TexParameteri(target, GlTextureParameterName.MagFilter, (GLint)textureMagFilter);

	public void ActiveTexture(uint unit) => GetProc<ProcActiveTexture>("glActiveTexture")(GL_TEXTURE0 + unit);

	public void TexImage2D<T>(GlTextureTarget target, GLint level, GlTextureInternalFormat internalFormat, GLsizei width, GLsizei height, GLint border, GlTextureFormat format,
		GlTextureType type, ReadOnlySpan<T> data) where T : unmanaged
	{
		fixed (void* dataPtr = data)
			GetProc<ProcTexImage2D>("glTexImage2D")(target, level, internalFormat, width, height, border, format, type, dataPtr);
	}

	public void TexSubImage2D<T>(GlTextureTarget target, GLint level, GLint xoffset, GLint yoffset, GLsizei width, GLsizei height, GlTextureFormat format, GlTextureType type,
		ReadOnlySpan<T> data) where T : unmanaged
	{
		fixed (void* dataPtr = data)
			GetProc<ProcTexSubImage2D>("glTexSubImage2D")(target, level, xoffset, yoffset, width, height, format, type, dataPtr);
	}

	public void DeleteTextures(ReadOnlySpan<uint> textures)
	{
		fixed (uint* texturesPtr = textures)
			GetProc<ProcDeleteTextures>("glDeleteTextures")((GLuint)textures.Length, texturesPtr);
	}

	public void DeleteTexture(uint texture) => GetProc<ProcDeleteTextures>("glDeleteTextures")(1, &texture);
	public void GenerateMipmap(GlTextureTarget target) => GetProc<ProcGenerateMipmap>("glGenerateMipmap")(target);

	public void Enable(GlCapability cap) => GetProc<ProcEnable>("glEnable")(cap);
	public void Disable(GlCapability cap) => GetProc<ProcDisable>("glDisable")(cap);
	public void BlendFunc(GlBlendFactor sfactor, GlBlendFactor dfactor) => GetProc<ProcBlendFunc>("glBlendFunc")(sfactor, dfactor);

	public void TexImage3D<T>(GlTextureTarget target, GLint level, GlTextureInternalFormat internalFormat, GLsizei width, GLsizei height, GLsizei depth, GLint border,
		GlTextureFormat format, GlTextureType type, ReadOnlySpan<T> data) where T : unmanaged
	{
		fixed (void* dataPtr = data)
			GetProc<ProcTexImage3D>("glTexImage3D")(target, level, internalFormat, width, height, depth, border, format, type, dataPtr);
	}

	public void TexSubImage3D<T>(GlTextureTarget target, GLint level, GLint xoffset, GLint yoffset, GLint zoffset, GLsizei width, GLsizei height, GLsizei depth,
		GlTextureFormat format, GlTextureType type, ReadOnlySpan<T> data) where T : unmanaged
	{
		fixed (void* dataPtr = data)
			GetProc<ProcTexSubImage3D>("glTexSubImage3D")(target, level, xoffset, yoffset, zoffset, width, height, depth, format, type, dataPtr);
	}

	public GLint GetAttribLocation(GLuint program, string name) => GetProc<ProcGetAttribLocation>("glGetAttribLocation")(program, name);

}
