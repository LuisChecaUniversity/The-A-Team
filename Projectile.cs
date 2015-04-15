using System;
using Sce.PlayStation.Core;
using Sce.PlayStation.Core.Graphics;

using Sce.PlayStation.HighLevel.GameEngine2D;
using Sce.PlayStation.HighLevel.GameEngine2D.Base;

namespace TheATeam
{
	public enum Type
	{
		Neutral = 0,
		Fire = 1,
		FireAir = 2,
	};
	public class Projectile
	{
		//public TextureInfo bulletTex;
		private SpriteUV bulletSprite;
		private Vector2 position;
		private Vector2 velocity;
		private Vector2 rotation;
		public bool collided;
		private Type bulletType;
		private float bulletSpeed = 0.5f;
		private Player player;
		public int bulletDamage = 10;

		public Projectile (Player player)
		{
			//bulletTex = new TextureInfo("/Application/Assets/bullet.png");
			if(player.Element == 'F' ||  player.Element2 == 'F')
			{
				bulletSprite = new SpriteUV(ProjectileManager.fireTex);
				bulletSprite.Quad.S = ProjectileManager.fireTex.TextureSizef;
				
				if(player.Element == 'L' ||  player.Element2 == 'L')
					bulletDamage = 45;
				else
					bulletDamage = 35;
				
				if(player.Element == 'A' ||  player.Element2 == 'A')
					bulletType = Type.FireAir;
				else
					bulletType = Type.Fire;
			}
			else
			{
				bulletSprite = new SpriteUV(ProjectileManager.neutralTex);
				bulletSprite.Quad.S = ProjectileManager.neutralTex.TextureSizef;
				bulletDamage = 25;
				bulletType = Type.Neutral;
			}
			bulletSprite.CenterSprite();
			this.player = player;

			
			float offset = 0.0f;
//			if(player.GetDirection().X == 0.0f || player.GetDirection().Y == 0.0f)
//				offset = 50.0f;
//			else
//				offset = 77.0f;
			

			rotation = player.GetShootingDirection().Normalize();
			velocity = player.GetShootingDirection().Normalize() * bulletSpeed;
			position = new Vector2(player.Position.X + rotation.X * offset, player.Position.Y + rotation.Y * offset); 
			collided = false;
			
			Director.Instance.CurrentScene.AddChild(bulletSprite);
			//GameSceneManager.currentScene.AddChild(bulletSprite);
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
		public void setType(Type t){bulletType = t;}
		public Type getType(){return bulletType;}
		public Player GetPlayer() { return player;}
		
	}
}

