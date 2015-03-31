using System;
using System.Collections.Generic;
using Sce.PlayStation.Core;
using Sce.PlayStation.HighLevel.GameEngine2D;
using Sce.PlayStation.HighLevel.GameEngine2D.Base;

namespace TheATeam
{
	public enum PlayerIndex
	{
		PlayerOne = 1,
		PlayerTwo = 2,
	}

	public enum PlayerState
	{
		Idle,
		Moving,
		Shooting,
	}

	public class Player: Tile
	{
		private static int Y_INDEX = 5;
		private static float MoveDelta = 4f;
		private static float PlayerSize = 64; // 64x64 px
		private static float UISize = 32;
		protected bool canShoot = true;
		private bool keyboardTest = true;
		public bool PlayerAlive = true;
		private char _element, _element2;
		protected Vector2 Direction;
		protected Vector2 ShootingDirection;
		private PlayerIndex whichPlayer;
		protected PlayerState playerState;

		public int Health { get { return _stats.health; } }

		public int Shieldhp { get { return _stats.shield; } }

		public int Mana { get { return _stats.mana; } }
		
		private float shieldScale { get { return (_stats.MaxShield > 0) ? _stats.shield / (float)_stats.MaxShield : 1f; } }

		private float manaTimer, healthTimer, shieldTimer, slowTimer;
		private Vector2 startingPosition;
		private Vector2 positionDelta;
		private Vector2i animationRangeX;
		protected bool slowed = false;
		private SpriteTile elementRing = null;
		private SpriteTile elementShield = null;
		
		// Player Tiles
		public List<Tile> playerTiles = new List<Tile>();
		
		public bool ShieldVisible
		{
			get { return (elementShield != null) ? elementShield.Visible : false; }
			protected set { if (elementShield != null)
				{
					elementShield.Visible = value;
				} }
		}
		
		public char Element
		{
			get { return _element; }
			set
			{
				_element = value;
				TileIndex2D.Y = Y_INDEX - Tile.Elements.IndexOf(value);
			}
		}
		
		public char Element2
		{
			get { return _element2; }
			set
			{
				_element2 = value;
				elementRing.TileIndex2D.Y = 5 - Tile.Elements.IndexOf(value);
			}
		}

		private Player(int spriteIndexY, Vector2 position, Vector2i animationRangeX, float interval=0.2f):
			base(position)
		{
			TextureInfo = TextureManager.Get("players");
			
			// Assign variables
			this.animationRangeX = animationRangeX;
			TileIndex2D = new Vector2i(animationRangeX.X, spriteIndexY);
			
			// Attach custom animation function
			ScheduleInterval((dt) => {
				if (IsAlive)
				{
					int newTileIndex = TileIndex2D.X < animationRangeX.Y ? TileIndex2D.X + 1 : animationRangeX.X;
					TileIndex2D.X = newTileIndex;
				}
				else
				{
					TileIndex2D.X = TextureInfo.NumTiles.X - 1;
					UnscheduleUpdate();
				}
			}, interval);
			
			// Second element ring sprite
			elementRing = new SpriteTile(TextureManager.Get("rings"));
			elementRing.Quad.S = elementRing.TextureInfo.TileSizeInPixelsf;
			elementRing.CenterSprite();			
			// Shield aura sprite
			elementShield = new SpriteTile(TextureManager.Get("shields"));
			elementShield.Quad.S = elementShield.TextureInfo.TileSizeInPixelsf;
			elementShield.TileIndex2D.Y = elementShield.TextureInfo.NumTiles.Y - 1;
			elementShield.Visible = false;
			elementShield.CenterSprite();
			
			AddChild(elementRing);
			AddChild(elementShield);
		}
		
		public Player(Vector2 position, bool isPlayer1, List<Tile> tiles):
			this(Y_INDEX, position, new Vector2i(0, 3))
		{
			_stats = new Stats(122, 122);
			startingPosition = position;
			Element = 'N';
			Element2 = 'N';
			ShieldVisible = false;

			CenterSprite();	

			whichPlayer = isPlayer1 ? PlayerIndex.PlayerOne : PlayerIndex.PlayerTwo;
			
			playerState = PlayerState.Idle;
			Direction = new Vector2(1.0f, 0.0f);
			ShootingDirection = new Vector2(1.0f, 0.0f);
			playerTiles = tiles;
		}
		
