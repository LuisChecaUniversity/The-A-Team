using System.Diagnostics;
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
	
	public enum ShieldEffect
	{
		None = 2,
		Damage = 1,
		KnockBack = 0
	}

	public class Player: Tile
	{
		
		private float updatePosTime = 1.0f;
		private float curUpdatePosTime;
		
		private static int Y_INDEX = 5;
		private static float MoveDelta = 4f;
		new static Vector2 boundsScale = new Vector2(1f);
		protected bool canShoot = true;
		private bool keyboardTest = false;
		private char _element, _element2;
		protected Vector2 Direction;
		protected Vector2 ShootingDirection;
		public PlayerIndex playerIndex;
		protected PlayerState playerState;

		public int Health { get { return _stats.health; } }

		public int Shield { get { return _stats.shield; } }

		public int Mana { get { return _stats.mana; } }
		
		private float manaTimer, healthTimer, shieldTimer, slowTimer, speedTimer = 0.0f;
		private Vector2 startingPosition;
		private Vector2 positionDelta;
		private Vector2i animationRangeX;
		protected bool slowed = false;
		private SpriteTile elementRing = null;
		private SpriteTile elementShield = null;
		
		private bool ShieldCollidable { get { return elementShield.TileIndex2D.Y < (int)ShieldEffect.None; } }
		
		private ShieldEffect ShieldEffect { get { return (ShieldEffect)elementShield.TileIndex2D.Y; } }
		
		public float ShieldScale { get { return (_stats.MaxShield > 0) ? _stats.shield / (float)_stats.MaxShield : 1f; } }
		
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
		
		public List<Tile> playerTiles;

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

			playerIndex = isPlayer1 ? PlayerIndex.PlayerOne : PlayerIndex.PlayerTwo;
			
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

			Position = Position + (positionDelta * dt / 16f);
			
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
				if (AppMain.ISHOST && playerIndex == PlayerIndex.PlayerOne || !AppMain.ISHOST && playerIndex == PlayerIndex.PlayerTwo)
				{
					// Handle movement/attacks
					HandleInput(dt);

					// Apply the movement
					Position = Position + (positionDelta * dt / 16f);
					//Set Position for Data Message
					AppMain.client.SetMyPosition(Position.X, Position.Y);
				}
				else
				{
					Direction = AppMain.client.NetworkDirection;
					//set position and direction from the network positions of enemy
					Position += Direction * _stats.moveSpeed * 2.0f;
					
					curUpdatePosTime += dt;
					if(curUpdatePosTime > updatePosTime)
					{
						Position = AppMain.client.networkPosition;
						curUpdatePosTime = 0.0f;
					}
					
					//Position = AppMain.client.networkPosition;
					
					if (AppMain.client.HasShot)
					{
						Shoot(false);	
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
			HandleCollision(dt);
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
			positionDelta.X = Input2.GamePad0.AnalogLeft.X * MoveDelta * _stats.moveSpeed;
			positionDelta.Y = -Input2.GamePad0.AnalogLeft.Y * MoveDelta * _stats.moveSpeed;
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
			ShootingDirection.X = Input2.GamePad0.AnalogRight.X;
			ShootingDirection.Y = -Input2.GamePad0.AnalogRight.Y;
			if (ShootingDirection.IsZero())
			{
				ShootingDirection = Direction;
			}
			if (positionDelta.IsZero())
			{
				AppMain.client.SetActionMessage('I');
			}
			else
			{
				AppMain.client.SetActionMessage('M');
				Direction = positionDelta.Normalize();
				AppMain.client.SetMyDirection(Direction.X, Direction.Y);
			}			
			if (Input2.GamePad0.R.Down)
			{
				if (canShoot)
				{
					AppMain.client.SetMyShootingDirection(ShootingDirection.X,ShootingDirection.Y);
					Shoot(true);
				}
			
			}
			if (Input2.GamePad0.R.Release)
			{
				canShoot = true;
			}
		}
		
		private void DualUpdate(float dt)
		{
			if (playerIndex == PlayerIndex.PlayerOne)
			{
				positionDelta.X = Input2.GamePad0.AnalogLeft.X * MoveDelta * _stats.moveSpeed;
				positionDelta.Y = -Input2.GamePad0.AnalogLeft.Y * MoveDelta * _stats.moveSpeed;

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
			else if (playerIndex == PlayerIndex.PlayerTwo)
			{
				positionDelta.X = Input2.GamePad0.AnalogRight.X * MoveDelta * _stats.moveSpeed;
				positionDelta.Y = -Input2.GamePad0.AnalogRight.Y * MoveDelta * _stats.moveSpeed;

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

		private bool hasCollidedTile(Tile t)
		{
			float width = Quad.Bounds2().Point11.X;
			float height = Quad.Bounds2().Point11.Y;
			float tileHeight = 64.0f / 2;
			float tileWidth = 40.0f / 2;
			
			if (Center.X + width < t.Center.X - tileWidth)
			{
				return false;
			}
			else if (Center.X - width > t.Center.X + tileWidth)
			{
				return false;
			}
			else if (Center.Y + height < t.Center.Y - tileHeight)
			{
				return false;
			}
			else if (Center.Y - height > t.Center.Y + tileHeight)
			{
				return false;
			}
			else
			{
				return true;
			}
		}
		
		protected void HandleCollision(float dt)
		{
			float width = Quad.Bounds2().Point11.X;
			float height = Quad.Bounds2().Point11.Y;

			
			// check viewport collisions
			if (Center.X + width > Director.Instance.GL.Context.Screen.Width)
			{
				// right side
				Position = new Vector2(Director.Instance.GL.Context.Screen.Width - width, Position.Y);
			}
			if (Center.X - width < 0)
			{
				// left side
				Position = new Vector2(0 + width, Position.Y);
			}
			if (Center.Y + height > Director.Instance.GL.Context.Screen.Height - 32.0f)
			{
				// top
				Position = new Vector2(Position.X, Director.Instance.GL.Context.Screen.Height - height - 32.0f);
			}
			if (Center.Y - height < 0)
			{
				// bottom
				Position = new Vector2(Position.X, 0 + height);
			}
			
			float tileHeight = 64.0f / 2;
			float tileWidth = 40.0f / 2;
			
			// check tile collisions
			foreach (Tile t in Tile.Collisions)
			{
				if (t.IsCollidable && (t.Key != Element || t.Key == 'N'))
				{
					if (hasCollidedTile(t))
					{
						float pushBack = 4.0f;
						if (Center.X + width >= t.Center.X - tileWidth && Center.X - width < t.Center.X - tileWidth && Center.Y < t.Center.Y + tileHeight && Center.Y > t.Center.Y - tileHeight)
						{

							Position = new Vector2(t.Center.X - tileWidth - width - pushBack, Position.Y);
						}
						else if (Center.X - width <= t.Center.X + tileWidth && Center.X + width > t.Center.X + tileWidth && Center.Y < t.Center.Y + tileHeight && Center.Y > t.Center.Y - tileHeight)
						{

							Position = new Vector2(t.Center.X + tileWidth + width + pushBack, Position.Y);
						}
						else if (Center.Y + height >= t.Center.Y - tileHeight && Center.Y - height < t.Center.Y - tileHeight && Center.X - width < t.Center.X + tileWidth && Center.X + width > t.Center.X - tileWidth)
						{

							Position = new Vector2(Position.X, t.Center.Y - tileHeight - height - pushBack);
						}
						else if (Center.Y - height <= t.Center.Y + tileHeight && Center.Y + height > t.Center.Y + tileHeight && Center.X - width < t.Center.X + tileWidth && Center.X + width > t.Center.X - tileWidth)
						{

							Position = new Vector2(Position.X, t.Center.Y + tileHeight + height + pushBack);
						}
						else // delete
						{

//							Vector2 point = Center.Reflect((Center - t.Center).Perpendicular()).Normalize();
//							
//							if(!point.IsNaN())
//								Position = new Vector2(Position.X + point.X, Position.Y + point.Y);
//							positionDelta = Vector2.Zero;
						}
						
					}
				}
			}

			// Loop through enemy tiles
			Player p = Info.P1 == this ? Info.P2 : Info.P1;
			foreach (Tile t in p.playerTiles)
			{
				if (t.IsWall && t.Overlaps(this))
				{
					// Fire + Earth -> Collsion with Walls cause damage
					if ((p.Element == 'F' && p.Element2 == 'E') || (p.Element2 == 'F' && p.Element == 'E'))
					{
						TakeDamage(1);
					}
				}
			}
			
			
			
			if ((Element == 'E' && Element2 == 'A') || (Element2 == 'E' && Element == 'A'))
			{ 
				speedTimer += dt/1000;
				if(speedTimer > 1.75f)
				{
					_stats.moveSpeed = 1.33f;
					speedTimer =   0.0f;
				}
			}
			
			
			foreach (Tile t in playerTiles)
			{
				if (t.Key != '_' && t.Overlaps(this))
				{
					//Earth + Air -> Tiles Grant Speed Boost
					if ((Element == 'E' && Element2 == 'A') || (Element2 == 'E' && Element == 'A'))
					{ 
						_stats.moveSpeed = 1.75f;	
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
				//Console.WriteLine("X: " + ShootingDirection.X + " Y: " + ShootingDirection.Y);
				ProjectileManager.Instance.Shoot(this);//pos, ShootingDirection, _element);
				canShoot = false;
				if(!AudioManager.IsSoundPlaying)
					AudioManager.PlaySound("fire");
			}
		}
		virtual public void Shoot(bool isMeShooting)
		{
			if (_stats.mana >= _stats.manaCost)
			{
				_stats.mana -= _stats.manaCost;
				if (isMeShooting)
				{
					AppMain.client.SetActionMessage('S');
				}
				else
				{
					ShootingDirection = AppMain.client.networkShootDir;
				}
				playerState = PlayerState.Shooting;
				Vector2 pos = new Vector2(Position.X, Position.Y);
				
				ShootingDirection.Normalize();

				ProjectileManager.Instance.Shoot(this);//pos, ShootingDirection, _element);
				canShoot = false;
				if(!AudioManager.IsSoundPlaying)
					AudioManager.PlaySound("fire");
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
				_stats.moveSpeed = 1.33f;
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
				foreach (Tile t in playerTiles)
				{ 
					t.IsRegenerative = true;
				}
			}
			// Earth + Air -> Tiles Grant Speed Boost
//			if ((Element == 'E' && Element2 == 'A') || (Element2 == 'E' && Element == 'A'))
//			{
//				//_stats.moveSpeed = 1.75f;
//			}
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
				_stats.Reset();
				ChangeTiles("Neutral");
				Element = 'N';
				Element2 = 'N';
				ElementBuff("Neutral");
				
				
				slowed = false;
				
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
			Item p1Flag = ItemManager.Instance.GetItem(ItemType.flag, "Player1Flag");
			
			if (p1Flag != null && p1Flag.hasCollided(Position, Quad.Bounds2().Point11))
			{
				Info.Winner = Info.P1;
				Info.IsGameOver = true;
			}
		}
		
		public void Player2Score()
		{
			Item p2Flag = ItemManager.Instance.GetItem(ItemType.flag, "Player2Flag");
			
			if (p2Flag != null && p2Flag.hasCollided(Position, Quad.Bounds2().Point11))
			{
				Info.Winner = Info.P2;
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
				//Debug.WriteLine("slowed " + slowTimer);
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
						p.Position = p.Position - ((p.LocalBounds.Size * 4.0f) * (Center - p.Center).Normalize());
						_stats.shield = 0;
						if(p is AIPlayer)
						{
							AIPlayer t = (AIPlayer)p;
							t.HavePath = false;
						}
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
