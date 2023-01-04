using System.Numerics;
using Android.Content;
using Android.Opengl;
using Android.Util;
using ASFW.Graphics;
using ASFW.Graphics.OpenGL;
using Javax.Microedition.Khronos.Opengles;
using EGLConfig = Javax.Microedition.Khronos.Egl.EGLConfig;
using Size = System.Drawing.Size;

namespace ASFW.Platform.Android;

public sealed class RenderView : GLSurfaceView, GLSurfaceView.IRenderer, IRenderContext
{
	public delegate void RenderEvent(Renderer renderer);

	public event RenderEvent? Render;

	public Size FramebufferSize { get; private set; }

	public Vector2 RecommendedScale { get; }

	public readonly string GLRenderer;
	public readonly string GLVendor;
	public readonly string GLVersion;
	public readonly string GLSLVersion;

	private Renderer? renderer = null;

	public RenderView(Context context) : base(context)
	{
		GLRenderer = AndroidAsfwPlatform.Gl.GetString(GlStringName.Renderer)!;
		GLVendor = AndroidAsfwPlatform.Gl.GetString(GlStringName.Vendor)!;
		GLVersion = AndroidAsfwPlatform.Gl.GetString(GlStringName.Version)!;
		GLSLVersion = AndroidAsfwPlatform.Gl.GetString(GlStringName.ShadingLanguageVersion)!;
		SetEGLContextClientVersion(2);
		SetRenderer(this);
		RecommendedScale = new((float)context.Resources!.DisplayMetrics!.DensityDpi / 200);
	}

	public void OnSurfaceCreated(IGL10? gl, EGLConfig? config)
	{
		renderer ??= new(this);
	}

	public void OnDrawFrame(IGL10? gl)
	{
		if (renderer == null)
			return;

		Render?.Invoke(renderer);
		renderer.CommitBatch();
	}

	public void OnSurfaceChanged(IGL10? gl, int width, int height)
	{
		FramebufferSize = new(width, height);
		renderer?.UpdateViewportAndProjection();
	}

	protected override void Dispose(bool disposing)
	{
		renderer?.Dispose();
		base.Dispose(disposing);
	}
}