		override public void Update(float dt)
		{
			base.Update(dt);
			UpdateMana(dt);
			UpdateShield(dt);
			RegenHealth(dt);
			SlowEffect(dt);
			
			switch (AppMain.TYPEOFGAME)
			{
			case "DUAL":
			case "SINGLE":
				
				// Handle movement/attacks
				HandleInput(dt);
					
				// Apply the movement
				Position = Position + positionDelta;			
				break;
				
			case "MULTIPLAYER":
				if (AppMain.ISHOST && whichPlayer == PlayerIndex.PlayerOne || !AppMain.ISHOST && whichPlayer == PlayerIndex.PlayerTwo)
				{
					// Handle movement/attacks
					HandleInput(dt);

					// Apply the movement
					Position = Position + positionDelta;
					// Make camera follow the player
					Info.CameraCenter = Position;
					//Set Position for Data Message
					AppMain.client.SetMyPosition(Position.X, Position.Y);
				}
				else
				{
					//set position and direction from the network positions of enemy
					Position = AppMain.client.networkPosition;
					Direction = AppMain.client.NetworkDirection;
					if (AppMain.client.HasShot)
					{
						Shoot();	
						AppMain.client.SetHasShot(false);	
					}
				}
			
				break;
			default:
				break;
			}
			// Rotate player based on his direction
			HandleDirectionAnimation();
			
			// Find current tile and apply collision
			HandleCollision();
	
			// Make camera follow the player
			Info.CameraCenter = Position;
		}
		
		private void HandleInput(float dt)
		{
			switch (AppMain.TYPEOFGAME)
			{
			case "SINGLE":
				SingleUpdate(dt);
				break;
			case "MULTIPLAYER":
				MultiplayerUpdate(dt);
				break;
			case "DUAL":
				DualUpdate(dt);
				break;
			default:
				break;
			}
			
			if (keyboardTest)
			{
				if (Input2.GamePad0.Left.Down)
				{
					positionDelta.X = -MoveDelta * _stats.moveSpeed;
				}
	
				if (Input2.GamePad0.Right.Down)
				{
					positionDelta.X = MoveDelta * _stats.moveSpeed;	
				}
				
				if (Input2.GamePad0.Up.Down)
				{
					positionDelta.Y = MoveDelta * _stats.moveSpeed;	
				}
				
				if (Input2.GamePad0.Down.Down)
				{
					positionDelta.Y = -MoveDelta * _stats.moveSpeed;	
				}
			}
			
			if (!positionDelta.IsZero())
			{
				Direction = positionDelta.Normalize();
				ShootingDirection = Direction;
			}
		}
		
		private void SingleUpdate(float dt)
		{
			positionDelta.X = Input2.GamePad0.AnalogLeft.X * 2.0f * _stats.moveSpeed;
			positionDelta.Y = -Input2.GamePad0.AnalogLeft.Y * 2.0f * _stats.moveSpeed;
			ShootingDirection.X = Input2.GamePad0.AnalogRight.X;
			ShootingDirection.Y = -Input2.GamePad0.AnalogRight.Y;
			if (ShootingDirection.IsZero())
			{
				ShootingDirection = Direction;
			}
			if (!positionDelta.IsZero())
			{
				Direction = positionDelta.Normalize();
			}
			if (Input2.GamePad0.R.Down)
			{
				if (canShoot)
				{
					Shoot();
				}
			}
			if (Input2.GamePad0.R.Release)
			{
				canShoot = true;
			}
		}
		
		private void MultiplayerUpdate(float dt)
		{
			positionDelta.X = Input2.GamePad0.AnalogLeft.X * 2.0f * _stats.moveSpeed;
			positionDelta.Y = -Input2.GamePad0.AnalogLeft.Y * 2.0f * _stats.moveSpeed;
			if (positionDelta.IsZero())
			{
				AppMain.client.SetActionMessage('I');
			}
			else
			{
				AppMain.client.SetActionMessage('M');
				Direction = positionDelta;
				AppMain.client.SetMyDirection(Direction.X, Direction.Y);
			}			
			if (Input2.GamePad0.R.Down)
			{
				if (canShoot)
				{
					Shoot();
				}
			
			}
			if (Input2.GamePad0.R.Release)
			{
				canShoot = true;
			}
		}
		
