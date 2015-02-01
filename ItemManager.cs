using System;
using System.Diagnostics;
using System.Collections.Generic;
using Sce.PlayStation.Core;
using Sce.PlayStation.Core.Graphics;
using Sce.PlayStation.HighLevel.GameEngine2D;
using Sce.PlayStation.HighLevel.GameEngine2D.Base;

namespace TheATeam
{
	public class ItemManager
	{
		private static ItemManager instance = new ItemManager();
		private static Item redItem;
		private static Item blueItem;
		private Scene scene;
		
		private ItemManager ()
		{
			scene = Director.Instance.CurrentScene;
			Vector2 pos1 = new Vector2(50,300);
			Vector2 pos2 = new Vector2(800,300);
			redItem = new Item(scene, pos1);
			blueItem = new Item(scene, pos2);
		}
		
		public static ItemManager Instance
		{
			get{return instance;}
		}
		
		public void Update(float dt)
		{
			redItem.Update(dt); //could be array
			blueItem.Update(dt);
			
		}
		
		public void Grabbed()
		{
			//set the item/flag to follow player location... at least for now, drops when player dies
		if (blueItem.collided == true)
			{
				blueItem.iSprite.Visible = false;
			}
	
			if (redItem.collided == true)
			{
				redItem.iSprite.Visible = false;
			}
			
		}
		
			public void ItemCollision(Vector2 pos, Bounds2 bounds)
		{
			Vector2 size = new Vector2(bounds.Point11.X, bounds.Point11.Y);
			// Will need to check this against every tile + player positions
			
				if(blueItem.hasCollided(pos, size))
					blueItem.collided = true;
				if(redItem.hasCollided(pos, size))
					redItem.collided = true;
		}
		
	}
}




