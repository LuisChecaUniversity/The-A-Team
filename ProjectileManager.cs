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

		

		public static TextureInfo fireTex = new TextureInfo("/Application/assets/FireBullet.png");
		private static TextureInfo waterTex = new TextureInfo("/Application/assets/WaterBullet.png");
		public static TextureInfo neutralTex = new TextureInfo("/Application/assets/bullet.png");

		
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
		
		public void Shoot(Player player)//, Vector2 playerPos, Vector2 direction, char type)
		{
			Projectile newProjectile = new Projectile(player);
			projectiles.Add(newProjectile);
		}
		
		public void Update(float dt)
		{
			foreach(Projectile projectile in projectiles)
			{
				projectile.Update(dt);
				if(projectile.offScreen())
				{
					projectile.collided = true;
				}
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
		
		public Projectile ProjectileCollision(Tile player)
		{
			Projectile p = null;
			//bool collision = false;
			Vector2 size = new Vector2(player.Quad.Bounds2().Point11.X, player.Quad.Bounds2().Point11.Y);
			// Will need to check this against every tile + player positions
			foreach(Projectile projectile in projectiles)
			{
				if(projectile.GetPlayer() == player)
					return null;
				if(projectile.hasCollided(player.Position, size))
				{
					projectile.collided = true;
					return projectile;
					//collision = true;
				}
				
			}
			return p;
			//return collision;
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
					type = projectile.GetPlayer().Element;
//					switch(projectile.getType())
//					{
//					case Type.Neutral:
//						type = 'N';
//					break;
//					case Type.Fire:
//						type = 'F';
//					break;
//					case Type.Water:
//						type = 'W';
//					break;
//					}
				}
			}
			return type;
		}
	}
}

