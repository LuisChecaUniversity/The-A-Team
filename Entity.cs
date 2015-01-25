using System;
using Sce.PlayStation.Core;
using Sce.PlayStation.HighLevel.GameEngine2D;
using Sce.PlayStation.HighLevel.GameEngine2D.Base;

namespace PairedGame
{
	public class Entity: SpriteTile
	{
		public bool IsAlive { get; protected set; }
		
		public Entity(Vector2 position):
			base()
		{
			TextureInfo = TextureManager.Get("entities");
			// Size and position
			Position = position;
			Quad.S = TextureInfo.TileSizeInPixelsf;
			IsAlive = true;
			// Attach update function to scheduler
			ScheduleUpdate();
		}
		
		public Entity(Vector2i tileIndex2D, Vector2 position):
			this(position)
		{
			TileIndex2D = tileIndex2D;
		}
		
		override public void Update(float dt)
		{
			base.Update(dt);
		}
		
		private static float boundsScale = 0.7f;
		
		public bool Overlaps(SpriteBase sprite)
		{
			Bounds2 otherBounds = new Bounds2();
			Bounds2 thisBounds = new Bounds2();
			sprite.GetContentWorldBounds(ref otherBounds);
			GetContentWorldBounds(ref thisBounds);
			thisBounds = thisBounds.Scale(new Vector2(boundsScale, boundsScale), thisBounds.Center);
			return thisBounds.Overlaps(otherBounds);
		}
	}
}
