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
		public List<Item> Items { get { return items;}}
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

		public void initFlags(Scene curScene, Vector2 p1Flag, Vector2 p2Flag)
		{
			Player1HoldingFlag = false;
			Player2HoldingFlag = false;
			FirstFlag = p1Flag;//new Vector2(30, 290);
			SecondFlag = p2Flag;//new Vector2(926, 290);
			leftFlag = new Item(curScene, FirstFlag, flagIndex, ItemType.flag, "Player1Flag");
			rightFlag = new Item(curScene, SecondFlag, flagIndex, ItemType.flag, "Player2Flag");
			
			items.Clear();
			items.Add(leftFlag);
			items.Add(rightFlag);
		}

		public void initElements(Scene curScene)
		{
//			Vector2 pos1 = new Vector2(400, 50);
//			Vector2 pos2 = new Vector2(570, 140);
//			Vector2 pos3 = new Vector2(480, 250);
//			Vector2 pos4 = new Vector2(390, 370);
//			Vector2 pos5 = new Vector2(560, 450);
			
			List<int> indexList = new List<int>();
			for (int i = 0; i < 5; i++) 
			{
				int r = Info.Rnd.Next(0,5);
				while(indexList.Contains(r))
				{
					r = Info.Rnd.Next(0,5);
				}
				
				indexList.Add(r);
				//Console.WriteLine(r);
			}
			
			lightningElement = new Item(curScene, RandomPosition(indexList[0]), lightningIndex, ItemType.element, "Lightning");
			airElement = new Item(curScene, RandomPosition(indexList[1]), airIndex, ItemType.element, "Air");
			earthElement = new Item(curScene, RandomPosition(indexList[2]), earthIndex, ItemType.element, "Earth");
			fireElement = new Item(curScene, RandomPosition(indexList[3]), fireIndex, ItemType.element, "Fire");
			waterElement = new Item(curScene, RandomPosition(indexList[4]), waterIndex, ItemType.element, "Water");
			
			items.Add(lightningElement);
			items.Add(airElement);
			items.Add(earthElement);
			items.Add(fireElement);
			items.Add(waterElement);
		}
		public void initElements(Scene curScene,bool isMultiplayerSetup)
		{			
			string mess = AppMain.client.layoutMessage;
			int i = 0;
			int finalLoop = 1;
			int phase = 1;
			while ((i = mess.IndexOf('Q',i)) != -1)
			{
				char element = mess[i+1];
				int pos = mess.IndexOf('S',phase);
				phase = pos;
				string t = mess.Substring(i+2,pos - (i+2));
				float xPos = float.Parse(t);
				
				string tester = "";
				int startTest = mess.IndexOf('S',pos );
				if(finalLoop==5)
				{
					int len = mess.Length - startTest;
					 tester = mess.Substring(startTest+1,len-1);
				}
				else
				{
					int endTest = -1;
					if(mess[startTest +3].Equals('Q'))
						endTest = startTest+2;
					else if(mess[startTest+4].Equals('Q'))
						endTest = startTest+3;
					tester = mess.Substring(startTest+1,endTest-startTest);
				}
				float yPos = float.Parse(tester);
				
				switch (element)
				{
				case 'A':
					airElement = new Item(curScene, new Vector2(xPos,yPos), airIndex, ItemType.element, "Air");
					break;
				case 'E':
					earthElement = new Item(curScene,new Vector2(xPos,yPos), earthIndex, ItemType.element, "Earth");
					break;
				case 'F':
					fireElement = new Item(curScene, new Vector2(xPos,yPos), fireIndex, ItemType.element, "Fire");
					break;
				case 'W':
					waterElement = new Item(curScene,new Vector2(xPos,yPos), waterIndex, ItemType.element, "Water");
					break;
				case 'L':
					lightningElement = new Item(curScene, new Vector2(xPos,yPos), lightningIndex, ItemType.element, "Lightning");
					break;
				default:
					break;
				}
				
				i++;
				phase++;
				finalLoop++;
			}
			
			items.Add(lightningElement);
			items.Add(airElement);
			items.Add(earthElement);
			items.Add(fireElement);
			items.Add(waterElement);


		}
		private Vector2 RandomPosition(int i)
		{
			Vector2 pos = Vector2.Zero;
			switch(i)
			{
			case 0:
				pos = new Vector2(400, 50);
				break;
			case 1:
				pos = new Vector2(570, 140);
			break;
			case 2:
				pos = new Vector2(480, 250);
				break;
			case 3:
				pos = new Vector2(390, 370);
				break;
			case 4:
				pos = new Vector2(560, 450);
				break;
			}
			return pos;
			
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
							//item.ResetFlag();
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
					else if (item.hasCollided(p2.Position, p2Size))
					{
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
							//item.ResetFlag();
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
				p.ElementBuff(item.Name);
			}
			else if (p.Element2 == 'N')
			{
				p.Element2 = item.Name[0];
				p.ElementBuff(item.Name);
			}
		}
		
		public void ResetItems(Player p)
		{
			if (p.whichPlayer == PlayerIndex.PlayerOne)
			{
				Player1HoldingFlag = false;
				
			}
			else if (p.whichPlayer ==PlayerIndex.PlayerTwo)
			{
				Player2HoldingFlag = false;
			}
			       
			foreach (Item item in items)
			{
				if(item.collided)
				{
					if (p.Element == item.Name[0] || p.Element2 == item.Name[0])
					 {
						item.iSprite.Visible = true;
						item.collided = false;
						
					}
					if(item.Name == "Player2Flag" && p.whichPlayer == PlayerIndex.PlayerOne)
					{
						Player1HoldingFlag = false;
						item.iSprite.Visible = true;
						item.collided = false;
						
					}
					else if (item.Name == "Player1Flag" && p.whichPlayer ==PlayerIndex.PlayerTwo)
					{
						//Console.WriteLine("Reset " + item.Name);	
						Player2HoldingFlag = false;
						item.iSprite.Visible = true;
						item.collided = false;
					}
				}
			}
		}
		
		public void ScoreGameOver(Player p1, Player p2)
		{
			if (Player1HoldingFlag && !Player2HoldingFlag)
			{
				p1.Player1Score();
			}
			else if (Player2HoldingFlag && !Player1HoldingFlag)
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




