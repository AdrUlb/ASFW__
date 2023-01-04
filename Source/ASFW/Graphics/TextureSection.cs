using System.Drawing;

namespace ASFW.Graphics;

public struct TextureSection
{
	public Texture Texture;
	public int X;
	public int Y;
	public int Width;
	public int Height;
	
	public TextureSection(Texture texture, int x, int y, int width, int height)
	{
		Texture = texture;
		X = x;
		Y = y;
		Width = width;
		Height = height;
	}
	
	public Color this[int x, int y]
	{
		get
		{
			if (x < 0 || y < 0 || x >= Width || y >= Height)
				throw new IndexOutOfRangeException();

			var texX = x + X;
			var texY = y + Y;
			
			return Texture[texX, texY];
		}

		set
		{
			if (x < 0 || y < 0 || x >= Width || y >= Height)
				throw new IndexOutOfRangeException();

			var texX = x + X;
			var texY = y + Y;
			
			Texture[texX, texY] = value;
		}
	}
}

