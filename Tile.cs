using System.Linq;
using System.Xml;
using System.Xml.Linq;
using System.Collections.Generic;
using Sce.PlayStation.Core;
using Sce.PlayStation.Core.Graphics;
using Sce.PlayStation.HighLevel.GameEngine2D;
using Sce.PlayStation.HighLevel.GameEngine2D.Base;

namespace TheATeam
{
	public struct TileType
	{
		public char Key;
		public Vector2i TileIndex2D;
		public bool IsCollidable;
	}
	
	public class Stats
	{
		public int MaxHealth = 1;
		public int MaxLives = 3;
		public int Health = 1;
		public int Lives = 3;
	}

	public class Tile: SpriteTile
	{
		private static Dictionary<char, TileType> Types = new Dictionary<char, TileType>();
		private char _key;

		private bool _isWall { get { return Elements.Contains(_key); } }

		private Stats Stats = new Stats();
		public static List<char> Elements = new List<char> {'N', 'W', 'F'};
		public static List<Tile> Collisions = new List<Tile>();
		public static List<List<Tile>> Grid = new List<List<Tile>>();

		public static int Height { get { return 64; } }

		public static int Width { get { return 64; } }

		public bool IsCollidable { get; set; }

		public char Key { get { return _key; } set { LoadTileProperties(value); } }

		public Tile(Vector2 position): base()
		{
			Position = position;
			TextureInfo = TextureManager.Get("tiles");
			Quad.S = TextureInfo.TileSizeInPixelsf;
			IsCollidable = false;
		}

		public Tile(char loadKey, Vector2 position): this(position)
		{
			Key = loadKey;
		}
		
		private void LoadTileProperties(char loadKey)
		{
			_key = loadKey;
			TileType tt = new TileType();
			if(Types.TryGetValue(loadKey, out tt))
			{
				TileIndex2D = tt.TileIndex2D;
				IsCollidable = tt.IsCollidable;
			}
		}
		
		public bool WallDamage()
		{			
			if(!_isWall)
				return false;
			
			if(Stats.Lives > 0)
			{
				if(Stats.Health > Stats.MaxHealth && Stats.Lives < Stats.MaxLives)
				{
					Stats.Lives++;
					Stats.Health = Stats.MaxHealth;
				}
				
				if(Stats.Health <= 0)
				{
					Stats.Lives--;
					Stats.Health = Stats.MaxHealth;
				}
				
				int newTileIndex = Stats.MaxLives - Stats.Lives;
				if(newTileIndex != TileIndex2D.X && newTileIndex < TextureInfo.NumTiles.X)
				{
					TileIndex2D.X = newTileIndex;
				}
			}
			else
			{
				Key = 'E';
				return true;
			}
			return false;
		}
		
		public void TakeDamage(char element='N', int damage=1)
		{
			if(Stats.Lives > 0 && _isWall)
			{
				Stats.Health += damage * (element == Key ? 1 : -1);
			}
		}

		public bool Overlaps(SpriteBase sprite)
		{
			Vector2 boundsScale = new Vector2(0.95f);
			
			Bounds2 thisBounds = this.GetlContentLocalBounds();
			Bounds2 otherBounds = sprite.GetlContentLocalBounds();
			this.GetContentWorldBounds(ref thisBounds);
			sprite.GetContentWorldBounds(ref otherBounds);
			thisBounds = thisBounds.Scale(boundsScale, thisBounds.Center);
			return thisBounds.Overlaps(otherBounds);
		}
		
		public static Vector2i LoadSpriteIndex(char loadkey)
		{
			if(Types.Count < 1)
				XMLTypeLoader("/Application/assets/tiles.xml");
			
			TileType tt = new TileType();
			if(Types.TryGetValue(loadkey, out tt))
			{
				return tt.TileIndex2D;
			}
			return new Vector2i();
		}
		
		private static void XMLTypeLoader(string filepath)
		{
			// Read whole level xml to doc
			var doc = XDocument.Load(filepath);
			// Assign attributes to anonymous type with LINQ
			var lines = from tiletype in doc.Root.Elements("tiletype")
				select new {
					X = (int)tiletype.Attribute("tx"),
					Y = (int)tiletype.Attribute("ty"),
					Key = char.Parse(tiletype.Attribute("k").Value.ToUpper()),
					IsCollidable = (bool)tiletype.Attribute("c")
				};
			// Loop through anonymous types and assign to TileType
			TileType tt = new TileType();
			foreach(var line in lines)
			{
				tt.Key = line.Key;
				tt.TileIndex2D = new Vector2i(line.X, line.Y);
				tt.IsCollidable = line.IsCollidable;

				Types.Add(line.Key, tt);
			}
		}

		public static void Loader(string filepath, ref Vector2 player1Pos, ref Vector2 player2Pos, Scene scene)
		{
			Vector2 pos = Vector2.Zero;
			Tile t = null;
			// Pause timer
			SceneManager.PauseScene();
			// Load types if empty
			if(Types.Count < 1)
				XMLTypeLoader("/Application/assets/tiles.xml");
			// Clear collision list
			Collisions.Clear();
			// Read whole level files to lines
			var lines = System.IO.File.ReadAllLines(filepath);
			// Make SpriteLists to improve efficiency
			var tiles = new SpriteList(TextureManager.Get("tiles"));
			for(int i = lines.Length - 1; i >= 0; i--)
			{
				// New row: reset x position
				pos.X = 0;
				// Read next line in caps, just in case
				var line = lines[i].ToUpper();
				// Make empty list for new row
				var gridLine = new List<Tile>();

				foreach(char c in line)
				{
					// Makes a wall or tile
					t = new Tile(c, pos);
					// Add to SpriteList for drawing					
					tiles.AddChild(t);
					// Add to Tile Grid
					gridLine.Add(t);

					// If has collision add to collisions checklist
					if(t.IsCollidable)
						Collisions.Add(t);

					// Player 1 start position
					if(c == '1')
						player1Pos = pos;
					
					// Player 2 start
					if(c == '2')
						player2Pos = pos;

					// End col: Move to next tile "grid"
					pos.X += Width;
				}
				Grid.Add(gridLine);
				// End row: Move y position to next tile row
				pos.Y += Height;
			}

			// Add Tiles to Scene
			scene.AddChild(tiles);
			// Resume Timers
			SceneManager.ResumeScene();
		}
	}
}
