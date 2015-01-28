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
		private float bulletSpeed = 100.0f;
		
		private ProjectileManager ()
		{
			Console.WriteLine ("HERE");
			scene = Director.Instance.CurrentScene;
			projectiles = new List<Projectile>();
		}
		
		public static ProjectileManager Instance
		{
			get{return instance;}
		}
		
		public void Shoot(Vector2 playerPos, Vector2 direction)
		{
			Vector2 Velocity = new Vector2(direction.X * bulletSpeed, direction.Y * bulletSpeed);
			Projectile newProjectile = new Projectile(scene, playerPos, Velocity);
			projectiles.Add(newProjectile);
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
		public void ProjectileCollision(Vector2 pos, Bounds2 bounds)
		{
			Vector2 size = new Vector2(bounds.Point11.X, bounds.Point11.Y);
			// Will need to check this against every tile + player positions
			foreach(Projectile projectile in projectiles)
			{
				if(projectile.hasCollided(pos, size) || projectile.offScreen())
					projectile.collided = true;
			}
		}
	}
}

