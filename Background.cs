using Sce.PlayStation.HighLevel.GameEngine2D;
using Sce.PlayStation.Core;

namespace TheATeam
{
	public class Background: SpriteUV
	{
		public Background(): base()
		{
			TextureInfo = TextureManager.Get("background");
			Quad.S = TextureInfo.TextureSizef;
		}
	}
}

