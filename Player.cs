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
		private static int Y_INDEX = 1;
		private bool canShoot = true;

		public char Element { get; set; }

		public AttackStatus Attack { get { return attackState; } }
		
		public Player(Vector2 position):
			base(Y_INDEX, position, new Vector2i(0, 1))
		{
			Element = 'N';
			IsDefending = false;
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
			Position = Position + positionDelta;
			// Make camera follow the player
			Info.CameraCenter = Position;
			
			// handle bullet update and collision
			ProjectileManager.Instance.Update(dt);
			foreach(Tile t in Tile.Collisions)
			{
				ProjectileManager.Instance.ProjectileCollision(t.Position, t.Quad.Bounds2());
			}
		}
		
		private Vector2 Direction;
		private static float MoveDelta = 2f;
		
		private void HandleInput()
		{
			var gamePadData = GamePad.GetData(0);
			
			// Apply direction
			if((gamePadData.Buttons & GamePadButtons.Left) != 0) //&& gamePadData.AnalogLeftX <0)
			{
				positionDelta.X = -MoveDelta;
			}
			if((gamePadData.Buttons & GamePadButtons.Right) != 0) //&& gamePadData.AnalogLeftX >0)
			{
				positionDelta.X = MoveDelta;
			}
			if((gamePadData.Buttons & GamePadButtons.Up) != 0) //&& gamePadData.AnalogLeftY >0)
			{
				positionDelta.Y = MoveDelta;
			}
			if((gamePadData.Buttons & GamePadButtons.Down) != 0) //&& gamePadData.AnalogLeftY <0)
			{
				positionDelta.Y = -MoveDelta;
			}
			// Preserve Movement vector in Direction
			if(positionDelta != Vector2.Zero)
			{
				Direction = positionDelta.Normalize();
			}
			// Added player shoot
			if(Input2.GamePad0.Cross.Down)  // S Eky
			{
				if(canShoot)
				{
					canShoot = false;
					ProjectileManager.Instance.Shoot(Position, Direction);
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
			if(TileIndex2D.X < animationRangeX.X || TileIndex2D.X > animationRangeX.Y)
				TileIndex2D.X = animationRangeX.X;
		}
		
		private void HandleDirectionAnimation()
		{
			// Declare Ranges
			Vector2i LeftRange = new Vector2i(6, 7);
			Vector2i RightRange = new Vector2i(4, 5);
			Vector2i UpRange = new Vector2i(2, 3);
			Vector2i DownRange = new Vector2i(0, 1);
			if(Direction.X > 0)
			{
				
			}
		}
		
		private void HandleCollision()
		{
			// Loop through tiles
			foreach(Tile t in Tile.Collisions)
			{
				if(t.Overlaps(this))
				{
					if(!positionDelta.IsZero() && t.IsCollidable & t.Key != Element)
					{
						Vector2 X = new Vector2(3, 0);
						Vector2 Y = new Vector2(0, 3);
						if(positionDelta.X < 0)
							Position = Position + X;
						if(positionDelta.X > 0)
							Position = Position - X;
						
						if(positionDelta.Y < 0)
							Position = Position + Y;
						if(positionDelta.Y > 0)
							Position = Position - Y;
						
						positionDelta = Vector2.Zero;
					}
					
					if(t.Key == 'Z')
						Info.LevelClear = true;
				}
			}
		}
	}
}
