using System;
using System.Collections.Generic;
using Sce.PlayStation.Core;
using Sce.PlayStation.Core.Input;
using Sce.PlayStation.HighLevel.GameEngine2D;
using Sce.PlayStation.HighLevel.GameEngine2D.Base;


namespace TheATeam
{
	
	public class AIPlayer: Player
	{
		private Player player1;
		private Vector2 velocity;
		private Vector2 target;
		private float maxSpeed = 20.0f;
		private float rotationAngle = 0.0f;
		private float prevAng = 0.0f;
		private PathFinder pathfinder;
			

		public AIPlayer(Vector2 position, bool isPlayer1, List<Tile> tiles, Player player1):base(position, isPlayer1, tiles)
		{
			this.player1 = player1;
			velocity = new Vector2(0.0f, 0.0f);
			Position = new Vector2(400.0f, 300.0f);
			target = Position;
			pathfinder = new PathFinder(this);
		}
		
		override public void Update(float dt)
		{

//			if(!ItemManager.Instance.GetItem(ItemType.element, "Water").collided && Element == 'N')
//				target = ItemManager.Instance.GetItem(ItemType.element, "Water").position;
//			else if(Element == 'N')
//				target = ItemManager.Instance.GetItem(ItemType.element, "Fire").position;
			List<TouchData> touches = Touch.GetData(0);
			foreach(TouchData data in touches)
			{
				float xPos = (data.X +0.5f) * 960.0f;
				float yPos = 544.0f -((data.Y +0.5f) * 544.0f);
				
				if(new Vector2(xPos, yPos) != target)
				{
					target = new Vector2(xPos, yPos);
					pathfinder.FindPath(target);
				}
			}

			MoveInHeadingDirection(dt);
			HandleCollision();
			if(Position.Distance(pathfinder.GetTarget()) > 5.0f)
			{
				base.Direction = Vec2DNormalize(velocity);
			}
			else
			{
				base.Direction = Vec2DNormalize(player1.Position - Position);
			}
			base.HandleDirectionAnimation();
		}
		void RotateToFacePosition(Vector2 target)
		{
			Vector2 toTarget = Vec2DNormalize(Center - target);
			
			Vector2 heading = Vec2DNormalize(velocity);
			float angle = FMath.Acos(heading.Dot(toTarget));
			
			int sign = 1;
			// sign
			if (heading.Y*toTarget.X > heading.X*toTarget.Y)
			{
				sign = -1;
			}
			else 
			{
				sign = 1;
				
			}
			
			if(angle <= 0.0001)
				return;
			
			prevAng = angle;
			float maxTurnRate = 0.0099f;
			if(angle > maxTurnRate)
				angle = maxTurnRate;
			else if(angle < -maxTurnRate)
				angle = - maxTurnRate;
			else
				return;

			rotationAngle = FMath.Degrees(angle)*sign;
			
		}
		private void MoveInHeadingDirection(float dt)
		{
			dt /= 100.0f;
			//Vector2 force = Seek(target);
			
			
			Vector2 acceleration = Arrive(pathfinder.GetTarget());
			
			// a = (v - u)/t  -> v = u + at calculate velocity
			velocity += acceleration * dt;

			if(velocity.Length() > maxSpeed)
			{
				velocity = velocity.Normalize();
				velocity.X *= maxSpeed;
				velocity.Y *= maxSpeed;
			}
		
			// s = s + vt calculate position
			Position = new Vector2(Position.X + velocity.X * dt, Position.Y + velocity.Y *dt);

		}
		
