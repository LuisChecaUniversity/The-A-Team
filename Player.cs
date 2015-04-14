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
	
	public enum ShieldEffect { None = 2, Damage = 1, KnockBack = 0 }

	public class Player: Tile
	{
		private static int Y_INDEX = 5;
		private static float MoveDelta = 4f;
		private static float PlayerSize = 64; // 64x64 px
		private static float UISize = 32;
		new static Vector2 boundsScale = new Vector2(1f);
		protected bool canShoot = true;
		private bool keyboardTest = true;
		private char _element, _element2;
		protected Vector2 Direction;
		protected Vector2 ShootingDirection;
		public PlayerIndex whichPlayer;
		protected PlayerState playerState;

		public int Health { get { return _stats.health; } }

		public int Shield { get { return _stats.shield; } }

		public int Mana { get { return _stats.mana; } }
		
		private float manaTimer, healthTimer, shieldTimer, slowTimer;
		private Vector2 startingPosition;
		private Vector2 positionDelta;
		private Vector2i animationRangeX;
		protected bool slowed = false;
		private SpriteTile elementRing = null;
		private SpriteTile elementShield = null;
		
		private bool ShieldCollidable { get { return elementShield.TileIndex2D.Y < (int)ShieldEffect.None; } }
		
		private ShieldEffect ShieldEffect { get { return (ShieldEffect)elementShield.TileIndex2D.Y; } }
		
		private float ShieldScale { get { return (_stats.MaxShield > 0) ? _stats.shield / (float)_stats.MaxShield : 1f; } }
		
		private bool ShieldVisible
		{
			get { return (elementShield != null) ? elementShield.Visible : false; }
			set
			{ 
				if (elementShield != null)
				{
					elementShield.Visible = value;
				}
			}
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
		
		public List<Tile> playerTiles = new List<Tile>();

		private Player(int spriteIndexY, Vector2 position, Vector2i animationRangeX, float interval=0.2f):
			base(position)
		{
			TextureInfo = TextureManager.Get("players");
			Quad.S = TextureInfo.TileSizeInPixelsf;
			
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
			elementShield.TileIndex2D.Y = (int)ShieldEffect.None;
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
			
			Position = Position + positionDelta;
			
			switch (AppMain.TYPEOFGAME)
			{
			case "DUAL":
				HandleInput(dt);
				break;
			case "SINGLE":
				
				// Handle movement/attacks
				HandleInput(dt);
					
				// Apply the movement
							
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
			if(whichPlayer == PlayerIndex.PlayerOne)
			{
				positionDelta.X = Input2.GamePad0.AnalogLeft.X * 2.0f * _stats.moveSpeed;
				positionDelta.Y = -Input2.GamePad0.AnalogLeft.Y * 2.0f * _stats.moveSpeed;

				if (ShootingDirection.IsZero())
				{
					ShootingDirection = Direction;
				}
				if (!positionDelta.IsZero())
				{
					Direction = positionDelta.Normalize();
				}
				if (Input2.GamePad0.Up.Down)// .R.Down)
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
			else if(whichPlayer == PlayerIndex.PlayerTwo)
			{
				positionDelta.X = Input2.GamePad0.AnalogRight.X * 2.0f * _stats.moveSpeed;
				positionDelta.Y = -Input2.GamePad0.AnalogRight.Y * 2.0f * _stats.moveSpeed;

				if (ShootingDirection.IsZero())
				{
					ShootingDirection = Direction;
				}
				if (!positionDelta.IsZero())
				{
					Direction = positionDelta.Normalize();
				}
				if (Input2.GamePad0.Triangle.Down)// .R.Down)
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
			
			//no movement for dual player - Didnt think we continuing with this feature
//			if (whichPlayer == PlayerIndex.PlayerOne)
//			{
//				if (Input2.GamePad0.Up.Down)
//				{
//					if (canShoot)
//					{
//						Shoot();
//					}
//				}
//				if (Input2.GamePad0.Up.Release)
//				{
//					canShoot = true;
//				}
//			}
//			else
//			{
//				if (Input2.GamePad0.Triangle.Down)
//				{
//					if (canShoot)
//					{
//						Shoot();
//					}
//				}
//				if (Input2.GamePad0.Triangle.Release)
//				{
//					canShoot = true;
//				}
//			}
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
			// Loop through enemy tiles
			Player p = Info.P1 == this ? Info.P2 : Info.P1;
			foreach (Tile t in p.playerTiles)
			{
				if (t.Key != '_' && t.Overlaps(this))
				{
					// Fire + Earth -> Collsion with Walls cause damage
					if ((p.Element == 'F' && p.Element2 == 'E') || (p.Element2 == 'F' && p.Element == 'E'))
					{
						TakeDamage(1);
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
				ShootingDirection.Normalize();
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
			// Single Element buffs
			switch (element)
			{
			case "Neutral":
				// reset all buffs
				_stats.Reset();
				elementShield.TileIndex2D.Y = (int)ShieldEffect.None;
				ShieldVisible = false;
				break;
			case "Earth":
				// More health tiles, implemented in LoadTileProperties()
				break;
			case "Fire":
				// More bullet damage, implemented in Projectile.BulletDamage
				break;
			case "Water":
				// Adds a shield
				_stats.MaxShield = _stats.MaxHealth;
				elementShield.TileIndex2D.Y = (int)ShieldEffect.None;
				ShieldVisible = true;
				break;
			case "Air":
				// Speed boost
				_stats.moveSpeed = 1.5f;
				break;
			case "Lightning":
				// Increased mana regen
				_stats.manaRecharge = 15;
				break;
			}
			// Combined Elements buffs
			
			// Water + Fire -> Damaging shield
			if ((Element == 'W' && Element2 == 'F') || (Element == 'F' && Element2 == 'W'))
			{
				elementShield.TileIndex2D.Y = (int)ShieldEffect.Damage;
			}
			// Water + Earth -> Stronger shield
			if ((Element == 'W' && Element2 == 'E') || (Element == 'E' && Element2 == 'W'))
			{
				_stats.MaxShield = _stats.MaxHealth * 2;
			}
			// Water + Air -> KnockBack shield
			if ((Element == 'W' && Element2 == 'A') || (Element == 'A' && Element2 == 'W'))
			{
				elementShield.TileIndex2D.Y = (int)ShieldEffect.KnockBack;
			}
			// Water + Lightning -> Increased shield regen
			if ((Element == 'W' && Element2 == 'L') || (Element2 == 'W' && Element == 'L'))
			{
				_stats.shieldRecharge = 45;
			}
			// Earth + Lightning -> Wall HP Regen
			if ((Element == 'E' && Element2 == 'L') || (Element2 == 'E' && Element == 'L'))
			{
				foreach (Tile t in Tile.Collisions)
				{ 
					t.IsRegenerative = true;
				}
			}
			// Earth + Air -> Tiles Grant Speed Boost
			if ((Element == 'E' && Element2 == 'A') || (Element2 == 'E' && Element == 'A'))
			{
				_stats.moveSpeed = 2f;
			}
			// Fire + Earth -> Collsion with Walls cause damage, implemented in HandleCollisions()
			
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
				ItemManager.Instance.ResetItems(this);
				Position = startingPosition;
				_stats.health = _stats.MaxHealth;
				_stats.mana = _stats.MaxMana;
				_stats.moveSpeed = 1.0f;
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
			elementShield.Scale = new Vector2(ShieldScale);
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
		
		public void ShieldCollision(Player p)
		{
			if (ShieldCollidable && ShieldVisible)
			{
				if (p.Overlaps(elementShield))
				{
					switch (this.ShieldEffect)
					{
					case ShieldEffect.Damage:
						p.TakeDamage(1);
						break;
					case ShieldEffect.KnockBack:
						p.Position = p.Position - (new Vector2(50) * p.Direction);
						break;						
					case ShieldEffect.None:
					default:
						break;
					}
				}
			}
		}
	}
}
