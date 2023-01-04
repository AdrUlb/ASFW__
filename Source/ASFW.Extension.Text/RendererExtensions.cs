using System.Drawing;
using System.Numerics;
using ASFW.Graphics;

namespace ASFW.Extension.Text;

public static class RendererExtensions
{
	public unsafe static void DrawText(this Renderer renderer, Vector2 position, string text, Font font, Color color)
	{
		position.X = float.Round(position.X);
		position.Y = float.Round(position.Y);

		var startX = position.X;

		var yOffset = 0;

		foreach (var c in text)
		{
			switch (c)
			{
				case '\n':
					yOffset += (int)font.Face->Size->Metrics.Height >> 6;
					position.X = startX;
					continue;
				case '\r':
					position.X = startX;
					continue;
			}

			var data = font.GetFontCharData(c);
			var texSec = data.TextureSection;

			var pos = new Vector2(
				position.X + data.BitmapLeft,
				position.Y - data.BitmapTop + font.Height + yOffset
			);

			var size = new Vector2(texSec.Width, texSec.Height);

			renderer.DrawTextureSection(pos, size, texSec, color);

			position.X += data.AdvanceX >> 6;
		}
	}
}
