using System;
using System.Collections.Generic;
using Sce.PlayStation.Core;
using Sce.PlayStation.Core.Input;
using Sce.PlayStation.HighLevel.GameEngine2D;
using Sce.PlayStation.HighLevel.GameEngine2D.Base;

namespace TheATeam
{
	public class Waypoint
	{
		public Tile tile;
		public int ID;
		public float F, G, H, cumulativeCost;
		public Waypoint parent;
		
		public Waypoint(Tile t)
		{
			tile = t;
			F = 0.0f; G = 0.0f; H = 0.0f; cumulativeCost = 0.0f;
			parent = null;
		}
		public float Cost(Waypoint n)
		{
			return tile.Center.Distance(n.tile.Center);
		}
		public float CalculateHCost(Tile t)
		{
			return tile.Center.Distance(t.Center);
		}
		public void SetValues(float g, float h){ G = g; H = h; F = g + h; }
	};
	

	
	public class PathFinder
	{
		private List<Waypoint> path;
		private List<Waypoint> waypoints;
		private AIPlayer player;
		private Vector2 target;
		
		public PathFinder (AIPlayer player)
		{
			this.player = player;
			waypoints = new List<Waypoint>();
			path = new List<Waypoint>();
			InitWaypoints();
			target = player.Center;
		}
		public Vector2 GetTarget()
		{
			if (path.Count > 0)
			{
				target = path[0].tile.Center;
				if (player.Center.Distance(path[0].tile.Center) <= 30.0f)
					path.RemoveAt(0);
		
				return target;
			}
			else
			{
				return target;
			}
		}
		
		private void InitWaypoints()
		{
			int i = 0;
			foreach(List<Tile> l in Tile.Grid)
			{
				foreach(Tile t in l)
				{
					Waypoint newPoint = new Waypoint(t);
					newPoint.ID = i;
					waypoints.Add(newPoint);
					i++;
				}
			}
		}
		private Waypoint FindWaypoint(Vector2 position)
		{
			foreach(Waypoint w in waypoints)
			{
				if(position == w.tile.Center)
					return w;
			}
			return null;
		}
		private Waypoint FindNearestWaypoint(Vector2 target)
		{
			float smallestDistance = target.Distance(waypoints[0].tile.Center);
			Waypoint toReturn; toReturn = null;
			foreach(Waypoint w in waypoints)
			{
				float dist = target.Distance(w.tile.Center);
				if (dist < smallestDistance)
				{
					smallestDistance = dist;
					toReturn = w;
				}
			}
			return toReturn;
		}
		private List<Waypoint> GetConnections(Waypoint point)
		{
			float width = point.tile.Quad.Point11.X;
			float height = point.tile.Quad.Point11.Y;
			
			List<Waypoint> connections = new List<Waypoint>();
			Waypoint top, right, left, bottom;
			top = FindWaypoint(new Vector2(point.tile.Center.X, point.tile.Center.Y + height));
			bottom = FindWaypoint(new Vector2(point.tile.Center.X, point.tile.Center.Y - height));
			left = FindWaypoint(new Vector2(point.tile.Center.X - width, point.tile.Center.Y));
			right = FindWaypoint(new Vector2(point.tile.Center.X + width, point.tile.Center.Y));
			
			if(null != top && ((top.tile.IsCollidable && top.tile.Key == player.Element) || !top.tile.IsCollidable))
				connections.Add(top); 
			if(null != bottom && ((bottom.tile.IsCollidable && bottom.tile.Key == player.Element) || !bottom.tile.IsCollidable))
				connections.Add(bottom); 
			if(null != left && ((left.tile.IsCollidable && left.tile.Key == player.Element) || !left.tile.IsCollidable))
				connections.Add(left); 
			if(null != right && ((right.tile.IsCollidable && right.tile.Key == player.Element) || !right.tile.IsCollidable))
				connections.Add(right);
			
			return connections;
		}
		
		private void outputPath(Waypoint n)
		{
			if (null != n.parent)
				outputPath(n.parent);
			path.Add(n);//.push_back(n->waypoint->GetID());
		}
		public void FindPath(Vector2 target)
		{
			Waypoint p = FindNearestWaypoint(target);
			if(null != p)
				FindPath(FindNearestWaypoint(target).tile);
		}
		
		private void FindPath(Tile target)
		{
			path.Clear();
			List<Waypoint> open = new List<Waypoint>();
			List<Waypoint> closed = new List<Waypoint>();
			float G, H, F;
		
			Waypoint current = FindNearestWaypoint(player.Center);
			current.parent = null;
			//origin->SetValues(0.0, Origin->CalcHCost(Target));
			open.Add(current);
		
			
		
			while (open.Count > 0)
			{
				float smallestF = open[0].F;
				int smallestPos = 0;

				for (int i = 0; i < open.Count; i++)
				{
					if (open[i].F <= smallestF)
					{
						smallestF = open[i].F;
						smallestPos = i;
					}
				}
				current = open[smallestPos];
				open.Remove(open[smallestPos]); // remove the smallest from the open list (will be added to closed list)

				// have we reached target
				if (current.tile.Center == target.Center)
				{
					open.Clear();
					closed.Add(current);
					outputPath(current);//outputPath(FindNode(target));
					return;
				}
		
		
				List<Waypoint> connections = GetConnections(current);
		
		
				for (int i = 0; i < connections.Count; i++)
				{
					Waypoint successor = connections[i];
		
					G = current.G + successor.Cost(current);
					H = successor.CalculateHCost(target);
					F = G + H;
					if (open.Contains(successor))//open.Find(x => x.ID == successor.ID)) //if node is in open list already
					{
						if (closed.Contains(successor))
						{
							if (F < successor.F) // possibly dont need this ??
							{
								/*closed.erase(find(open.begin(), open.end(), connections[i].Connected));
								connections[i].Connected->SetValues(G, H);
								connections[i].Connected->SetParent(currentNode);
								open.push_back(connections[i].Connected);*/
							}
						}
						else
						{
							if (F < successor.F)
							{
								open.Remove(successor);
								successor.SetValues(G, H);
								successor.parent = current;
								open.Add(successor);
							}
						}
					}
					else
					{
						if (closed.Contains(successor)) // possibly dont need this ??
						{
							/*closed.erase(find(open.begin(), open.end(), connections[i].Connected));
							connections[i].Connected->SetValues(G, H);
							connections[i].Connected->SetParent(currentNode);
							open.push_back(connections[i].Connected);*/
						}
						else
						{
							successor.SetValues(G, H);
							successor.parent = current;
							open.Add(successor);
						}
					}
				}
				closed.Add(current);
			}
		}
		
	}
}

