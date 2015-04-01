using System;
using System.Collections.Generic;
using Sce.PlayStation.Core;
using Sce.PlayStation.Core.Input;
using Sce.PlayStation.HighLevel.GameEngine2D;
using Sce.PlayStation.HighLevel.GameEngine2D.Base;


namespace TheATeam
{
	enum Behaviour
	{
		Attacking,
		SeekElement,
		GetFlag,
		ReturnFlag,
	}
	public class AIPlayer: Player
	{
		private Player player1;
		private Vector2 velocity;
		private Vector2 target, finalTarget;
		private Waypoint targetWP;
		private float maxSpeed = 10.0f;
		private PathFinder pathfinder;
		private bool havePath = false;
		private Behaviour behaviour = Behaviour.SeekElement;
		private float attackDistance = 200.0f;
		private float attackTime = 6.0f;
		private float attackTimer = 0.0f;
		private float maxFireRate = 600.0f;
		private float shootTimer = 0.0f;
		private Item player1Flag = ItemManager.Instance.GetItem(ItemType.flag, "Player1Flag");
		private Item player2Flag = ItemManager.Instance.GetItem(ItemType.flag, "Player2Flag");
		private List<Waypoint> path;
		private Item elementTarget;
			

		public AIPlayer(Vector2 position, bool isPlayer1, List<Tile> tiles, Player player1):base(position, isPlayer1, tiles)
		{
			this.player1 = player1;
			velocity = new Vector2(0.0f, 0.0f);
			//Position = new Vector2(400.0f, 300.0f);
			target = Position;
			finalTarget = Position;
			pathfinder = new PathFinder(this);
			path = new List<Waypoint>();
			targetWP = null;
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
				
				if(new Vector2(xPos, yPos) != finalTarget)
				{
					finalTarget = new Vector2(xPos, yPos);
					path = pathfinder.FindPath(finalTarget);
				}
			}
			
			updateBehaviours(dt);
			
			MoveInHeadingDirection(dt);
			HandleCollision();
			
//			if(Position.Distance(target) > 5.0f)
//			{
//				base.Direction = Vec2DNormalize(velocity);
//			}
//			else
//			{
//				base.Direction = Vec2DNormalize(player1.Position - Position);
//			}
			
			
			shootTimer += dt;
			
			UpdateTarget();
			
			base.HandleDirectionAnimation();
			base.UpdateMana(dt);
			base.UpdateShield(dt);
			base.SlowEffect(dt);
		}
		
		void updateBehaviours(float dt)
		{
			// if player is in range attack them
			if(player1.Center.Distance(Center) < attackDistance)
			{
				Shoot(player1.Center);
			}
			else if(targetWP != null)
			{
				if(targetWP.tile.IsCollidable && targetWP.tile.Key != Element)
				{
					if(_stats.mana > _stats.MaxMana/2)
						Shoot (targetWP.tile.Center);
				}
			}
			
			switch(behaviour)
			{
			case Behaviour.GetFlag:
				GetFlag();
				break;
			case Behaviour.ReturnFlag:
				ReturnFlag();
				break;
			case Behaviour.SeekElement:
				SeekElement();
				break;
			}
			
//			// if have flag and low hp avoid player + return flag
//			// if low mana and attack player greater distance
//			// if low hp only attack player when player mana low
//
//			if(!ItemManager.Player2HoldingFlag && ItemManager.Player1HoldingFlag && player1.Position.Distance(ItemManager.Instance.GetItem(ItemType.flag, "Player1Flag").position) < Director.Instance.GL.Context.GetViewport().Width / 2)
//			{
//				ChangeBehaviour(Behaviour.CollectFlag);
//			}
//			//else if(player1.Position.Distance(
//			else if(player1.Position.Distance(Position) < attackDistance  || ItemManager.Player1HoldingFlag)
//			{
//				ChangeBehaviour(Behaviour.Attacking);
//			}
//			else if( Element != 'N' && attackTimer == 0.0f)
//			{
//				ChangeBehaviour( Behaviour.CollectFlag);
//			}
//			else if(attackTimer == 0.0f)
//			{
//				ChangeBehaviour(Behaviour.SeekElement);
//			}
//			
//			if(havePath && pathfinder.PathLength() <= 0)
//				havePath = false;
//			
//			switch(behaviour)
//			{
//			case Behaviour.CollectFlag:
//				if(ItemManager.Player2HoldingFlag)
//					ReturnFlag();
//				else
//					CollectFlag();
//				break;
//			case Behaviour.SeekElement:
//			SeekElement();
//				break;
//			case Behaviour.Attacking:
//				attackTimer += dt/1000;
//				if(attackTimer >= attackTime && player1.Position.Distance(Position) > attackDistance)
//					attackTimer = 0.0f;
//				AttackPlayer();
//				break;
//			}
		}
		
