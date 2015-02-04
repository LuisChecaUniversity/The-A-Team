using System;
using System.Diagnostics;
using System.Collections.Generic;
using Sce.PlayStation.Core;
using Sce.PlayStation.Core.Graphics;
using Sce.PlayStation.HighLevel.GameEngine2D;
using Sce.PlayStation.HighLevel.GameEngine2D.Base;

namespace TheATeam
{
	public enum ItemType
	{
		flag,
		fire,
		water,
	}
	public class ItemManager
	{
		private static ItemManager instance = new ItemManager();

		private List<Item> items;
		private Item leftFlag, rightFlag, fireElement, waterElement;
		private Scene scene;
		
		private static TextureInfo flagTex;// =  new TextureInfo("/Application/Assets/FlagTemp.png");
		private static TextureInfo fireTex;// = new TextureInfo("/Application/Assets/FireElement.png");
		private static TextureInfo waterTex;// = new TextureInfo("/Application/Assets/WaterElement.png");
		
		private ItemManager ()
		{
			scene = GameSceneManager.currentScene;
			if(flagTex == null)
				flagTex =  new TextureInfo("/Application/Assets/FlagTemp.png");
			if(fireTex == null)
				fireTex = new TextureInfo("/Application/Assets/FireElement.png");
			if(waterTex == null)
				waterTex = new TextureInfo("/Application/Assets/WaterElement.png");
			
			items = new List<Item>();
			if(items.Count == 0)
			{
				Vector2 pos1 = new Vector2(100,290);
				Vector2 pos2 = new Vector2(864,290);
				leftFlag = new Item(scene, pos1, flagTex);
				rightFlag = new Item(scene, pos2, flagTex);
				
				pos1 = new Vector2(480,190);
				pos2 = new Vector2(480,390);
				fireElement = new Item(scene, pos1, fireTex);
				waterElement = new Item(scene, pos2, waterTex);
				
				items.Add(leftFlag);
				items.Add(rightFlag);
				items.Add(fireElement);
				items.Add(waterElement);
			}
		}
		
		public static ItemManager Instance
		{
			get{return instance;}
		}
		
		public void Update(float dt)
		{
//			redItem.Update(dt); //could be array
//			blueItem.Update(dt);
			foreach(Item item in items)
			{
				item.Update(dt);
			}
		}
		
		public void Grabbed()
		{
			//set the item/flag to follow player location... at least for now, drops when player dies
//			if (blueItem.collided == true)
//			{
//				blueItem.iSprite.Visible = false;
//			}
//	
//			if (redItem.collided == true)
//			{
//				redItem.iSprite.Visible = false;
//			}
			foreach(Item item in items)
			{
				if(item.collided == true)
					item.iSprite.Visible = false;
			}
			
		}
		
		public void ItemCollision(Vector2 pos, Bounds2 bounds)
		{
			Vector2 size = new Vector2(bounds.Point11.X, bounds.Point11.Y);
			// Will need to check this against every tile + player positions
			
			foreach(Item item in items)
			{
				if(item.hasCollided(pos, size))
					item.collided = true;
			}
			
//				if(blueItem.hasCollided(pos, size))
//					blueItem.collided = true;
//				if(redItem.hasCollided(pos, size))
//					redItem.collided = true;
			
			Grabbed();
		}
		
	}
}




