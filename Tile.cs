using System.Linq;
using System.Xml.Linq;
using System.Collections.Generic;
using Sce.PlayStation.Core;
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
		private int initialMaxHealth, initialMaxMana, initialMaxShield;
		
		public Stats(int maxHealth=30, int maxMana=100, int maxShield=0)
		{
			initialMaxHealth = MaxHealth = maxHealth;
			initialMaxMana = MaxMana = maxMana;
			initialMaxShield = MaxShield = maxShield;
		}
		
		public void Reset()
		{
			MaxHealth = initialMaxHealth;
			MaxMana = initialMaxMana;
			MaxShield = initialMaxShield;
			manaCost = 30;
			shieldRecharge = 85;
			manaRecharge = 25;
			healthRecharge = 180;
			moveSpeed = 1f;
		}
	}
	
	public class Tile: SpriteTile
	{
		private static Dictionary<char, TileType> _types = XMLTypeLoader();
		protected static Vector2 boundsScale = new Vector2(40 / 64f, 1f);
		public static TextureInfo TexInfo = TextureManager.Get("tiles");
		public static List<char> Elements = new List<char> {'N', 'W', 'F', 'E', 'A', 'L'};
		public static List<Tile> Collisions = new List<Tile>();
		public static List<List<Tile>> Grid = new List<List<Tile>>();
		private char _key;
		private float healthTimer = 0.0f;
		protected Stats _stats = new Stats();

		public bool IsWall { get { return Elements.Contains(_key); } }

		public int Height { get { return (int)LocalBounds.Size.Y; } }

		public int Width { get { return (int)LocalBounds.Size.X; } }
		
		public Vector2 Center { get { return Position + Quad.Center; } }

		public bool IsCollidable { get; set; }
		
		public bool IsRegenerative { get; set; }

		public char Key { get { return _key; } set { LoadTileProperties(value); } }
		
		public bool IsAlive { get { return _stats.health > 0; } }
		
		public Bounds2 LocalBounds { get { return Quad.Bounds2().Scale(boundsScale, Quad.Center); } }
		
		public Bounds2 WorldBounds
		{
			get
			{
				Bounds2 thisBounds = default(Bounds2);
				if (GetContentWorldBounds(ref thisBounds))
				{
					thisBounds = thisBounds.Scale(boundsScale, thisBounds.Center);
				}
				else
				{
					System.Diagnostics.Debug.WriteLine("Failed to get world bounds");
				}
				return thisBounds;
			}
		}

		public Tile(Vector2 position): base()
		{
			Position = position;
			IsCollidable = false;
		}

		public Tile(char loadKey, Vector2 position): this(position)
		{
			TextureInfo = TexInfo;
			Quad.S = TextureInfo.TileSizeInPixelsf;
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
			IsRegenerative = false;
		}
		
		public bool WallDamage(float dt)
		{			
			if (!IsWall)
			{
				return false;
			}
			
			if (IsAlive)
			{
				RegenTile(dt);
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
		
		public void RegenTile(float dt)
		{
			if (IsRegenerative)
			{
				if (_stats.health < _stats.MaxHealth)
				{
					healthTimer += dt;
				}
				if (healthTimer >= _stats.healthRecharge)
				{
					_stats.health++;
					healthTimer = 0.0f;
				}
			}
		}

		public void TakeDamage(char element='N', int damage=10)
		{
			if (IsAlive && IsWall)
			{
				if (element == 'N')
				{
					_stats.health -= damage;
				}
				else
				{
					_stats.health += damage * (element == Key ? 1 : -1);
				}
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
			Tile t = sprite as Tile;
			Bounds2 otherBounds = default(Bounds2);
			if (t != null)
			{
				otherBounds = t.WorldBounds;
			}
			else
			{
				sprite.GetContentWorldBounds(ref otherBounds);
			}
			return Overlaps(otherBounds);
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
		
		public static void Loader(string filepath, ref Vector2 player1Pos, ref Vector2 player2Pos, 
		                          ref Vector2 p1Flag, ref Vector2 p2Flag, Scene scene)
		{
			Vector2 pos = Vector2.Zero;
			Tile t = null;
			const int TileSize = 64;
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
						player2Pos = new Vector2(pos.X + TileSize, pos.Y);
					}
					
					// Player 1 flag position
					if (c == '3')
					{
						p1Flag = new Vector2(pos.X + TileSize / 2, pos.Y + TileSize / 2);
					}
					
					// Player 2 flag position
					if (c == '4')
					{
						p2Flag = new Vector2(pos.X + TileSize / 2, pos.Y + TileSize / 2);
					}

					// End col: Move to next tile "grid"
					pos.X += TileSize;
				}
				Grid.Add(gridLine);
				// End row: Move y position to next tile row
				pos.Y += TileSize;
			}

			// Add Tiles to Scene
			scene.AddChild(tiles);
		}
	}
}
