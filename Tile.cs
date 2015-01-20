using System;
using Sce.PlayStation.Core;
using Sce.PlayStation.Core.Graphics;
using Sce.PlayStation.HighLevel.GameEngine2D;
using Sce.PlayStation.HighLevel.GameEngine2D.Base;

namespace PairedGame
{
	public enum Collidable
	{
		None = 0,
		Left,
		Bottom,
		Right,
		Top,
		BottomLeft,
		BottomRight,
		TopRight,
		TopLeft
	}
	
	public class Tile: SpriteTile
	{
		private Collidable collidableSides = Collidable.None;
		public char Key;
		
		public Tile(char loadKey, Vector2 position): base()
		{
			TextureInfo = TextureManager.Get("tiles");
			Position = position;
			Quad.S = TextureInfo.TileSizeInPixelsf;
			// Reset variables
			Key = loadKey;
			// Based on loadKey set Tile to draw and its collision.
			switch(loadKey)
			{
			case 'S': 
				TileIndex2D = new Vector2i(Info.Rnd.Next(1, 4), Info.Rnd.Next(1, 4));
				break;
			case 'H':
			case 'X':
				TileIndex2D = new Vector2i(Info.Rnd.Next(1, 4), Info.Rnd.Next(1, 4));
				break;
			case 'D':
				TileIndex2D = new Vector2i(Info.Rnd.Next(8, 11), 1);
				break;
			case 'P':
				TileIndex2D = new Vector2i(0, 4);
				collidableSides = Collidable.TopLeft;
				break;
			case 'O':
				TileIndex2D = new Vector2i(4, 4);
				collidableSides = Collidable.TopRight;
				break;
			case 'I':
				TileIndex2D = new Vector2i(0, 0);
				collidableSides = Collidable.BottomLeft;
				break;
			case 'U':
				TileIndex2D = new Vector2i(4, 0);
				collidableSides = Collidable.BottomRight;
				break;
			case 'M':
				TileIndex2D = new Vector2i(Info.Rnd.Next(1, 4), 4);
				collidableSides = Collidable.Top;
				break;
			case 'N':
				TileIndex2D = new Vector2i(Info.Rnd.Next(1, 4), 0);
				collidableSides = Collidable.Bottom;
				break;
			case 'B':
				TileIndex2D = new Vector2i(0, Info.Rnd.Next(1, 4));
				collidableSides = Collidable.Left;
				break;
			case 'V':
				TileIndex2D = new Vector2i(4, Info.Rnd.Next(1, 4));
				collidableSides = Collidable.Right;
				break;
			case 'Z':
				TileIndex2D = new Vector2i(11, 3);
				break;
			default:
				break;
			}
		}

		private static float boundsScale = 0.6f;
		
		public bool Overlaps(SpriteBase sprite)
		{
			Bounds2 otherBounds = new Bounds2();
			Bounds2 thisBounds = new Bounds2();
			sprite.GetContentWorldBounds(ref otherBounds);
			GetContentWorldBounds(ref thisBounds);
			thisBounds = thisBounds.Scale(new Vector2(boundsScale, boundsScale), thisBounds.Center);
			return thisBounds.Overlaps(otherBounds);
		}
		
		public void HandleCollision(Vector2 pos, ref Vector2 speed)
		{
			// Collision offset and returning force
			int offset = (int)(System.Math.Min(Width, Height) * (1 - boundsScale) / 4);
			float factor = -1f;
			// Repeating booleans
			bool collisionLeft = pos.X < Position.X + offset && speed.X < 0;
			bool collisionRight = pos.X + Width > Position.X + Width - offset && speed.X > 0;
			bool collisionTop = pos.Y + Height > Position.Y + Height - offset && speed.Y > 0;
			bool collisionBottom = pos.Y < Position.Y + offset && speed.Y < 0;
			
			switch(collidableSides)
			{
			case Collidable.Bottom:
			case Collidable.BottomLeft:
			case Collidable.BottomRight:
				if(collisionBottom)
					speed.Y *= factor;
				
				if((collisionLeft && collidableSides == Collidable.BottomLeft) ||
					(collisionRight && collidableSides == Collidable.BottomRight))
					speed.X *= factor;
				break;
			case Collidable.Left:
				if(collisionLeft)
					speed.X *= factor;
				break;
			case Collidable.Right:
				if(collisionRight)
					speed.X *= factor;
				break;
			case Collidable.Top:
			case Collidable.TopLeft:
			case Collidable.TopRight:
				if(collisionTop)
					speed.Y *= factor;
				
				if((collisionLeft && collidableSides == Collidable.TopLeft) ||
				    (collisionRight && collidableSides == Collidable.TopRight))
					speed.X *= factor;
				break;
			case Collidable.None:
			default:
				break;
			}
		}
		
		public static int Height { get { return 32; } }

		public static int Width { get { return 32; } }
		
		public static System.Collections.Generic.List<Tile> Collisions = new System.Collections.Generic.List<Tile>();
		
		public static void Loader(string filepath, ref Vector2 playerPos, Scene scene)
		{
			Vector2 pos = Vector2.Zero;
			Tile t = null;
			// Pause timer
			SceneManager.PauseScene();
			
			// Clear collision list
			Collisions.Clear();			
			// Read whole level files to lines
			var lines = System.IO.File.ReadAllLines(filepath);
			// Make SpriteLists to improve efficiency
			var tiles = new SpriteList(TextureManager.Get("tiles"));
			var entities = new SpriteList(TextureManager.Get("entities"));
			// Iterate end to start, line by line
			for(int i = lines.Length - 1; i >= 0; i--)
			{
				// New row: reset x position and read next line.
				pos.X = 0;
				var line = lines[i].ToUpper();
				foreach(char c in line)
				{
					if(c == ' ')
					{
						// Move to next tile in "grid"
						pos.X += Width;
						continue;
					}
					// Make/add tile at pos
					t = new Tile(c, pos);
					tiles.AddChild(t);
					
					// If has collision add to list
					if(t.collidableSides != Collidable.None || c == 'Z')
						Collisions.Add(t);
					
					// Player start, pass position
					if(c == 'S')
						playerPos = pos;
					
					// If floor, chance to spawn enemy
					if(c == 'X' && Info.Rnd.Next(0, 11) == 1)
					{
						EntityAlive e = new EntityAlive(Info.Rnd.Next(1, 10), pos, new Vector2i(0, 1));
						entities.AddChild(e);
					}
					
					if(c == 'H')
					{
						EntityAlive e = new EntityAlive(new Vector2i(Info.Rnd.Next(5), 13), pos);
						entities.AddChild(e);
						e.IsBoss = true;
						e.Stats.Health = 200;
						e.Stats.Lives = 5;
						e.Stats.Defense = 100;
						e.Stats.Attack = 30;
						e.Stats.RangedAttack = 20;
					}
					
					// End col: Move to next tile "grid"
					pos.X += Width;
				}
				// End row: Move y position to next tile row 
				pos.Y += Height;
			}
			// Add Tiles to Scene
			scene.AddChild(tiles);
			// Add Entites to Scene
			scene.AddChild(entities);
			// Player has position, add player last to scene
			if(!playerPos.IsZero())
				scene.AddChild(new Player(playerPos));
			
			// Resume Timers
			SceneManager.ResumeScene();
		}
	}
}