		private void CheckPlayer()
		{
			// if player is in range
				// if player has more hp
					// if player is low on mana
			
				// if player has equal or less
					// if ai is low on mana
			
			// if player is out of range
				// does player have flag
					// is player closer to our flag
			
			if(player1.Center.Distance(Center) > attackDistance)
			{
				if(player1.Health >= Health *1.5f)
				{
					if(player1.Mana < 30)
					{
						// not enough mana to shoot - attack
					}
					else
					{
						// retreat
					}
				}
				else
				{
					// player similar or low hp - attack
					
					
				}
			}
			else
			{
				if(ItemManager.Player1HoldingFlag)
				{
					if(Center.X > Director.Instance.GL.Context.GetViewport().Width / 2)
					{
						// player has flag and AI is in his half of the map - attack the player
					}
					else
					{
						// grab flag
						ChangeBehaviour(Behaviour.GetFlag);
					}
				}
			}
		}
		
		private void GetFlag()
		{
			if(!havePath)
			{
				finalTarget = ItemManager.Instance.GetItem(ItemType.flag, "Player1Flag").position;
				FindPath(finalTarget);
			}
			
			if(ItemManager.Player2HoldingFlag)
			{
				ChangeBehaviour(Behaviour.ReturnFlag);
			}
		}
		
		private void ReturnFlag()
		{
			if(!havePath)
			{
				finalTarget = ItemManager.Instance.GetItem(ItemType.flag, "Player2Flag").position;
				FindPath(finalTarget);
			}
		}
		private void SeekElement()
		{
			if(Element == 'N' || Element2 == 'N')
			{
				if(!havePath)
				{
					Item closest = ClosestItem();
				
					if(elementTarget != closest)
						elementTarget = closest;
					
					finalTarget = elementTarget.position;
					FindPath(finalTarget);
				}
				
				if(elementTarget.collided)
					havePath = false;
			}
			else
			{
				ChangeBehaviour(Behaviour.GetFlag);
			}
			
		}
		
		private Item ClosestItem()
		{
			int shortestDistance = 100;
			Item element = ItemManager.Instance.GetItem(ItemType.element, "Fire");
			
			List<Waypoint> route = new List<Waypoint>();
			
			foreach (Item item in ItemManager.Instance.GetAllItems())
			{
				if(item.Type == ItemType.element && !item.collided)
				{
					int distance;
					route = pathfinder.FindPath(item.position);
					distance = route.Count;
					if (distance < shortestDistance)
					{
						shortestDistance = distance;
						element = item;
					}
				}
			}
			
			return element;
		}
		
