using System;
using System.Diagnostics;
using System.Collections.Generic;
using Sce.PlayStation.Core;
using Sce.PlayStation.Core.Graphics;
using Sce.PlayStation.HighLevel.GameEngine2D;
using Sce.PlayStation.HighLevel.GameEngine2D.Base;

namespace TheATeam
{
	public class ProjectileManager
	{
		private static ProjectileManager instance = new ProjectileManager();
		private static List<Projectile> projectiles;
		private Scene scene;

		

		private static TextureInfo fireTex = new TextureInfo("/Application/assets/FireBullet.png");
		private static TextureInfo waterTex = new TextureInfo("/Application/assets/WaterBullet.png");
		private static TextureInfo neutralTex = new TextureInfo("/Application/assets/bullet.png");

		
		private ProjectileManager ()
		{
			scene = GameSceneManager.currentScene;
			projectiles = new List<Projectile>();
		}
		
		public static ProjectileManager Instance
		{
			get{return instance;}
		}
		public Scene GetScene()
		{
			return scene;
		}
		
		public void Shoot(Vector2 playerPos, Vector2 direction, char type)
		{
			Vector2 Velocity = new Vector2(direction.X, direction.Y);
			Projectile newProjectile;
			if(type == 'N')
			{
				newProjectile = new Projectile(neutralTex, playerPos, Velocity);
				newProjectile.setType(Type.Neutral);
				projectiles.Add(newProjectile);
			}
			else if(type == 'F')
			{
				newProjectile = new Projectile(fireTex, playerPos, Velocity);
				newProjectile.setType(Type.Fire);
				projectiles.Add(newProjectile);
			}
			else if(type == 'W')
			{
				newProjectile = new Projectile(waterTex, playerPos, Velocity);
				newProjectile.setType(Type.Water);
				projectiles.Add(newProjectile);
			}
		}
		
		public void Update(float dt)
		{
			foreach(Projectile projectile in projectiles)
			{
				projectile.Update(dt);
			}
			for(int i = 0; i < projectiles.Count; i++)
			{
				if(projectiles[i].collided)
				{
					projectiles[i].Dispose();
					projectiles[i] = null;
					projectiles.RemoveAt(i);
					i--;
				}
			}
		}
		public bool ProjectileCollision(Vector2 pos, Bounds2 bounds)
		{
			bool collision = false;
			Vector2 size = new Vector2(bounds.Point11.X, bounds.Point11.Y);
			// Will need to check this against every tile + player positions
			foreach(Projectile projectile in projectiles)
			{
				if(projectile.hasCollided(pos, size))
				{
					projectile.collided = true;
					collision = true;
				}
				else if(projectile.offScreen())
				{
					projectile.collided = true;
				}
			}
			return collision;
		}
		public char ProjectileTileCollision(Vector2 pos, Bounds2 bounds)
		{
			char type = 'X';
			Vector2 size = new Vector2(bounds.Point11.X, bounds.Point11.Y);
			// Will need to check this against every tile + player positions
			foreach(Projectile projectile in projectiles)
			{
				if(projectile.hasCollided(pos, size))
				{
					projectile.collided = true;
					switch(projectile.getType())
					{
					case Type.Neutral:
						type = 'N';
					break;
					case Type.Fire:
						type = 'F';
					break;
					case Type.Water:
						type = 'W';
					break;
					}
				}
			}
			return type;
		}
	}
}

