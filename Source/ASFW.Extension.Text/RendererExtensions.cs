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

		foreach (var c in text)
		{
			switch (c)
			{
				case '\n':
					position.Y += (int)font.Face->Size->Metrics.Height >> 6;
					position.X = startX;
					continue;
			}

			var data = font.GetFontCharData(c);
			var texSec = data.TextureSection;

			var pos = new Vector2(
				position.X + data.BitmapLeft,
				position.Y - data.BitmapTop + font.Height
			);

			var size = new Vector2(texSec.Width, texSec.Height);

			renderer.DrawTextureSection(pos, size, texSec, color);

			position.X += data.AdvanceX >> 6;
		}
	}

	public unsafe static Vector2 MeasureText(this Renderer renderer, string text, Font font)
	{
		var minX = 0f;
		var maxX = 0f;
		var minY = 0f;
		var maxY = 0f;
		
		var position = new Vector2();
		var startX = position.X;

		foreach (var c in text)
		{
			switch (c)
			{
				case '\n':
					position.Y += (int)font.Face->Size->Metrics.Height >> 6;
					position.X = startX;
					continue;
			}

			var data = font.GetFontCharData(c);
			var texSec = data.TextureSection;

			var pos = new Vector2(
				position.X + data.BitmapLeft,
				position.Y - data.BitmapTop + font.Height
			);

			var size = new Vector2(texSec.Width, texSec.Height);

			minX = Math.Min(minX, pos.X);
			minY = Math.Min(minY, pos.Y);
			maxX = Math.Max(maxX, pos.X + size.X - 1);
			maxY = Math.Max(maxY, pos.Y + size.Y - 1);
			
			position.X += data.AdvanceX >> 6;
		}

		var width = maxX - minX + 1;
		var height = maxY - minY + 1;

		return new(width, height);
	}
}
