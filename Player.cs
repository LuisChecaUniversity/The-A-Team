using System;
using Sce.PlayStation.Core;
using Sce.PlayStation.Core.Input;
using Sce.PlayStation.HighLevel.GameEngine2D.Base;

namespace TheATeam
{
	public enum PlayerIndex
	{
		PlayerOne = 1,
		PlayerTwo = 2,
	}
	public class Player: EntityAlive
	{
		private static int PLAYER_INDEX = 0;
		private bool canShoot = true;
		private PlayerIndex Index;
		private bool KeyboardTest = true;
		

		

		private Vector2 oldPos;
		
		//AI variables
		private bool movingLeft = true;
		private bool shooting = false;
		private float fireRate = 800.0f;
		private float curTime = 0.0f;
		
		public Player(Vector2 position,bool isPlayer1):

			base(PLAYER_INDEX, position, new Vector2i(0, 1))
		{
			IsDefending = false;
			Stats.Lives = 5;

			if(isPlayer1)
				whichPlayer = PlayerIndex.PlayerOne;
			else
				whichPlayer = PlayerIndex.PlayerTwo;
			
			playerState = PlayerState.Idle;
			Direction = new Vector2(1.0f,0.0f);

		}
		
		override public void Update(float dt)
		{
	
			// Handle battle
			base.Update(dt);
			
			// Handle Death
//			if(!IsAlive)
//				SceneManager.ReplaceUIScene(new DeadUI());
			switch (AppMain.TYPEOFGAME)
			{
			case "SINGLE":
				// Handle movement/attacks
					HandleInput();
					
				// Apply the movement
					Position = Position + MoveSpeed;
			
				break;
				
			case "MULTIPLAYER":
				if(AppMain.ISHOST && whichPlayer == PlayerIndex.PlayerOne || !AppMain.ISHOST && whichPlayer == PlayerIndex.PlayerTwo)
				{
					// Handle movement/attacks
					HandleInput();
					
					// Apply the movement
					Position = Position + MoveSpeed;
					
					//Set Position for Data Message
					AppMain.client.SetMyPosition(Position.X,Position.Y);
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
			// Find current tile and apply collision
			HandleCollision();
			
			
			// Make camera follow the player
			Info.CameraCenter = Position;
			

		}
		
		public void UpdateAI(Player player1)
		{
			
		}
		public AttackStatus Attack { get { return attackState; } }
		
		public Vector2 Direction;
		public Vector2 GetPosition { get { return Position; }}
		private static float MoveDelta = 2f;
		
		private void HandleInput()
		{
			//var gamePadData = GamePad.GetData(0);
	
			//MoveSpeed.X = 0.0f; MoveSpeed.Y = 0.0f;
			MoveSpeed.X = Input2.GamePad0.AnalogLeft.X;
			MoveSpeed.Y = -Input2.GamePad0.AnalogLeft.Y;
			if(Input2.GamePad0.Left.Down)
			{
			MoveSpeed.X -= 1.0f;	
				Direction = MoveSpeed;
			}
			

			if(Index == PlayerIndex.PlayerOne)
			{
				MoveSpeed.X = Input2.GamePad0.AnalogLeft.X;
				MoveSpeed.Y = -Input2.GamePad0.AnalogLeft.Y; 
			}
			
			else if (Index == PlayerIndex.PlayerTwo)
			{
				MoveSpeed.X = Input2.GamePad0.AnalogRight.X;
				MoveSpeed.Y = -Input2.GamePad0.AnalogRight.Y; 
			}
			
			
				
			
			
			
			if (KeyboardTest == true)
			{

			if(Input2.GamePad0.Right.Down)
			{
			MoveSpeed.X += 1.0f;	
				Direction = MoveSpeed;
			}
			
			if(Input2.GamePad0.Up.Down)
			{
			MoveSpeed.Y += 1.0f;	
				Direction = MoveSpeed;
			}
			
			if(Input2.GamePad0.Down.Down)
			{
			MoveSpeed.Y -= 1.0f;	
				Direction = MoveSpeed;
			}
//			
			}
			
			switch (AppMain.TYPEOFGAME)
			{
			case "SINGLE":
				Direction = MoveSpeed;
				break;
			case "MULTIPLAYER":
				if(MoveSpeed.X == 0.0f && MoveSpeed.Y == 0.0f)
					AppMain.client.SetActionMessage('I');
				else
				{
					AppMain.client.SetActionMessage('M');
					Direction = MoveSpeed;
					AppMain.client.SetMyDirection(Direction.X,Direction.Y);
				}			
				break;
			default:
				break;

			}
			
		
				
				if(Input2.GamePad0.Cross.Down|| Input2.GamePad0.Cross.Down && Input2.GamePad0.Left.Down ||
			   Input2.GamePad0.Cross.Down && Input2.GamePad0.Right.Down || Input2.GamePad0.Cross.Down && Input2.GamePad0.Up.Down
			   || Input2.GamePad0.Cross.Down && Input2.GamePad0.Down.Down)
				{
					if(canShoot)
					{
						Shoot ();
					}
				}
				if(Input2.GamePad0.Cross.Release)
					canShoot = true;
				

			// Set frame to start of animation range if outside of range
			if(TileIndex2D.X < TileRangeX.X || TileIndex2D.X > TileRangeX.Y)
				TileIndex2D.X = TileRangeX.X;
		}
		
		private void HandleCollision()
		{
			if(SceneManager.CurrentScene == null)
				return;
			// Loop through tiles
			foreach(Tile t in Tile.Collisions)
			{
				bool fromLeft = Position.X + 32  > t.Position.X;
				bool fromRight = Position.X < t.Position.X + Tile.Width;
				bool fromTop = Position.Y < t.Position.Y + Tile.Height;
				bool fromBottom = Position.Y + 32  > t.Position.Y;
				if(fromLeft && fromRight && fromTop && fromBottom)
				{
					if(!MoveSpeed.IsZero() && t.IsCollidable)
					{
						//t.HandleCollision(Position, ref MoveSpeed);
						
						Vector2 HorizontalOffset = new Vector2(3, 0);
						Vector2 VerticalOffset = new Vector2(0, 3);
						if(fromLeft && MoveSpeed.X > 0)
						{
							Position = Position - HorizontalOffset;
						}
						if (fromRight && MoveSpeed.X < 0)
						{
							Position = Position + HorizontalOffset;
						}
					
						if (fromTop && MoveSpeed.Y < 0)
						{
							Position = Position + VerticalOffset;
						}
						if (fromBottom && MoveSpeed.Y > 0)
						{
							Position = Position - VerticalOffset;
						}
						MoveSpeed = Vector2.Zero;
					}
						
					if(t.Key == 'Z')
						Info.LevelClear = true;
				}
			}
		}
		
		public void Shoot()
		{
			if(AppMain.TYPEOFGAME.Equals("MULTIPLAYER"))
				AppMain.client.SetActionMessage('S');
			playerState = PlayerState.Shooting;
			Vector2 pos = new Vector2(Position.X + Quad.Bounds2().Point11.X/2, Position.Y + Quad.Bounds2().Point11.Y/2);
			ProjectileManager.Instance.Shoot(pos, Direction,(int)whichPlayer);
			canShoot = false;
			
		}
		
		public void UpdateAI(float dt,Player p)
		{
			if(movingLeft)
			{
				if(Position.X > 30)
				{
					MoveSpeed = new Vector2(-0.05f * dt,0.0f);
					Position += MoveSpeed;
					Direction = MoveSpeed;	
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
					MoveSpeed = new Vector2(0.05f * dt,0.0f);
					Position += MoveSpeed;
					Direction = MoveSpeed;	
				}
				else
				{
				movingLeft = true;	
				}
			}
			
			float dist = Vector2.Distance(p.Position,Position);	
			if(dist <300)
			{
				curTime += dt;
				if(curTime > fireRate)
				{
					Direction = p.Position - Position;
					Direction = Direction.Normalize();
					Shoot();
					curTime = 0.0f;	
				}
			}
		}
	}
}
