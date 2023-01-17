using System.Collections.Concurrent;
using System.Drawing;
using System.Numerics;
using ASFW.Graphics.OpenGL;
using ASFW.Graphics.OpenGL.Abstractions;

namespace ASFW.Graphics;

public sealed class Renderer : IDisposable
{
	private readonly struct BatchEntryVertices
	{
		public readonly Vector2 TopLeft;
		public readonly Vector2 TopRight;
		public readonly Vector2 BottomLeft;
		public readonly Vector2 BottomRight;

		public BatchEntryVertices(Vector2 topLeft, Vector2 topRight, Vector2 bottomLeft, Vector2 bottomRight)
		{
			TopLeft = topLeft;
			TopRight = topRight;
			BottomLeft = bottomLeft;
			BottomRight = bottomRight;
		}
	}

	private readonly struct BatchEntry
	{
		public readonly BatchEntryVertices Vertices;
		public readonly BatchEntryVertices TexCoords;
		public readonly Color Tint;

		public BatchEntry(in BatchEntryVertices vertices, in BatchEntryVertices texCoords, Color tint)
		{
			Vertices = vertices;
			TexCoords = texCoords;
			Tint = tint;
		}
	}

	private readonly IRenderContext context;

	private const nuint triangleVerts = 3;
	private const nuint triangleVertBytes = triangleVerts * 2 * sizeof(float);

	private readonly GLProgram textureShader;
	private readonly GLBuffer batchBuffer = new(Asfw.Gl);
	
	internal static readonly ConcurrentDictionary<Renderer, ConcurrentDictionary<Texture, GLTexture>> TextureCache = new();

	private static readonly Texture rectTexture = new(1, 1);

	private Texture? boundTexture = null;
	private readonly List<BatchEntry> batch = new();

	static Renderer()
	{
		rectTexture.Lock();
		rectTexture[0, 0] = Color.White;
		rectTexture.Unlock();
	}

	public Renderer(IRenderContext context)
	{
		this.context = context;

		ReadOnlySpan<uint> rectangleIndices = stackalloc uint[] { 0, 1, 2, 0, 2, 3 };

		textureShader = Asfw.AssetLoader.LoadShaderProgram("Texture");

		Asfw.Gl.BufferData(GlBufferTarget.ArrayBuffer, triangleVertBytes, GlBufferUsage.DynamicDraw);

		Asfw.Gl.BufferData(GlBufferTarget.ElementArrayBuffer, rectangleIndices, GlBufferUsage.StaticDraw);

		UpdateViewportAndProjection();

		Asfw.Gl.PolygonMode(GlPolygonFace.FrontAndBack, GlPolygonModes.Fill);
		Asfw.Gl.BlendFunc(GlBlendFactor.SrcAlpha, GlBlendFactor.OneMinusSrcAlpha);

		TextureCache.TryAdd(this, new());
	}

	public void UpdateViewportAndProjection()
	{
		var size = context.FramebufferSize;
		Asfw.Gl.Viewport(0, 0, (uint)(size.Width), (uint)(size.Height));
		var projection = Matrix4x4.CreateOrthographicOffCenter(0, size.Width, size.Height, 0, -1.0f, 1.0f);
		textureShader.Use();
		textureShader.UniformMatrix4fv("projection", false, projection);
	}

	public void Clear(Color color)
	{
		batch.Clear();
		Asfw.Gl.ClearColor(color.R / 255.0f, color.G / 255.0f, color.B / 255.0f, color.A / 255.0f);
		Asfw.Gl.Clear(GlClearMask.ColorBufferBit);
	}

	public void DrawTextureSection(Vector2 position, Vector2 size, TextureSection section, Color tint)
	{
		var bind = false;

		if (!TextureCache[this].TryGetValue(section.Texture, out var tex))
		{
			tex = section.Texture.GetGlTexture();
			TextureCache[this].TryAdd(section.Texture, tex);
			bind = true;
		}

		if (boundTexture != section.Texture)
		{
			if (boundTexture != null)
				CommitBatch();
			bind = true;
		}

		if (bind)
		{
			tex.Bind(0);
			boundTexture = section.Texture;
		}

		var bottomLeft = position;
		var bottomRight = position + size with { Y = 0.0f };
		var topLeft = position + size with { X = 0.0f };
		var topRight = position + size;

		var xOff = section.X / (float)section.Texture.Width;
		var xSize = section.Width / (float)section.Texture.Width;
		var yOff = section.Y / (float)section.Texture.Height;
		var ySize = section.Height / (float)section.Texture.Height;

		var topLeftTex = new Vector2(xOff, ySize + yOff);
		var bottomLeftTex = new Vector2(xOff, yOff);
		var bottomRightTex = new Vector2(xSize + xOff, yOff);
		var topRightTex = new Vector2(xSize + xOff, ySize + yOff);

		var entry = new BatchEntry(new(topLeft, topRight, bottomLeft, bottomRight), new(topLeftTex, topRightTex, bottomLeftTex, bottomRightTex), tint);

		batch.Add(entry);
	}

