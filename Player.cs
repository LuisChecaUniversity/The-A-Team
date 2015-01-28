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
		}
		
		override public void Update(float dt)
		{
	
			// Handle battle
			base.Update(dt);
			
			// Handle Death
//			if(!IsAlive)
//				SceneManager.ReplaceUIScene(new DeadUI());
			
			// Handle movement/attacks
			HandleInput();
			
			// Find current tile and apply collision
			HandleCollision();
			
			// Apply the movement
			Position = Position + MoveSpeed;
			// Make camera follow the player
			Info.CameraCenter = Position;
			
			// handle bullet update and collision
			ProjectileManager.Instance.Update(dt);
			foreach(Tile t in Tile.Collisions)
			{
				ProjectileManager.Instance.ProjectileCollision(t.Position, t.Quad.Bounds2());
			}
		}
		
		public AttackStatus Attack { get { return attackState; } }
		
		private Vector2 Direction;
		private static float MoveDelta = 2f;
		
		private void HandleInput()
		{
			var gamePadData = GamePad.GetData(0);
			
//			if(whichPlayer == PlayerIndex.PlayerOne)
//			{
				// Apply direction and animation
				if(Input2.GamePad0.Left.Down)//(gamePadData.Buttons & GamePadButtons.Left) != 0) //&& gamePadData.AnalogLeftX <0)
				{
					MoveSpeed.X = -MoveDelta;
					// Set animation range.
					TileRangeX = new Vector2i(6, 7);
				}
				if(Input2.GamePad0.Right.Down)//if((gamePadData.Buttons & GamePadButtons.Right) != 0) //&& gamePadData.AnalogLeftX >0)
				{
					MoveSpeed.X = MoveDelta;
					TileRangeX = new Vector2i(4, 5);
				}
				if(Input2.GamePad0.Up.Down)//if((gamePadData.Buttons & GamePadButtons.Up) != 0) //&& gamePadData.AnalogLeftY >0)
				{
					MoveSpeed.Y = MoveDelta;
					TileRangeX = new Vector2i(2, 3);
				}
				if(Input2.GamePad0.Down.Down)//if((gamePadData.Buttons & GamePadButtons.Down) != 0) //&& gamePadData.AnalogLeftY <0)
				{
					MoveSpeed.Y = -MoveDelta;
					TileRangeX = new Vector2i(0, 1);
				}
				
				if (MoveSpeed != Vector2.Zero)
				{
					Direction = MoveSpeed;
					playerState = PlayerState.Moving;
				}
				else
					playerState = PlayerState.Idle;
				// added player shoot 
				if((gamePadData.ButtonsDown & GamePadButtons.Cross) != 0) // S key
				{
					//Console.WriteLine("SHOOTING");
					//ProjectileManager.Instance.Shoot(Position, Direction);
				}
				
				if(Input2.GamePad0.Cross.Down)
				{
					if(canShoot)
					{
						playerState = PlayerState.Shooting;
						canShoot = false;
						ProjectileManager.Instance.Shoot(Position, Direction);
					}
				}
				if(Input2.GamePad0.Cross.Release)
					canShoot = true;
				
				
				
//			}
//			else if(whichPlayer == PlayerIndex.PlayerTwo)
//			{
//				
//			}
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
	}
}
