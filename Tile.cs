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
		public char key;
		public Vector2i tileIndex2D;
		public bool isCollidable;
	}
	
	public class Stats
	{
		private int _maxHealth, _maxMana, _maxShield;

		public int MaxHealth
		{
			get{ return _maxHealth; }
			set
			{
				_maxHealth = value;
				health = value;
			}
		}

		public int MaxMana
		{
			get{ return _maxMana; }
			set
			{
				_maxMana = value;
				mana = value;
			}
		}

		public int MaxShield
		{
			get{ return _maxShield; }
			set
			{
				_maxShield = value;
				shield = value;
			}
		}

		public int health, mana, shield;
		public int manaCost = 30;
		
		public int shieldRecharge = 85;
		public int manaRecharge = 25;
		public int healthRecharge = 180;
		public float moveSpeed = 1f;
		
		public Stats(int maxHealth=30, int maxMana=100, int maxShield=0)
		{
			MaxHealth = maxHealth;
			MaxMana = maxMana;
			MaxShield = maxShield;
		}
	}
	
	public enum Sides
	{
		None = 0,
		Top = 1,
		Left = 2,
		TopLeft = 3,
		Right = 4,
		TopRight = 5,
		LeftRight = 6,
		TopLeftRight = 7,
		Bottom = 8,
		TopBottom = 9,
		BottomLeft = 10,
		BottomTopLeft = 11,
		BottomRight = 12,
		BottomTopRight = 13,
		BottomLeftRight = 14,
		All = 15
	}

	public class Tile: SpriteTile
	{
		private static Dictionary<char, TileType> _types = XMLTypeLoader();
		protected Stats _stats = new Stats();
		private char _key;

		private bool _isWall { get { return Elements.Contains(_key); } }
		
		public static TextureInfo TexInfo = TextureManager.Get("tiles");
		public static List<char> Elements = new List<char> {'N', 'W', 'F', 'E', 'A', 'L'};
		public static List<Tile> Collisions = new List<Tile>();
		public static List<List<Tile>> Grid = new List<List<Tile>>();

		public static int Height { get { return (int)TexInfo.TileSizeInPixelsf.Y; } }

		public static int Width { get { return (int)TexInfo.TileSizeInPixelsf.X; } }
		
		public Vector2 Center { get { return Position + Quad.Center; } }

		public bool IsCollidable { get; set; }

		public char Key { get { return _key; } set { LoadTileProperties(value); } }
		
		public bool IsAlive { get { return _stats.health > 0; } }
		
		public Sides Sides { get; set; }
		
		public Bounds2 WorldBounds
		{ 
			get
			{
				Vector2 boundsScale = new Vector2(0.95f);
				Bounds2 thisBounds = this.GetlContentLocalBounds();
				this.GetContentWorldBounds(ref thisBounds);
				thisBounds = thisBounds.Scale(boundsScale, thisBounds.Center);
				return thisBounds;
			}
		}

		public Tile(Vector2 position): base()
		{
			Position = position;
			TextureInfo = TexInfo;
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
			if (_types.TryGetValue(loadKey, out tt))
			{
				TileIndex2D = tt.tileIndex2D;
				IsCollidable = tt.isCollidable;
			}
			// Earth buffs tiles health
			if (loadKey == 'E')
			{
				_stats.MaxHealth = 50;
			}
		}
		
		public bool WallDamage()
		{			
			if (!_isWall)
			{
				return false;
			}
			
			if (IsAlive)
			{
				if (_stats.health > _stats.MaxHealth)
				{
					_stats.health = _stats.MaxHealth;
				}
				
				int newTileIndex = (_stats.MaxHealth - _stats.health) / 10;
				if (newTileIndex != TileIndex2D.X && newTileIndex < TextureInfo.NumTiles.X)
				{
					TileIndex2D.X = newTileIndex;
				}
			}
			else
			{
				Key = '_';
				return true;
			}
			return false;
		}
		
		public void TakeDamage(char element='N', int damage=10)
		{
			if (IsAlive && _isWall)
			{
				_stats.health += damage * (element == Key ? 1 : -1);
			}
		}
		
		public bool Overlaps(Bounds2 otherBounds)
		{
			return WorldBounds.Overlaps(otherBounds);
		}
		
		public bool Overlaps(Vector2 point)
		{
			Bounds2 otherBounds = new Bounds2(point);
			return Overlaps(otherBounds);
		}

		public bool Overlaps(SpriteBase sprite)
		{
			Bounds2 otherBounds = sprite.GetlContentLocalBounds();
			sprite.GetContentWorldBounds(ref otherBounds);
			return Overlaps(otherBounds);
		}
		
		public static Vector2i GetSpriteIndex(char loadkey)
		{
			TileType tt = new TileType();
			if (_types.TryGetValue(loadkey, out tt))
			{
				return tt.tileIndex2D;
			}
			return new Vector2i(-1, -1);
		}
		
		private static Dictionary<char, TileType> XMLTypeLoader(string filepath="/Application/assets/tiles.xml")
		{
			// Read whole level xml to doc
			var doc = XDocument.Load(filepath);
			// Assign attributes to anonymous type with LINQ
			Dictionary<char, TileType> types = doc.Descendants("tiletype").ToDictionary(
				desc => char.Parse(desc.Attribute("k").Value.ToUpper()), 
                desc => new TileType {
						tileIndex2D = new Vector2i((int)desc.Attribute("tx"), (int)desc.Attribute("ty")),
						key = char.Parse(desc.Attribute("k").Value.ToUpper()),
						isCollidable = (bool)desc.Attribute("c")
					}
				);
			return types;
		}
		
		public static void Loader(string filepath, ref Vector2 player1Pos, ref Vector2 player2Pos, Scene scene)
		{
			Vector2 pos = Vector2.Zero;
			Tile t = null;
			// Clear Grid list
			Grid.Clear();
			// Clear collision list
			Collisions.Clear();
			// Read whole level files to lines
			var lines = System.IO.File.ReadAllLines(filepath);
			// Make SpriteLists to improve efficiency
			var tiles = new SpriteList(TexInfo);
			for (int i = lines.Length - 1; i >= 0; i--)
			{
				// New row: reset x position
				pos.X = 0;
				// Read next line in caps, just in case
				var line = lines[i].ToUpper();
				// Make empty list for new row
				var gridLine = new List<Tile>();

				foreach (char c in line)
				{
					// Makes a wall or tile
					t = new Tile(c, pos);
					// Add to SpriteList for drawing					
					tiles.AddChild(t);
					// Add to Tile Grid
					gridLine.Add(t);

					// If has collision add to collisions checklist
					if (t.IsCollidable)
					{
						Collisions.Add(t);
					}

					// Player 1 start position
					if (c == '1')
					{
						player1Pos = pos;
					}
					
					// Player 2 start
					if (c == '2')
					{
						player2Pos = pos;
					}

					// End col: Move to next tile "grid"
					pos.X += Width;
				}
				Grid.Add(gridLine);
				// End row: Move y position to next tile row
				pos.Y += Height;
			}

			// Add Tiles to Scene
			scene.AddChild(tiles);
		}
	}
}