		private Vector2 Seek(Vector2 target)
		{
			Vector2 force = Vec2DNormalize((target - Position));
			
			force.X *= maxSpeed;
			force.Y *= maxSpeed;
			
			force -= velocity;
			
			return force;
		}
		private Vector2 Arrive(Vector2 arriveLocation)
		{
			// travel to next pathfind location (arriveLocation) while checking distance to final target (target)
			Vector2 toTarget = arriveLocation - Position;
				
			float distance = toTarget.Length();
			
			Vector2 force = new Vector2(0.0f, 0.0f);
			
			if(target.Distance(Position) < 64.0f && target.Distance(Position) > 0.0f && distance > 0.0)
			{
				// scale dist by factor 10 otherwise force returned is too small (very slow movement)
				float speed = distance * 10 / maxSpeed;
				force = new Vector2 ((toTarget.X * speed / distance) - velocity.X,(toTarget.Y * speed / distance) - velocity.Y);
	
				return force;
				
			}
			else if( target.Distance(Position) > 0.0f && distance > 0.0)
			{
				float speed = maxSpeed;
				
				force = new Vector2 ((toTarget.X * speed / distance) - velocity.X,(toTarget.Y * speed / distance) - velocity.Y);
			
				return force;
			}
			
			return new Vector2(0.0f, 0.0f);
		}
		private Vector2 Vec2DNormalize(Vector2 v)
		{
			Vector2 vec = v;
			float length = v.Length();
			
			if(length > 0.0f)
			{
				vec.X /= length;
				vec.Y /= length;
			}
			
			return vec;
		}
		
		private Vector2 obstacleAvoidance()
		{
			// distance to project infront of tank (faster tank projects further)
			float detectionDist = 50.0f *( velocity.Length() / maxSpeed);
		
			// create 2 vectors to determine collision infront of tank (max and half dist infront)
			Vector2 ahead = new Vector2(Center.X + Vec2DNormalize(velocity).X * detectionDist, Center.Y + Vec2DNormalize(velocity).Y * detectionDist);
			Vector2 ahead2 = new Vector2(Center.X + Vec2DNormalize(velocity).X * detectionDist/2, Center.Y + Vec2DNormalize(velocity).Y * detectionDist/2);

			Tile nearestObject = FindNearestObstacle(ahead, ahead2);
			Vector2 resultantVelocity = new Vector2(0.0f, 0.0f);
		
			if (null != nearestObject)
			{
				// if tank is going to collide create force away from object
				resultantVelocity = ahead - nearestObject.Center;
				resultantVelocity = resultantVelocity.Normalize();
		
				//resultantVelocity *= maxSpeed;
			}
		
			return resultantVelocity;
		}
		private Tile FindNearestObstacle(Vector2 ahead, Vector2 ahead2)
		{
			Tile nearest = null;
			
			float distanceToObject;
			float distanceToNearest = 1000.0f;
			
			foreach(Tile t in Tile.Collisions)
			{
				if((t.Key != Element || t.Key != 'E') && t.IsCollidable)
				{
					bool collision;
					if(LineIntersectsTile(ahead, t) || LineIntersectsTile(ahead2, t) || LineIntersectsTile(Center, t))
					{
						collision = true;
					}
					else
					{
						collision = false;
					}
					
					distanceToObject = Position.Distance(t.Center);
					
					if(collision && (null == nearest || distanceToObject < distanceToNearest))
					{
						distanceToNearest = distanceToObject;
						nearest = t;
					}
				}
			}
			
			return nearest;
		}
		
		private bool LineIntersectsTile(Vector2 ahead, Tile obstacle)
		{
			float width = Width /2;
			float height = Height /2;
			
//			if (ahead.X == Center.X)
//			{
//				height = 0.0f;
//				width = 0.0f;
//			}
			
			// -------- when sprite are actaully 64x64
			//float oWidth = obstacle.Quad.Bounds2().Point11.X;
			//float oHeight = obstacle.Quad.Bounds2().Point11.Y;
			
			float oWidth = 36.0f ;
			float oHeight = 64.0f;
		
			if (ahead.X + width/2 < obstacle.Center.X - oWidth/ 2.0)
				return false;
			else if (ahead.X - width/2 >  obstacle.Center.X + oWidth / 2.0)
				return false;
			else if (ahead.Y  + height/2 <  obstacle.Center.Y - oHeight / 2.0)
				return false;
			else if (ahead.Y - height/2 >  obstacle.Center.Y + oHeight / 2.0)
				return false;
			else
			{
				return true;
			}
		}
		
		
		
	}
}

