using System;
using Sce.PlayStation.Core;
using Sce.PlayStation.Core.Graphics;

using Sce.PlayStation.HighLevel.GameEngine2D;
using Sce.PlayStation.HighLevel.GameEngine2D.Base;

namespace TheATeam
{
	public class Projectile
	{
		public TextureInfo bulletTex;
		public SpriteUV bulletSprite;
		public Vector2 position;
		public Vector2 velocity;
		public bool collided;
		
		public Projectile (Scene scene, Vector2 pos, Vector2 vel)
		{
			bulletTex = new TextureInfo("/Application/Assets/fireBullet.png");
			bulletSprite = new SpriteUV(bulletTex);
			bulletSprite.Quad.S = bulletTex.TextureSizef;
			bulletSprite.CenterSprite(TRS.Local.Center);
			
			scene.AddChild(bulletSprite);
			
			position = pos;
			velocity = vel;
			collided = false;
		}
		
		public void Update(float dt)
		{
			position = new Vector2(velocity.X * dt, velocity.Y *dt);
		}
		
		public bool hasCollided(Vector2 objectPosition, Vector2 objectSize)
		{
			// Collision for objects that are centred
			
			Bounds2 bulletBounds = bulletSprite.Quad.Bounds2();
			float bulletWidth = bulletBounds.Point11.X;
			float bulletHeight = bulletBounds.Point11.Y;
			
			float objectWidth = objectSize.X;
			float objectHeight = objectSize.Y;
			
			if((position.X + bulletWidth) < objectPosition.X - objectWidth)
				return false;
			else if(position.X - bulletWidth > (objectPosition.X + objectWidth))
				return false;
			else if((position.Y + bulletHeight) < objectPosition.Y - objectHeight)
				return false;
			else if(position.Y - bulletHeight > (objectPosition.Y + objectHeight))
				return false;
			else 
				return true;
			
		}
	}
}