		private void DualUpdate(float dt)
		{
			//no movement for dual player - Didnt think we continuing with this feature
			if (whichPlayer == PlayerIndex.PlayerOne)
			{
				if (Input2.GamePad0.Up.Down)
				{
					if (canShoot)
					{
						Shoot();
					}
				}
				if (Input2.GamePad0.Up.Release)
				{
					canShoot = true;
				}
			}
			else
			{
				if (Input2.GamePad0.Triangle.Down)
				{
					if (canShoot)
					{
						Shoot();
					}
				}
				if (Input2.GamePad0.Triangle.Release)
				{
					canShoot = true;
				}
			}
		}
		
		protected void HandleDirectionAnimation()
		{
			// Convert direction into angle
			if (!Direction.IsZero())
			{
				RotationNormalize = Direction;
			}
			
			// Set frame to start of animation range if outside of range
			if (TileIndex2D.X < animationRangeX.X || TileIndex2D.X > animationRangeX.Y)
			{
				TileIndex2D.X = animationRangeX.X;
			}
		}
		
		protected void HandleCollision()
		{
			Vector2 nextPos = Position + positionDelta;
			float screenWidth = Director.Instance.GL.Context.Screen.Width;
			float screenHeight = Director.Instance.GL.Context.Screen.Height - UISize; // Blank space for UI.

			if (nextPos.X + PlayerSize > screenWidth + 50)
			{
				Position = new Vector2(screenWidth + 50 - PlayerSize, Position.Y);
			}

			if (nextPos.X < 18)
			{
				Position = new Vector2(18, Position.Y);
			}
			
			if (nextPos.Y < 18)
			{
				Position = new Vector2(Position.X, 18);
			}

			if (nextPos.Y + PlayerSize > screenHeight + 50)
			{
				Position = new Vector2(Position.X, screenHeight + 50 - PlayerSize);
			}

			// Loop through tiles
			foreach (Tile t in Tile.Collisions)
			{
				bool fromLeft = nextPos.X + PlayerSize > t.Position.X + 64;
				bool fromRight = nextPos.X < t.Position.X + Tile.Width;
				bool fromTop = nextPos.Y < t.Position.Y + 18 + Tile.Height;
				bool fromBottom = nextPos.Y + PlayerSize > t.Position.Y + 50;
				
				if (fromLeft && fromRight && fromTop && fromBottom)
				{
					if (!positionDelta.IsZero() && t.IsCollidable && (t.Key != Element || t.Key == 'N'))
					{
						if (fromLeft && positionDelta.X > 0)
						{
							Position = new Vector2(t.Position.X + 64 - PlayerSize, Position.Y);
						}
						else if (fromRight && positionDelta.X < 0)
						{
							Position = new Vector2(t.Position.X + PlayerSize, Position.Y);
						}
						else if (fromTop && positionDelta.Y < 0)
						{
							Position = new Vector2(Position.X, t.Position.Y + 18 + PlayerSize);
						}
						else if (fromBottom && positionDelta.Y > 0)
						{
							Position = new Vector2(Position.X, t.Position.Y + 50 - PlayerSize);
						}
						positionDelta = Vector2.Zero;
					}
				}
			}
		}
		
		virtual public void Shoot()
		{
			if (_stats.mana >= _stats.manaCost)
			{
				_stats.mana -= _stats.manaCost;
				if (AppMain.TYPEOFGAME.Equals("MULTIPLAYER"))
				{
					AppMain.client.SetActionMessage('S');
				}
				playerState = PlayerState.Shooting;
				Vector2 pos = new Vector2(Position.X, Position.Y);
				Console.WriteLine("X: " + ShootingDirection.X + " Y: " + ShootingDirection.Y);
				ProjectileManager.Instance.Shoot(this);//pos, ShootingDirection, _element);
				canShoot = false;
			}
		}
		