	public void DrawTexture(Vector2 position, Vector2 size, Texture texture, Color tint) =>
		DrawTextureSection(position, size, new(texture, 0, 0, texture.Width, texture.Height), tint);

	public void FillRectangle(Vector2 position, Vector2 size, Color color) => DrawTexture(position, size, rectTexture, color);

	public unsafe void CommitBatch()
	{
		var vertsSize = batch.Count * 4 * sizeof(Vector2);
		var texCoordsSize = batch.Count * 4 * sizeof(Vector2);
		var texColorsSize = batch.Count * 4 * sizeof(Vector4);
		var indicesSize = batch.Count * 6 * sizeof(uint);

		var buffer = stackalloc byte[vertsSize + texCoordsSize + texColorsSize + indicesSize];
		var bufferSpan = new Span<byte>(buffer, vertsSize + texCoordsSize + texColorsSize + indicesSize);
		var verts = (Vector2*)buffer;
		var texCoords = (Vector2*)(buffer + vertsSize);
		var texColors = (Vector4*)(buffer + vertsSize + texCoordsSize);
		var indices = (uint*)(buffer + vertsSize + texCoordsSize + texColorsSize);
		
		for (var i = 0; i < batch.Count; i++)
		{
			var entry = batch[i];
			verts[i * 4] = entry.Vertices.TopLeft;
			verts[i * 4 + 1] = entry.Vertices.BottomLeft;
			verts[i * 4 + 2] = entry.Vertices.BottomRight;
			verts[i * 4 + 3] = entry.Vertices.TopRight;

			texCoords[i * 4] = entry.TexCoords.TopLeft;
			texCoords[i * 4 + 1] = entry.TexCoords.BottomLeft;
			texCoords[i * 4 + 2] = entry.TexCoords.BottomRight;
			texCoords[i * 4 + 3] = entry.TexCoords.TopRight;

			var indexOffset = (uint)(i * 4);

			indices[i * 6] = 0 + indexOffset;
			indices[i * 6 + 1] = 1 + indexOffset;
			indices[i * 6 + 2] = 2 + indexOffset;
			indices[i * 6 + 3] = 0 + indexOffset;
			indices[i * 6 + 4] = 2 + indexOffset;
			indices[i * 6 + 5] = 3 + indexOffset;

			texColors[i * 4] = new(entry.Tint.R / 255.0f, entry.Tint.G / 255.0f, entry.Tint.B / 255.0f, entry.Tint.A / 255.0f);
			texColors[i * 4 + 1] = new(entry.Tint.R / 255.0f, entry.Tint.G / 255.0f, entry.Tint.B / 255.0f, entry.Tint.A / 255.0f);
			texColors[i * 4 + 2] = new(entry.Tint.R / 255.0f, entry.Tint.G / 255.0f, entry.Tint.B / 255.0f, entry.Tint.A / 255.0f);
			texColors[i * 4 + 3] = new(entry.Tint.R / 255.0f, entry.Tint.G / 255.0f, entry.Tint.B / 255.0f, entry.Tint.A / 255.0f);
		}

		batchBuffer.Bind(GlBufferTarget.ArrayBuffer);
		Asfw.Gl.BufferData<byte>(GlBufferTarget.ArrayBuffer, bufferSpan, GlBufferUsage.StaticDraw);

		using var vao = new GLVertexArray(Asfw.Gl);
		vao.VertexAttribPointer(0, 2, GlVertexAttribPointerType.Float, false, (uint)sizeof(Vector2), 0, batchBuffer);
		vao.EnableVertexAttribArray(0);
		vao.VertexAttribPointer(1, 2, GlVertexAttribPointerType.Float, false, (uint)sizeof(Vector2), (nuint)vertsSize, batchBuffer);
		vao.EnableVertexAttribArray(1);
		vao.VertexAttribPointer(2, 4, GlVertexAttribPointerType.Float, false, (uint)sizeof(Vector4), (nuint)(vertsSize + texCoordsSize), batchBuffer);
		vao.EnableVertexAttribArray(2);

		textureShader.Use();
		textureShader.Uniform1i("tex", 0);

		Asfw.Gl.Enable(GlCapability.Blend);
		vao.DrawElements(GlDrawMode.Triangles, (uint)(batch.Count * 6), GlIndexType.UnsignedInt, (nuint)(vertsSize + texCoordsSize + texColorsSize), batchBuffer);
		Asfw.Gl.Disable(GlCapability.Blend);

		batch.Clear();
	}

	public void Dispose()
	{
		foreach (var tex in TextureCache[this])
			tex.Value.Dispose();

		batchBuffer.Dispose();
		textureShader.Dispose();
	}
}
