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
<<<<<<< HEAD
		private float bulletSpeed = 0.5f;
=======
		private float bulletSpeed = 100.0f;
		private static TextureInfo fireTex = new TextureInfo("/Application/Assets/FireBullet.png");
		private static TextureInfo waterTex = new TextureInfo("/Application/Assets/WaterBullet.png");
		private static TextureInfo neutralTex = new TextureInfo("/Application/Assets/bullet.png");
>>>>>>> origin/Peter
		
		private ProjectileManager ()
		{
			Console.WriteLine ("HERE");
			scene = GameSceneManager.currentScene;
			projectiles = new List<Projectile>();
		}
		
		public static ProjectileManager Instance
		{
			get{return instance;}
		}
		
		public void Shoot(Vector2 playerPos, Vector2 direction, int type)
		{
			Vector2 Velocity = new Vector2(direction.X * bulletSpeed, direction.Y * bulletSpeed);
			Projectile newProjectile;
			if(type == 0)
			{
				newProjectile = new Projectile(scene, neutralTex, playerPos, Velocity);
				projectiles.Add(newProjectile);
			}
			else if(type == 1)
			{
				newProjectile = new Projectile(scene, fireTex, playerPos, Velocity);
				projectiles.Add(newProjectile);
			}
			else if(type == 2)
			{
				newProjectile = new Projectile(scene, waterTex, playerPos, Velocity);
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
				if(projectile.hasCollided(pos, size) || projectile.offScreen())
				{
					projectile.collided = true;
					collision = true;
				}
			}
			return collision;
		}
	}
}

