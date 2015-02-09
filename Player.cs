using System;
using System.Collections.Generic;
using Sce.PlayStation.Core;
using Sce.PlayStation.Core.Input;
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

	public class Player: EntityAlive
	{
		private static int Y_INDEX = 4;
		private static float MoveDelta = 4f;
		private static float PlayerSize = Tile.Width; // 64x64 px
		private bool canShoot = true;
		private bool keyboardTest = true;
		private Vector2 Direction;
		private PlayerIndex whichPlayer;
		private PlayerState playerState;
		
		//AI variables
		private bool movingLeft = true;
		private bool shooting = false;
		private float fireRate = 800.0f;
		private float curTime = 0.0f;
		private char _element;
		
		//Player Tiles
		private List<Tile> playerTiles = new List<Tile>();
		
		public char Element
		{
			get { return _element; }
			set
			{
				_element = value;
				TileIndex2D.Y = Y_INDEX - Tile.Elements.IndexOf(value);
			}
		}

		public AttackStatus Attack { get { return attackState; } }
		
	public Player(Vector2 position, bool isPlayer1,List<Tile> tiles):
			base(Y_INDEX, position, new Vector2i(0, 3))

		{
			Element = 'N';
			IsDefending = false;

			if(isPlayer1)
				whichPlayer = PlayerIndex.PlayerOne;
			else
				whichPlayer = PlayerIndex.PlayerTwo;
			
			playerState = PlayerState.Idle;
			Direction = new Vector2(1.0f, 0.0f);
			playerTiles = tiles;
		}
		
		override public void Update(float dt)
		{
	
			// Handle battle
			base.Update(dt);
			
			switch(AppMain.TYPEOFGAME)
			{
			case "SINGLE":
				
				// Handle movement/attacks
				HandleInput();
					
				// Apply the movement
				Position = Position + positionDelta;			
				break;
				
			case "MULTIPLAYER":
				if(AppMain.ISHOST && whichPlayer == PlayerIndex.PlayerOne || !AppMain.ISHOST && whichPlayer == PlayerIndex.PlayerTwo)
				{
					// Handle movement/attacks
					HandleInput();
					
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
			// Find current tile and apply collision
			HandleCollision();
	
			// Make camera follow the player
			Info.CameraCenter = Position;
		}
		
		private void HandleInput()
		{
			//var gamePadData = GamePad.GetData(0);

			if(whichPlayer == PlayerIndex.PlayerOne)
			{
				positionDelta.X = Input2.GamePad0.AnalogLeft.X;
				positionDelta.Y = -Input2.GamePad0.AnalogLeft.Y;
			}
			else if(whichPlayer == PlayerIndex.PlayerTwo)
			{
				positionDelta.X = Input2.GamePad0.AnalogRight.X;
				positionDelta.Y = -Input2.GamePad0.AnalogRight.Y;
			}

			
			if (keyboardTest == true)

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

			switch(AppMain.TYPEOFGAME)

			{
			case "SINGLE":
			// Preserve Movement vector in Direction
				if(!positionDelta.IsZero())
				{
					Direction = positionDelta.Normalize();
				}
				break;
			case "MULTIPLAYER":
				if(positionDelta.IsZero())
					AppMain.client.SetActionMessage('I');
				else
				{
					AppMain.client.SetActionMessage('M');
					Direction = positionDelta;
					AppMain.client.SetMyDirection(Direction.X, Direction.Y);
				}			
				break;
			default:
				break;
			}

			
			if(Input2.GamePad0.Cross.Down || Input2.GamePad0.Cross.Down && Input2.GamePad0.Left.Down ||

			   Input2.GamePad0.Cross.Down && Input2.GamePad0.Right.Down || Input2.GamePad0.Cross.Down && Input2.GamePad0.Up.Down
			   || Input2.GamePad0.Cross.Down && Input2.GamePad0.Down.Down)
			{
				if(canShoot)
				{
					Shoot();
				}
			}
			if(Input2.GamePad0.Cross.Release)
				canShoot = true;
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
			// Set frame to start of animation range if outside of range
			if(TileIndex2D.X < animationRangeX.X || TileIndex2D.X > animationRangeX.Y)
				TileIndex2D.X = animationRangeX.X;
		}
		
		private void HandleCollision()
		{
			float screenWidth = Director.Instance.GL.Context.Screen.Width;
			float screenHeight = Director.Instance.GL.Context.Screen.Height - 32; // Blank space for UI.
			
			Vector2 HorizontalOffset = new Vector2(MoveDelta * 1.2f, 0);
			Vector2 VerticalOffset = new Vector2(0, MoveDelta * 1.2f);
			
			if(Position.X + PlayerSize > screenWidth)
			{
				Position = Position - HorizontalOffset;
			}
			
			if(Position.X < 0)
			{
				Position = Position + HorizontalOffset;
			}
			
			if(Position.Y < 0)
			{
				Position = Position + VerticalOffset;
			}
			
			if(Position.Y + PlayerSize > screenHeight)
			{
				Position = Position - VerticalOffset;
			}
			
			// Loop through tiles
			foreach(Tile t in Tile.Collisions)
			{
				bool fromLeft = Position.X + PlayerSize > t.Position.X;
				bool fromRight = Position.X < t.Position.X + Tile.Width;
				bool fromTop = Position.Y < t.Position.Y + Tile.Height;
				bool fromBottom = Position.Y + PlayerSize > t.Position.Y;
				if(fromLeft && fromRight && fromTop && fromBottom)
				{
					if(!positionDelta.IsZero() && t.IsCollidable && t.Key != Element)
					{
						if(fromLeft && positionDelta.X > 0)
						{
							Position = Position - HorizontalOffset;
						}
						if(fromRight && positionDelta.X < 0)
						{
							Position = Position + HorizontalOffset;
						}
					
						if(fromTop && positionDelta.Y < 0)
						{
							Position = Position + VerticalOffset;
						}
						if(fromBottom && positionDelta.Y > 0)
						{
							Position = Position - VerticalOffset;
						}
						positionDelta = Vector2.Zero;
					}
				}
			}
		}
		
		public void Shoot()
		{
			if(AppMain.TYPEOFGAME.Equals("MULTIPLAYER"))
				AppMain.client.SetActionMessage('S');
			playerState = PlayerState.Shooting;
			Vector2 pos = new Vector2(Position.X + Quad.Bounds2().Point11.X / 2, Position.Y + Quad.Bounds2().Point11.Y / 2);
			ProjectileManager.Instance.Shoot(pos, Direction, (int)whichPlayer);
			canShoot = false;
			
		}
		
		public void UpdateAI(float dt, Player p)
		{
			if(movingLeft)
			{
				if(Position.X > 30)
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
					Shoot();
					curTime = 0.0f;	
				}
			}
		}
		
		public void ChangeTiles(string type)
		{
			if(type.Equals("Fire"))
			{
				foreach (Tile t in playerTiles)
				{
					if(t.Key == 'N')
						t.Key = 'F';
				}
			}
			else if (type.Equals("Water"))
			{
				
				foreach (Tile t in playerTiles)
				{
					if(t.Key == 'N')
						t.Key = 'W';
				}
			}
		}
	}
}
