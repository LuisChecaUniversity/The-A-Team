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
	public class Player: EntityAlive
	{
		private static int PLAYER_INDEX = 0;
		private bool canShoot = true;
		
		public Player(Vector2 position):
			base(PLAYER_INDEX, position, new Vector2i(0, 1))
		{
			IsDefending = false;
			Stats.Lives = 5;
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
			ProjectileManager.Instance.ProjectileCollision(Position, Quad.Bounds2());
		}
		
		public AttackStatus Attack { get { return attackState; } }
		
		private Vector2 Direction;
		private static float MoveDelta = 2f;
		
		private void HandleInput()
		{
			var gamePadData = GamePad.GetData(0);
			
			// Apply direction and animation
			if((gamePadData.Buttons & GamePadButtons.Left) != 0) //&& gamePadData.AnalogLeftX <0)
			{
				MoveSpeed.X = -MoveDelta;
				// Set animation range.
				TileRangeX = new Vector2i(6, 7);
			}
			if((gamePadData.Buttons & GamePadButtons.Right) != 0) //&& gamePadData.AnalogLeftX >0)
			{
				MoveSpeed.X = MoveDelta;
				TileRangeX = new Vector2i(4, 5);
			}
			if((gamePadData.Buttons & GamePadButtons.Up) != 0) //&& gamePadData.AnalogLeftY >0)
			{
				MoveSpeed.Y = MoveDelta;
				TileRangeX = new Vector2i(2, 3);
			}
			if((gamePadData.Buttons & GamePadButtons.Down) != 0) //&& gamePadData.AnalogLeftY <0)
			{
				MoveSpeed.Y = -MoveDelta;
				TileRangeX = new Vector2i(0, 1);
			}
			
			if (MoveSpeed != Vector2.Zero)
			{
				Direction = MoveSpeed;
			}
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
					canShoot = false;
					// need to pass the centre of the player x+widht, y+height
					Vector2 pos = new Vector2(Position.X + this.Quad.Point11.X/2, Position.Y + this.Quad.Point11.Y/2);
					ProjectileManager.Instance.Shoot(pos, Direction);
				}
			}
			if(Input2.GamePad0.Cross.Release)
				canShoot = true;
			// Attacks if in battle
			if(InBattle)
			{
				if((gamePadData.ButtonsDown & GamePadButtons.Cross) != 0)
				{
					attackState = AttackStatus.MeleeNormal;
				}
				if((gamePadData.ButtonsDown & GamePadButtons.Circle) != 0)
				{
					attackState = AttackStatus.MeleeStrong;
				}
				if((gamePadData.ButtonsDown & GamePadButtons.Square) != 0)
				{
					attackState = AttackStatus.RangedNormal;
				}
				if((gamePadData.ButtonsDown & GamePadButtons.Triangle) != 0)
				{
					attackState = AttackStatus.RangedStrong;
				}
				
				if((gamePadData.ButtonsDown & GamePadButtons.L) != 0)
				{
					IsDefending = true;
				}
			}
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
