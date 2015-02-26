using System;
using System.Collections.Generic;
using Sce.PlayStation.Core;
using Sce.PlayStation.HighLevel.GameEngine2D;
using Sce.PlayStation.HighLevel.GameEngine2D.Base;

namespace TheATeam
{
	public class AIPlayer: Player
	{
		private Player player1;
		private Vector2 velocity;
		private Vector2 target;
		private float maxSpeed = 1.5f;
		

		
		public AIPlayer(Vector2 position, bool isPlayer1, List<Tile> tiles, Player player1):base(position, isPlayer1, tiles)
		{
			this.player1 = player1;
			velocity = new Vector2(0.0f, 0.0f);
		}
		
		override public void Update(float dt)
		{

			if(!ItemManager.Instance.GetItem(ItemType.element, "Water").collided && Element == 'N')
				target = ItemManager.Instance.GetItem(ItemType.element, "Water").position;
			else if(Element == 'N')
				target = ItemManager.Instance.GetItem(ItemType.element, "Fire").position;
			
			MoveInHeadingDirection(dt);
			base.Direction = velocity.Normalize();
			base.HandleDirectionAnimation();
		}
		
		private void MoveInHeadingDirection(float dt)
		{
			dt /= 10.0f;
			//Vector2 force = Seek(target);

			Vector2 acceleration = Seek(target);
		
			// a = (v - u)/t  -> v = u + at calculate velocity
			velocity += acceleration * dt;

			if(velocity.Length() > maxSpeed)
			{
				velocity = velocity.Normalize();
				velocity.X *= maxSpeed;
				velocity.Y *= maxSpeed;
			}
			Console.WriteLine("velocityx " + velocity.X);
			Console.WriteLine("velocityy " + velocity.Y);
		
			// s = s + vt calculate position
			Position = new Vector2(Position.X + velocity.X * dt, Position.Y + velocity.Y *dt);
		}
		
		private Vector2 Seek(Vector2 target)
		{
			Vector2 force = (target - Position).Normalize();
			
			force.X *= maxSpeed;
			force.Y *= maxSpeed;
			
			//force -= velocity;
			
			return force;
		}
		
		private Vector2 obstacleAvoidance()
		{
			// distance to project infront of tank (faster tank projects further)
			float detectionDist = 60.0f *( velocity.Length() / maxSpeed);
		
			// create 2 vectors to determine collision infront of tank (max and half dist infront)
			Vector2 ahead = Position + velocity.Normalize() * detectionDist;
			Vector2 ahead2 = Position + velocity.Normalize()* detectionDist * 0.5;
			
		
			Tile nearestObject = FindNearestObstacle(ahead, ahead2);
			Vector2D resultantVelocity;
		
			if (nullptr != nearestObject)
			{
				// if tank is going to collide create force away from object
				resultantVelocity = ahead - nearestObject.get;
				resultantVelocity.Normalize();
		
				resultantVelocity *= _tank->GetMaxSpeed();
			}
		
			return resultantVelocity;
		}
		private Tile FindNearestObstacle(Vector2 ahead, Vector2 ahead2)
		{
			
		}
	}
}

