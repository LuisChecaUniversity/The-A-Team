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

	public class Tile: SpriteTile
	{
		private static Dictionary<char, TileType> Types = new Dictionary<char, TileType>();
		private char _key;
		public static List<Tile> Collisions = new List<Tile>();

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
			ScheduleUpdate();
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
		
		private static void XMLTypeLoader(string filepath)
		{
			// Read whole level xml to doc
			var doc = XDocument.Load(filepath);
			var lines = from tiletype in doc.Root.Elements("tiletype")
				select new {
					X = (int)tiletype.Attribute("tx"),
					Y = (int)tiletype.Attribute("ty"),
					Key = char.Parse(tiletype.Attribute("k").Value.ToUpper()),
					IsCollidable = (bool)tiletype.Attribute("c")
				};
			TileType tt = new TileType();
			foreach(var line in lines)
			{
				tt.Key = line.Key;
				tt.TileIndex2D = new Vector2i(line.X, line.Y);
				tt.IsCollidable = line.IsCollidable;

				Types.Add(line.Key, tt);
			}
		}

		public static void Loader(string filepath, ref Vector2 playerPos, Scene scene)
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
			//var entities = new SpriteList(TextureManager.Get("entities"));
			// Keys to make a wall
			char[] wallKeys = {'N', 'W', 'F'};
			// Iterate end to start, line by line
			for(int i = lines.Length - 1; i >= 0; i--)
			{
				// New row: reset x position
				pos.X = 0;
				// Read next line in caps, just in case
				var line = lines[i].ToUpper();
				// Make empty list for new row
				//var gridLine = new List<Tile>();

				foreach(char c in line)
				{
					if(wallKeys.Contains(c))
					{
						// Make wall at pos
						t = new HealthWall(c, pos);
					}
					else
					{
						// Make tile at pos
						t = new Tile(c, pos);
					}
					// Add to SpriteList for drawing					
					tiles.AddChild(t);
					// Add to Tile Grid
					//gridLine.Add(t);

					// If has collision add to collisions checklist
					if(t.IsCollidable)
						Collisions.Add(t);

					// Player start, pass position
					if(c == 'S')
						playerPos = pos;

					// End col: Move to next tile "grid"
					pos.X += Width;
				}
				//Grid.Add(gridLine);
				// End row: Move y position to next tile row
				pos.Y += Height;
			}

			// Add Tiles to Scene
			scene.AddChild(tiles);
			// Add Entites to Scene
			//scene.AddChild(entities);
			// Player has position, add player last to scene
			if(!playerPos.IsZero())
				scene.AddChild(new Player(playerPos));

			// Resume Timers
			SceneManager.ResumeScene();
		}

		public static void XMLoader(string filepath, ref Vector2 playerPos, Scene scene)
		{
			// Pause timer
			SceneManager.PauseScene();
			// Clear collision list
			Collisions.Clear();

			// Read whole level xml to doc
			var doc = XDocument.Load(filepath);
			var lines = from tt in doc.Root.Elements("tile")
				select new {
				X = (float)tt.Attribute("x"),
				Y = (float)tt.Attribute("y"),
				Key = char.Parse(tt.Attribute("key").Value.ToUpper())
			};

			Vector2 pos = Vector2.Zero;
			Tile t = null;
			// Make SpriteLists to improve efficiency
			var tiles = new SpriteList(TextureManager.Get("tiles"));
			// Make empty list for each row
			List<Tile> gridLine = new List<Tile>();

			// Iterate end to start, line by line
			foreach(var line in lines)
			{
				pos = new Vector2(line.X, line.Y);
				t = new Tile(line.Key, pos);
				// Add to SpriteList for drawing
				tiles.AddChild(t);

				// Add to Grid row
				//gridLine.Add(t);

				// If has collision add to collisions checklist
				if(t.IsCollidable)
					Collisions.Add(t);

				// Player start, pass position
				if(line.Key == 'S')
					playerPos = pos;

				/* Add row to Grid
				if(gridLine.Count == 30)
				{
					Grid.Add(gridLine);
					gridLine.Clear();
				}*/
			}

			// Add Tiles to Scene
			scene.AddChild(tiles);
			// Add Entites to Scene
			//scene.AddChild(entities);
			// Player has position, add player last to scene
			if(!playerPos.IsZero())
				scene.AddChild(new Player(playerPos));

			// Resume Timers
			SceneManager.ResumeScene();
		}
	}
}
