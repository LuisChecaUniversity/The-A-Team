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
		private static int Y_INDEX = 4;
		private static float MoveDelta = 4f;
		private static float PlayerSize = 64; // 64x64 px
		private static float UISize = 32;
		private bool canShoot = true;
		private bool keyboardTest = true;
		public bool PlayerAlive = true;
		protected Vector2 Direction;
		public Vector2 ShootingDirection;
		private PlayerIndex whichPlayer;
		private PlayerState playerState;
		public int health = 124;
		public int mana = 124;
		public float manaTimer;
		private int manaCost = 30;
		private int manaRechargeRate = 20;
		private Vector2 startingPosition;
		private Vector2 positionDelta;
		private Vector2i animationRangeX;
		
		//AI variables
		private bool movingLeft = true;
		private bool shooting = false;
		private float fireRate = 600.0f;
		private float curTime = 0.0f;
		private char _element;
		bool isChasing = false;
		bool goingForElement = true;
		
		//Player Tiles
		public List<Tile> playerTiles = new List<Tile>();
		
		public char Element
		{
			get { return _element; }
			set {
				_element = value;
				TileIndex2D.Y = Y_INDEX - Tile.Elements.IndexOf(value);
			}
		}

		private Player(int spriteIndexY, Vector2 position, Vector2i animationRangeX, float interval=0.2f):
			base(position)
		{
			// Assign variables
			this.animationRangeX = animationRangeX;
			TileIndex2D = new Vector2i(animationRangeX.X, spriteIndexY);
			// Attach custom animation function
			ScheduleInterval((dt) => {
				if(IsAlive)
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
		}
		
		public Player(Vector2 position, bool isPlayer1, List<Tile> tiles):
			this(Y_INDEX, position, new Vector2i(0, 3))
		{
			startingPosition = position;
			Element = 'N';

			CenterSprite();

			whichPlayer = isPlayer1 ? PlayerIndex.PlayerOne : PlayerIndex.PlayerTwo;
			
			playerState = PlayerState.Idle;
			Direction = new Vector2(1.0f, 0.0f);
			ShootingDirection = new Vector2(1.0f, 0.0f);
			playerTiles = tiles;
		}
		
		override public void Update(float dt)
		{
	
			// Handle battle
			base.Update(dt);
			updateMana(dt);
			
			switch(AppMain.TYPEOFGAME)
			{
			case "DUAL":
			case "SINGLE":
				
				// Handle movement/attacks
				HandleInput(dt);
					
				// Apply the movement
				Position = Position + positionDelta;			
				break;
				
			case "MULTIPLAYER":
				if(AppMain.ISHOST && whichPlayer == PlayerIndex.PlayerOne || !AppMain.ISHOST && whichPlayer == PlayerIndex.PlayerTwo)
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
					if(AppMain.client.HasShot)
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
			
			if(keyboardTest)
			{
				
				
				if(Input2.GamePad0.Left.Down)
				{
					positionDelta.X = -MoveDelta;
				}
	
				if(Input2.GamePad0.Right.Down)
				{
					positionDelta.X = MoveDelta;	
				}
				
				if(Input2.GamePad0.Up.Down)
				{
					positionDelta.Y = MoveDelta;	
				}
				
				if(Input2.GamePad0.Down.Down)
				{
					positionDelta.Y = -MoveDelta;	
				}

			}
			
			if(!positionDelta.IsZero())
			{
				Direction = positionDelta.Normalize();
				ShootingDirection = Direction;
			}
		}
		
		private void SingleUpdate(float dt)
		{
			positionDelta.X = Input2.GamePad0.AnalogLeft.X * 2.0f;
			positionDelta.Y = -Input2.GamePad0.AnalogLeft.Y * 2.0f;
			ShootingDirection.X = Input2.GamePad0.AnalogRight.X;
			ShootingDirection.Y = -Input2.GamePad0.AnalogRight.Y;
			if(ShootingDirection.IsZero())
			{
				ShootingDirection = Direction;
			}
			if(!positionDelta.IsZero())
			{
				Direction = positionDelta.Normalize();
			}
			if(Input2.GamePad0.R.Down)
			{
				if(canShoot)
					Shoot();
			}
			if(Input2.GamePad0.R.Release)
				canShoot = true;
		}
		
		private void MultiplayerUpdate(float dt)
		{
			positionDelta.X = Input2.GamePad0.AnalogLeft.X * 2.0f;
			positionDelta.Y = -Input2.GamePad0.AnalogLeft.Y * 2.0f;
			if(positionDelta.IsZero())
				AppMain.client.SetActionMessage('I');
			else
			{
				AppMain.client.SetActionMessage('M');
				Direction = positionDelta;
				AppMain.client.SetMyDirection(Direction.X, Direction.Y);
			}			
			if(Input2.GamePad0.R.Down)
			{
				if(canShoot)
					Shoot();
			
			}
			if(Input2.GamePad0.R.Release)
				canShoot = true;
		}
		
		private void DualUpdate(float dt)
		{
			
			//no movement for dual player - Didnt think we continuing with this feature
			if(whichPlayer == PlayerIndex.PlayerOne)
				{
					if(Input2.GamePad0.Up.Down )
					{
						if(canShoot)
							Shoot();
					}
					if(Input2.GamePad0.Up.Release)
						canShoot = true;
				}
				else
				{
					if(Input2.GamePad0.Triangle.Down)
					{
						if(canShoot)
							Shoot();
					}
					if(Input2.GamePad0.Triangle.Release)
						canShoot = true;
				}
				
		}
		
		protected void HandleDirectionAnimation()
		{
			// Convert direction into angle
			if(!Direction.IsZero())
			{
				RotationNormalize = Direction;
			}
			
			// Set frame to start of animation range if outside of range
			if(TileIndex2D.X < animationRangeX.X || TileIndex2D.X > animationRangeX.Y)
				TileIndex2D.X = animationRangeX.X;
		}
		
		protected void HandleCollision()
		{
			Vector2 nextPos = Position + positionDelta;
			float screenWidth = Director.Instance.GL.Context.Screen.Width;
			float screenHeight = Director.Instance.GL.Context.Screen.Height - UISize; // Blank space for UI.
			
			
			if(nextPos.X + PlayerSize > screenWidth + 50)
			{
				Position = new Vector2(screenWidth + 50 - PlayerSize, Position.Y);
			}
			

			if(nextPos.X < 18)
			{
				Position = new Vector2(18, Position.Y);
			}
			
			if(nextPos.Y < 18)
			{
				Position = new Vector2(Position.X, 18);
			}

			if(nextPos.Y + PlayerSize > screenHeight + 50)
			{
				Position = new Vector2(Position.X, screenHeight + 50 - PlayerSize);
			}

			
			
			// Loop through tiles
			foreach(Tile t in Tile.Collisions)
			{

				bool fromLeft = nextPos.X + PlayerSize > t.Position.X + 64;
				bool fromRight = nextPos.X < t.Position.X + Tile.Width;
				bool fromTop = nextPos.Y < t.Position.Y + 18 + Tile.Height;
				bool fromBottom = nextPos.Y + PlayerSize > t.Position.Y + 50;
				
				if(fromLeft && fromRight && fromTop && fromBottom)
				{
					if(!positionDelta.IsZero() && t.IsCollidable && (t.Key != Element || t.Key == 'N'))
					{
						if(fromLeft && positionDelta.X > 0)
						{
							Position = new Vector2(t.Position.X + 64 - PlayerSize, Position.Y);
						}
						else if(fromRight && positionDelta.X < 0)
						{
							Position = new Vector2(t.Position.X + PlayerSize, Position.Y);
						}
						else if(fromTop && positionDelta.Y < 0)
						{
							Position = new Vector2(Position.X, t.Position.Y + 18 + PlayerSize);
						}
						else if(fromBottom && positionDelta.Y > 0)
						{
							Position = new Vector2(Position.X, t.Position.Y + 50 - PlayerSize);
						}
						positionDelta = Vector2.Zero;
					}
				}
			}
		}
		
		public void Shoot()
		{
			if(mana >= manaCost)
			{
				mana -= manaCost;
				if(AppMain.TYPEOFGAME.Equals("MULTIPLAYER"))
					AppMain.client.SetActionMessage('S');
				playerState = PlayerState.Shooting;
				Vector2 pos = new Vector2(Position.X, Position.Y );
				Console.WriteLine("X: " + ShootingDirection.X + " Y: " + ShootingDirection.Y);
				ProjectileManager.Instance.Shoot(pos, ShootingDirection, _element);
				canShoot = false;
			}
			
		}
		
		public void UpdateAI(float dt, Player p)
		{
			HandleDirectionAnimation();
			HandleCollision();
			updateMana(dt);
			
			if(goingForElement)
			{
				Vector2 dir = new Vector2(470.0f,380.0f);
				Vector2 newDir = new Vector2(dir.X - Position.X  ,dir.Y- Position.Y );
				positionDelta = newDir * 0.009f;
				Position += positionDelta;
						Direction = positionDelta;
				if(Position.X - 480.0f < 40.0f && 
				   Position.Y - 390.0f < 40.0f)
				goingForElement = false;
			}
			else if(!isChasing && !goingForElement)
			{
				if(movingLeft)
				{
					if(Position.X > 240)
					{
						positionDelta = new Vector2(-0.05f * dt, 0.0f);
						Position += positionDelta;
						Direction = positionDelta;
					}
					else
					{
						movingLeft = false;	
					}
				}
				else
				{
					if(Position.X < 930)
					{
						positionDelta = new Vector2(0.05f * dt, 0.0f);
						Position += positionDelta;
						Direction = positionDelta;
					}
					else
					{
						movingLeft = true;	
					}
				}
				
				float dist = Vector2.Distance(p.Position, Position);	
				if(dist < 300)
				{
					curTime += dt;
					if(curTime > fireRate)
					{
						Direction = p.Position - Position;
						Direction = Direction.Normalize();
						ShootingDirection = Direction;
						Shoot();
						curTime = 0.0f;	
					}
				}
				if(dist < 50)
					isChasing = true;
			}
			else
			{
			if(!positionDelta.IsZero())	
				Position = Vector2.Lerp(Position,p.Position,0.01f);
			}
		}
		
		public void ChangeTiles(string type)
		{
			if(type.Equals("Fire"))
			{
				foreach(Tile t in playerTiles)
				{
					if(t.Key == 'N')
						t.Key = 'F';
				}
			}
			else if(type.Equals("Water"))
			{
				
				foreach(Tile t in playerTiles)
				{
					if(t.Key == 'N')
						t.Key = 'W';
				}
			}
		}
		
		public void TakeDamage(int dmg)
		{
			if(health > 0 +dmg)
				health-= dmg;
			else  
			{
			    ItemManager.Instance.ResetItems();
				Position = startingPosition;
				health = 124;
				mana = 124;
				Element = 'N';
				
			}
		}
		public void updateMana(float dt)
		{
			if(mana < 124)
			{
				manaTimer += dt;
			}
			if(manaTimer >= manaRechargeRate)
			{
				mana++;
				manaTimer = 0.0f;
			}
		}
		
		public void Player1Score()  
		{
			Vector2 nextPos1 = Position + positionDelta;
			
		  	if(nextPos1.X < 35 && nextPos1.X > 25 && nextPos1.Y < 295 && nextPos1.Y > 285)
			{
				Level.isGameOver = true;
			}
		}
		
		public void Player2Score() 
		{
			Vector2 nextPos2 = Position + positionDelta;
			
			if(nextPos2.X < 931 && nextPos2.X > 921 && nextPos2.Y < 295 && nextPos2.Y > 285)
			{
				Level.isGameOver = true;
			}
		}
	}
}
