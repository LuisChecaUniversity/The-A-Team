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
		private Vector2 rotation;
		public bool collided;
		private float bulletSpeed = 50.0f;

		public Projectile (Scene scene, Vector2 pos, Vector2 vel)
		{
			bulletTex = new TextureInfo("/Application/Assets/bullet.png");
			bulletSprite = new SpriteUV(bulletTex);
			bulletSprite.Quad.S = bulletTex.TextureSizef;
			bulletSprite.CenterSprite();
			
			float offset;
			if(vel.X == 0.0f || vel.Y == 0.0f)
				offset = 37.0f;
			else
				offset = 50.0f;
			
			
			rotation = vel.Normalize();
			velocity = vel;
			position = new Vector2(pos.X + rotation.X * offset, pos.Y + rotation.Y * offset); 
			collided = false;
			
			
			
			scene.AddChild(bulletSprite);
		}
		public void Dispose()
		{
			Director.Instance.CurrentScene.RemoveChild(bulletSprite, true);
			bulletSprite.RegisterDisposeOnExitRecursive();			
		}
		
		public void Update(float dt)
		{
			position = new Vector2(position.X + velocity.X * dt, position.Y + velocity.Y *dt);
			rotation = velocity.Normalize();
			
			bulletSprite.Rotation = rotation;
			bulletSprite.Position = position;
		}
		
		public bool hasCollided(Vector2 objectPosition, Vector2 objectSize)
		{
			// Collision for objects that are centred
			
			Bounds2 bulletBounds = bulletSprite.Quad.Bounds2();
			float bulletWidth = bulletBounds.Point11.X;
			// because bullets rotate if a bullet is traveling in the y direction its collision 
			// will be based off height - which is now the rotated width
			float bulletHeight = bulletBounds.Point11.X;
			
			float objectWidth = objectSize.X;
			float objectHeight = objectSize.Y;
			
//			if((position.X + bulletWidth) < objectPosition.X - objectWidth)
//				return false;
//			else if(position.X - bulletWidth > (objectPosition.X + objectWidth))
//				return false;
//			else if((position.Y + bulletHeight) < objectPosition.Y - objectHeight)
//				return false;
//			else if(position.Y - bulletHeight > (objectPosition.Y + objectHeight))
//				return false;
//			else 
//				return true;
			
			if(position.X - bulletWidth> objectPosition.X + objectWidth)
				return false;
			else if(position.X + bulletWidth < objectPosition.X )
				return false;
			else if(position.Y - bulletHeight > objectPosition.Y + objectHeight)
				return false;
			else if(position.Y + bulletHeight < objectPosition.Y )
				return false;
			else 
				return true;
			
		}
		public bool offScreen()
		{
			float width = Director.Instance.GL.Context.GetViewport().Width;
			float height = Director.Instance.GL.Context.GetViewport().Height;
			if(position.X > width)
				return true;
			else if(position.X < 0.0f)
				return true;
			else if(position.Y < 0.0f)
				return true;
			else if(position.Y  > height)
				return true;
			else 
				return false;
		}
		
	}
}

