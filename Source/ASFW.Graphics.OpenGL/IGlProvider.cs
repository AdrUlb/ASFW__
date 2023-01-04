namespace ASFW.Graphics.OpenGL;

using GLboolean = System.Boolean;
using GLchar = System.Byte;
using GLenum = System.UInt32;
using GLfloat = System.Single;
using GLint = System.Int32;
using GLintptr = System.IntPtr;
using GLsizei = System.UInt32;
using GLsizeiptr = System.UIntPtr;
using GLuint = System.UInt32;

public unsafe interface IGlProvider
{
	public string? GetString(GlStringName name);
	public string? GetString(GlStringName name, GLuint index);
	public void ClearColor(GLfloat red, GLfloat green, GLfloat blue, GLfloat alpha);
	public void Clear(GlClearMask mask);
	public void Viewport(GLint x, GLint y, GLsizei width, GLsizei height);
	public void GenBuffers(Span<GLuint> buffers);
	public GLuint GenBuffer();
	public void DeleteBuffers(ReadOnlySpan<GLuint> buffers);
	public void DeleteBuffer(GLuint buffer);
	public void BufferData<T>(GlBufferTarget target, ReadOnlySpan<T> data, GlBufferUsage usage) where T : unmanaged;
	public void BufferData(GlBufferTarget target, GLsizeiptr size, GlBufferUsage usage);
	public void BufferSubData<T>(GlBufferTarget target, nint offset, nuint size, ReadOnlySpan<T> data) where T : unmanaged;
	public void BindBuffer(GlBufferTarget target, GLuint buffer);
	public void GenVertexArrays(Span<GLuint> arrays);
	public GLuint GenVertexArray();
	public void DeleteVertexArrays(ReadOnlySpan<GLuint> arrays);
	public void DeleteVertexArray(GLuint array);
	public void BindVertexArray(GLuint array);
	public void VertexAttribPointer(GLuint index, GLint size, GlVertexAttribPointerType type, GLboolean normalized, GLsizei stride, nuint pointer);
	public void EnableVertexAttribArray(GLuint index);
	public void DrawArrays(GlDrawMode mode, GLint first, GLsizei count);
	public GLuint CreateShader(GlShaderType shaderType);
	public void DeleteShader(GLuint shader);
	public void ShaderSource(GLuint shader, string str);
	public void CompileShader(GLuint shader);
	public GLint GetShader(GLuint shader, GlShaderParameterName pname);
	public string GetShaderInfoLog(GLuint shader);
	public GLuint CreateProgram();
	public void DeleteProgram(GLuint program);
	public void AttachShader(GLuint program, GLuint shader);
	public void DetachShader(GLuint program, GLuint shader);
	public void LinkProgram(GLuint program);
	public GLint GetProgram(GLuint program, GlProgramParameterName pname);
	public string GetProgramInfoLog(GLuint program);
	public void UseProgram(GLuint program);
	public void DrawElements(GlDrawMode mode, GLsizei count, GlIndexType type, nuint indices);
	public GLint GetUniformLocation(GLuint program, string name);
	public void Uniform1f(GLint location, GLfloat v0);
	public void Uniform2f(GLint location, GLfloat v0, GLfloat v1);
	public void Uniform3f(GLint location, GLfloat v0, GLfloat v1, GLfloat v2);
	public void Uniform4f(GLint location, GLfloat v0, GLfloat v1, GLfloat v2, GLfloat v3);
	public void Uniform1i(GLint location, GLint v0);
	public void Uniform2i(GLint location, GLint v0, GLint v1);
	public void Uniform3i(GLint location, GLint v0, GLint v1, GLint v2);
	public void Uniform4i(GLint location, GLint v0, GLint v1, GLint v2, GLint v3);
	public void Uniform1ui(GLint location, GLuint v0);
	public void Uniform2ui(GLint location, GLuint v0, GLuint v1);
	public void Uniform3ui(GLint location, GLuint v0, GLuint v1, GLuint v2);
	public void Uniform4ui(GLint location, GLuint v0, GLuint v1, GLuint v2, GLuint v3);
	public void UniformMatrix2fv(GLint location, GLsizei count, GLboolean transpose, GLfloat* value);
	public void UniformMatrix3fv(GLint location, GLsizei count, GLboolean transpose, GLfloat* value);
	public void UniformMatrix4fv(GLint location, GLsizei count, GLboolean transpose, GLfloat* value);
	public void UniformMatrix2x3fv(GLint location, GLsizei count, GLboolean transpose, GLfloat* value);
	public void UniformMatrix3x2fv(GLint location, GLsizei count, GLboolean transpose, GLfloat* value);
	public void UniformMatrix2x4fv(GLint location, GLsizei count, GLboolean transpose, GLfloat* value);
	public void UniformMatrix4x2fv(GLint location, GLsizei count, GLboolean transpose, GLfloat* value);
	public void UniformMatrix3x4fv(GLint location, GLsizei count, GLboolean transpose, GLfloat* value);
	public void UniformMatrix4x3fv(GLint location, GLsizei count, GLboolean transpose, GLfloat* value);
	public void PolygonMode(GlPolygonFace face, GlPolygonModes mode);
	public void GenTextures(Span<GLuint> textures);
	public GLuint GenTexture();
	public void BindTexture(GlTextureTarget target, GLuint texture);
	public void TexParameterf(GlTextureTarget target, GlTextureParameterName pname, GLfloat param);
	public void TexParameteri(GlTextureTarget target, GlTextureParameterName pname, GLint param);
	public void TexParameter(GlTextureTarget target, GlTextureMinFilter textureMinFilter);
	public void TexParameter(GlTextureTarget target, GlTextureMagFilter textureMagFilter);
	public void ActiveTexture(uint unit);
	public void TexImage2D<T>(GlTextureTarget target, GLint level, GlTextureInternalFormat internalFormat, GLsizei width, GLsizei height, GLint border, GlTextureFormat format,
		GlTextureType type, ReadOnlySpan<T> data) where T : unmanaged;
	public void TexSubImage2D<T>(GlTextureTarget target, GLint level, GLint xoffset, GLint yoffset, GLsizei width, GLsizei height, GlTextureFormat format, GlTextureType type,
		ReadOnlySpan<T> data) where T : unmanaged;
	public void DeleteTextures(ReadOnlySpan<uint> textures);
	public void DeleteTexture(uint texture);
	public void GenerateMipmap(GlTextureTarget target);
	public void Enable(GlCapability cap);
	public void Disable(GlCapability cap);
	public void BlendFunc(GlBlendFactor sfactor, GlBlendFactor dfactor);
	public void TexImage3D<T>(GlTextureTarget target, GLint level, GlTextureInternalFormat internalFormat, GLsizei width, GLsizei height, GLsizei depth, GLint border,
		GlTextureFormat format, GlTextureType type, ReadOnlySpan<T> data) where T : unmanaged;
	public void TexSubImage3D<T>(GlTextureTarget target, GLint level, GLint xoffset, GLint yoffset, GLint zoffset, GLsizei width, GLsizei height, GLsizei depth,
		GlTextureFormat format, GlTextureType type, ReadOnlySpan<T> data) where T : unmanaged;
	public GLint GetAttribLocation(GLuint program, string name);
}