		public void ChangeTiles(string type)
		{
			foreach (Tile t in playerTiles)
			{
				if (t.Key == 'N')
				{
					t.Key = type[0];
				}
				// Reset tiles before resetting element
				else if (type == "Neutral" & t.Key == Element)
				{
					t.Key = 'N';
				}
			}
		}
		
		public void ElementBuff(string element)
		{
			switch (element)
			{
			case "Neutral":
				// reset all buffs
				_stats.MaxShield = 0;
				_stats.moveSpeed = 1f;
				_stats.manaRecharge = 25;
				_stats.shieldRecharge = 85;
				ShieldVisible = false;
				break;
			case "Earth":
				// More health tiles, implemented in LoadTileProperties()
				break;
			case "Fire":
				// More
				break;
			case "Water":
				// Shield
				_stats.MaxShield = _stats.MaxHealth;
				ShieldVisible = true;
				break;
			case "Air":
				// Speed boost
				_stats.moveSpeed = 2f;
				break;
			case "Lightning":
				// Inc. mana regen
				_stats.manaRecharge = 15;
				break;
			}
		}
		
		public void TakeDamage(int dmg)
		{
			if (_stats.shield > 0)
			{
				_stats.shield -= dmg * 2;
			}
			else if (_stats.health > 0 + dmg)
			{
				_stats.health -= dmg;
			}
			else
			{
				ItemManager.Instance.ResetItems();
				Position = startingPosition;
				_stats.health = _stats.MaxHealth;
				_stats.mana = _stats.MaxMana;
				ElementBuff("Neutral");
				ChangeTiles("Neutral");
				Element = 'N';
				Element2 = 'N';
			}
		}
		
		public void RegenHealth(float dt)
		{
			if ((Element == 'A' && Element2 == 'L') || (Element2 == 'A' && Element == 'L'))
			{
				if (_stats.health < _stats.MaxHealth)
				{
					healthTimer += dt;
					//_stats.health += _stats.healthRecharge;
				}
				if (healthTimer >= _stats.healthRecharge)
				{
					_stats.health++;
					healthTimer = 0.0f;
				}
			}
		}
		
		public void UpdateMana(float dt)
		{
			if (_stats.mana < _stats.MaxMana)
			{
				manaTimer += dt;
			}
			if (manaTimer >= _stats.manaRecharge)
			{
				_stats.mana++;
				manaTimer = 0.0f;
			}
		}
		
		public void UpdateShield(float dt)
		{
			if ((Element == 'W' && Element2 == 'L') || (Element2 == 'W' && Element == 'L'))
			{
				_stats.shieldRecharge = 45;
			}
			else
			{
				_stats.shieldRecharge = 85;
			}
			
			if (_stats.shield < _stats.MaxShield)
			{
				shieldTimer += dt;
			}
			if (shieldTimer >= _stats.shieldRecharge)
			{
				_stats.shield++;
				shieldTimer = 0.0f;
			}
			// Element shield size
			elementShield.Scale = new Vector2(shieldScale);
		}
		
		public void Player1Score()
		{
			Vector2 nextPos1 = Position + positionDelta;
			
			if (nextPos1.X < 64 && nextPos1.X > 0 && nextPos1.Y < 322 && nextPos1.Y > 258)
			{
				Info.IsGameOver = true;
			}
		}
		
		public void Player2Score()
		{
			Vector2 nextPos2 = Position + positionDelta;
			
			if (nextPos2.X < 958 && nextPos2.X > 894 && nextPos2.Y < 322 && nextPos2.Y > 258)
			{
				Info.IsGameOver = true;
			}
		}

		public Vector2 GetShootingDirection()
		{
			return ShootingDirection;
		}
		
		public void isSlowed(bool b)
		{
			slowed = b;
		}
		
		protected void SlowEffect(float dt)
		{
			if (slowed)
			{
				slowTimer += dt / 1000;
				//Console.WriteLine("slowed " + slowTimer);
				_stats.moveSpeed = 0.3f;
				
				if (slowTimer > 2.0f)
				{
					slowed = false;
					slowTimer = 0.0f;
					_stats.moveSpeed = 1.0f;
				}
			}
		}
	}
}
