using System;
using Sce.PlayStation.Core;
using Sce.PlayStation.Core.Input;
using Sce.PlayStation.HighLevel.GameEngine2D.Base;

namespace TheATeam
{
	public enum PlayerIndex
	{
		PlayerOne = 0,
		PlayerTwo = 1,
	}
	
	public enum PlayerState
	{
		Idle,
		Moving,
		Shooting,
	}
	public class Player: EntityAlive
	{
		private static int PLAYER_INDEX = 0;
		private bool canShoot = true;
		private PlayerIndex whichPlayer;
		private PlayerState playerState;
		
		private Vector2 oldPos;
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
		
		public AttackStatus Attack { get { return attackState; } }
		
		public Vector2 Direction;
		public Vector2 GetPosition { get { return Position; }}
		private static float MoveDelta = 2f;
		
		private void HandleInput()
		{
			//var gamePadData = GamePad.GetData(0);
	
			MoveSpeed.X = 0.0f; MoveSpeed.Y = 0.0f;
			MoveSpeed.X = Input2.GamePad0.AnalogLeft.X;
			MoveSpeed.Y = -Input2.GamePad0.AnalogLeft.Y;
//			if(Input2.GamePad0.Left.Down)
//			{
//			MoveSpeed.X -= 1.0f;	
//				Direction = MoveSpeed;
//			}
//			
//			if(Input2.GamePad0.Right.Down)
//			{
//			MoveSpeed.X += 1.0f;	
//				Direction = MoveSpeed;
//			}
//			
//			if(Input2.GamePad0.Up.Down)
//			{
//			MoveSpeed.Y -= 1.0f;	
//				Direction = MoveSpeed;
//			}
//			
//			if(Input2.GamePad0.Down.Down)
//			{
//			MoveSpeed.Y += 1.0f;	
//				Direction = MoveSpeed;
//			}
//			
			
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
				if(t.Overlaps(this))
				{
					if(!MoveSpeed.IsZero() && t.IsCollidable)
						//t.HandleCollision(Position, ref MoveSpeed);
						MoveSpeed *= -1.0f;
					
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
			ProjectileManager.Instance.Shoot(Position, Direction,1);
			canShoot = false;
			
		}
	}
}
