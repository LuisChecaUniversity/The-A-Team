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
		element
	}

	public class ItemManager
	{
		private static ItemManager instance = new ItemManager();
		private List<Item> items;
		private Item leftFlag, rightFlag, lightningElement, airElement, earthElement, fireElement, waterElement;
		private static Vector2i flagIndex = new Vector2i();
		private static Vector2i lightningIndex = new Vector2i(0, 1);
		private static Vector2i airIndex = new Vector2i(0, 2);
		private static Vector2i earthIndex = new Vector2i(0, 3);
		private static Vector2i fireIndex = new Vector2i(0, 4);
		private static Vector2i waterIndex = new Vector2i(0, 5);
		public Vector2 FirstFlag;
		public Vector2 SecondFlag;
		public static bool Player1HoldingFlag = false;
		public static bool Player2HoldingFlag = false;
		
		public static ItemManager Instance { get { return instance; } }
		
		private ItemManager()
		{			
			items = new List<Item>();
		}

		public void initFlags(Scene curScene)
		{
			Player1HoldingFlag = false;
			Player2HoldingFlag = false;
			FirstFlag = new Vector2(30, 290);
			leftFlag = new Item(curScene, FirstFlag, flagIndex, ItemType.flag, "Player1Flag");
			SecondFlag = new Vector2(926, 290);
			rightFlag = new Item(curScene, SecondFlag, flagIndex, ItemType.flag, "Player2Flag");
			
			items.Clear();
			items.Add(leftFlag);
			items.Add(rightFlag);
		}

		public void initElements(Scene curScene)
		{
			Vector2 pos1 = new Vector2(480, 50);
			Vector2 pos2 = new Vector2(480, 150);
			Vector2 pos3 = new Vector2(480, 250);
			Vector2 pos4 = new Vector2(480, 350);
			Vector2 pos5 = new Vector2(480, 450);
			
			lightningElement = new Item(curScene, pos1, lightningIndex, ItemType.element, "Lightning");
			airElement = new Item(curScene, pos2, airIndex, ItemType.element, "Air");
			earthElement = new Item(curScene, pos3, earthIndex, ItemType.element, "Earth");
			fireElement = new Item(curScene, pos4, fireIndex, ItemType.element, "Fire");
			waterElement = new Item(curScene, pos5, waterIndex, ItemType.element, "Water");
			
			items.Add(lightningElement);
			items.Add(airElement);
			items.Add(earthElement);
			items.Add(fireElement);
			items.Add(waterElement);
		}
		
		public void Update(float dt)
		{
			foreach (Item item in items)
			{
				item.Update(dt);
			}
		}
		
		public void Grabbed()
		{
			//set the item/flag to follow player location... at least for now, drops when player dies
			foreach (Item item in items)
			{
				if (item.collided == true)
				{
					item.iSprite.Visible = false;
				}
			}
		}
		
		public void ItemCollision(Player p1, Player p2)
		{
			Vector2 p1Size = new Vector2(p1.Quad.Bounds2().Point11.X, p1.Quad.Bounds2().Point11.Y);
			Vector2 p2Size = new Vector2(p2.Quad.Bounds2().Point11.X, p2.Quad.Bounds2().Point11.Y);
			// Will need to check this against every tile + player positions
			
			foreach (Item item in items)
			{
				if (!item.collided)
				{
					//check player 1 with items first
					if (item.hasCollided(p1.Position, p1Size))
					{
						Console.WriteLine("Collided with " + item.Name);
						item.iSprite.Visible = false;
						item.collided = true;
						
						if (item.Type == ItemType.element)
						{
							ElementCollision(p1, item);
						}
						switch (item.Name)
						{
						case "Player1Flag":
							item.ResetFlag();
							item.iSprite.Visible = true;
							item.collided = false;
							break;
						case "Player2Flag":
							Player1HoldingFlag = true;
							break;
						default:
							break;
						}
					}
					//check player 2 with items
					if (item.hasCollided(p2.Position, p2Size))
					{
						//Console.WriteLine("Collided with " + item.Name);
						item.iSprite.Visible = false;
						item.collided = true;
						if (item.Type == ItemType.element)
						{
							ElementCollision(p2, item);
						}
						switch (item.Name)
						{
						case "Player1Flag":
							Player2HoldingFlag = true;
							break;
						case "Player2Flag":
							item.ResetFlag();
							item.iSprite.Visible = true;
							item.collided = false;
							break;
						default:
							break;
						}
					}
				}
			}
		}
		
		private void ElementCollision(Player p, Item item)
		{
			if (p.Element != 'N' && p.Element2 != 'N')
			{
				item.iSprite.Visible = true;
				item.collided = false;
			}
			else if (p.Element == 'N')
			{
				p.ChangeTiles(item.Name);
				p.Element = item.Name[0];
			}
			else if (p.Element2 == 'N')
			{
				p.Element2 = item.Name[0];
			}
		}
		
		public void ResetItems()
		{
			foreach (Item item in items)
			{
				item.iSprite.Visible = true;
				item.collided = false;
			}
		}
		
		public void ScoreGameOver(Player p1, Player p2)
		{
			if (Player1HoldingFlag)
			{
				p1.Player1Score();
			}
			else if (Player2HoldingFlag)
			{
				p2.Player2Score();
			}
		}

		public Item GetItem(ItemType type, string name)
		{
			Item toReturn = null;
			foreach (Item item in items)
			{
				if (item.Type == type && item.Name == name)
				{
					toReturn = item;
				}
			}
			return toReturn;
		}
		// For ai to evaluate all the items
		public List<Item> GetAllItems()
		{
			return items;
		}
	}
}