		private void ChangeBehaviour(Behaviour b)
		{
			if(behaviour == b)
				return;
			else
			{
				behaviour = b;
				havePath = false;
			}
		}
//		void AttackPlayer()
//		{
//			if(!ItemManager.Player1HoldingFlag || !ItemManager.Player2HoldingFlag)
//			{
//				if(Position.Distance(ItemManager.Instance.GetItem(ItemType.flag, "Player1Flag").position) < Position.Distance(player1.Position))
//				{
//					ChangeBehaviour(Behaviour.CollectFlag);
//					attackTimer = 0.0f;
//					return;
//				}
//			}
//			if(!havePath)
//			{
//				Vector2 pos = player1.Position; 
//				if(ItemManager.Player1HoldingFlag)
//					pos += Vec2DNormalize(ItemManager.Instance.GetItem(ItemType.flag, "Player1Flag").position - player1.Position) * 250;
//				else
//					pos += Vec2DNormalize(ItemManager.Instance.GetItem(ItemType.flag, "Player2Flag").position - player1.Position) * 250;
//				
//				if(pos.X > Director.Instance.GL.Context.GetViewport().Width || pos.Y > Director.Instance.GL.Context.GetViewport().Height)
//					return;
////				if(pos.X > Director.Instance.GL.Context.GetViewport().Width)
////				{
////					pos.X = pos.X - (pos.X - Director.Instance.GL.Context.GetViewport().Width);
////				}
////				if(pos.Y > Director.Instance.GL.Context.GetViewport().Height)
////				{
////					pos.Y = pos.Y - (pos.Y - Director.Instance.GL.Context.GetViewport().Height);
////				}
//				
//				FindPath(pos);
//			}
//			ShootingDirection = Vec2DNormalize(player1.Position - Position);
//			Shoot ();
//		}
//		void CollectFlag()
//		{
//			// want to know when to shoot P1 tiles...maybe mark for attack when pathfinding?
//			Item flag = ItemManager.Instance.GetItem(ItemType.flag, "Player1Flag");
//			if(target != flag.position)
//			{
//				if(!havePath)
//					FindPath(flag.position);
//			}
//			if(pathfinder.PathLength() <= 0 && target != flag.position)
//			{
//				ShootingDirection = Vec2DNormalize(flag.position - Position);
//				Shoot ();
//			}			
//		}
//		void ReturnFlag()
//		{
//			Item flag = ItemManager.Instance.GetItem(ItemType.flag, "Player2Flag");
//			if(target != flag.position && !havePath)
//			{
//				FindPath(flag.position);
//			}
////			if(pathfinder.PathLength() <= 0 && target != flag.position)
////			{
////				Shoot ();
////			}
//		}
		void FindPath(Vector2 pos)
		{
			path = pathfinder.FindPath(pos);
			havePath = true;
		}
//		void SeekElement()
//		{
//			// need to know which item im targeting so if it gets collected can swap
//			// need to check if AI dies, reseeks element
//			// seperate this into 2 parts. 1 find path, 2 carry out checks
//			if(Element == 'N')
//			{
//				if(!havePath)
//				{
//					int shortestDistance = 100;
//					Item element = ItemManager.Instance.GetItem(ItemType.element, "Fire");
//					foreach (Item item in ItemManager.Instance.GetAllItems())
//					{
//						if(item.Type == ItemType.element && !item.collided)
//						{
//							int distance;
//							pathfinder.FindPath(item.position);
//							distance = pathfinder.PathLength();
//							if (distance < shortestDistance)
//							{
//								shortestDistance = distance;
//								element = item;
//							}
//						}
//					}
//					
//					FindPath(element.position);
//				}
//			}
//			else
//				ChangeBehaviour(Behaviour.CollectFlag);
//			
//		}
		
		private void UpdateTarget()
		{
			if(path.Count > 0)
			{
				targetWP = path[0];
				if(Center.Distance(path[0].tile.Center) <= 30.0f)
				{
					path.RemoveAt(0);
				}
			}

		}
		private Vector2 moveNextTarget()
		{
			if(targetWP != null)
			{
				if(!targetWP.tile.IsCollidable || (targetWP.tile.IsCollidable && targetWP.tile.Key == Element))
					target = targetWP.tile.Center;
			}
			if(path.Count == 0)
				return finalTarget;
			else
			return target;
			
		}
		
		private void MoveInHeadingDirection(float dt)
		{
			dt /= 100.0f;
			//Vector2 force = Seek(target);
			Vector2 acceleration = Arrive(moveNextTarget());

			// a = (v - u)/t  -> v = u + at calculate velocity
			velocity += acceleration * dt;
			float speed = velocity.Length();
			
			if(speed > maxSpeed)
			{
				velocity = velocity.Normalize();
				velocity.X *= maxSpeed;
				velocity.Y *= maxSpeed;
			}
			else if(speed < 1.0f)
			{
				base.Direction = Vec2DNormalize(player1.Position - Position);
				return;
			}
			base.Direction = Vec2DNormalize(velocity);
			// s = s + vt calculate position
			Position = new Vector2(Position.X + velocity.X * _stats.moveSpeed * dt, Position.Y + velocity.Y * _stats.moveSpeed * dt);
	
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
	
		private void Shoot(Vector2 target)
		{
			ShootingDirection = Vec2DNormalize(target - Position);
			Shoot ();
		}
		
		override public void Shoot()
		{
			
			if (_stats.mana >= _stats.manaCost)
			{
				if(shootTimer > maxFireRate)
				{
					_stats.mana -= _stats.manaCost;
					if (AppMain.TYPEOFGAME.Equals("MULTIPLAYER"))
					{
						AppMain.client.SetActionMessage('S');
					}
					playerState = PlayerState.Shooting;
					
					ProjectileManager.Instance.Shoot(this);
					canShoot = false;
					shootTimer = 0.0f;
				}
			}
			
		}

		
	}
}

