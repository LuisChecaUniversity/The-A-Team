using System;
using System.Collections.Generic;
using Sce.PlayStation.Core;
using Sce.PlayStation.HighLevel.GameEngine2D;
using Sce.PlayStation.HighLevel.GameEngine2D.Base;
using Sce.PlayStation.Core.Imaging;
using Sce.PlayStation.Core.Audio;
using Sce.PlayStation.Core.Environment;
using Sce.PlayStation.Core.Graphics;
using Sce.PlayStation.Core.Input;
using System.Timers;

namespace TheATeam
{
	enum LevelStage
	{
		BuildStage,
		CombatStage
	}

	public class Level: Scene
	{
		private LevelStage levelStage = LevelStage.BuildStage;
		Player player1;
		Player player2;
		Label lblTopLeft;
		System.Timers.Timer timerA;
		Label lblTimer;
		int countMins = 5; //minutes
		int countSecs = 00; //seconds 
		private Label lblTopRight;
		private int screenWidth;
		private int screenHeight;
		Font font;
		FontMap debugFont;
		// players tiles
		List<Tile> player1Tiles = new List<Tile>();
		List<Tile> player2Tiles = new List<Tile>();
		int maxDeployed = 10;
		int player1Deployed = 0;
		int player2Deployed = 0;
		bool player1Turn = true;
		private SpriteUV blockedAreaSprite;
		private SpriteUV p1baseSprite;
		private SpriteUV p2baseSprite;
		private SpriteUV p1HealthSprite;
		private SpriteUV p1ShieldhpSprite;
		private SpriteUV p2ShieldhpSprite;
		private SpriteUV p2HealthSprite;
		private SpriteUV p1ManaSprite;
		private SpriteUV p2ManaSprite;
		private SpriteUV playerPointer;
		private SpriteTile[] UIElements;
		private SpriteUV hudBar;
		private bool pointerOn = false;
		
		public Level(): base()
		{
			Info.IsGameOver = false;
			screenWidth = Director.Instance.GL.Context.Screen.Width;
			screenHeight = Director.Instance.GL.Context.Screen.Height;

			font = new Font(FontAlias.System, 25, FontStyle.Bold);
			debugFont = new FontMap(font, 25);
			// Reload the font becuase FontMap disposes of it
			font = new Font(FontAlias.System, 25, FontStyle.Bold);
			
			InitMain();
			InitExtras();
			InitUI();
			
			Camera2D.SetViewFromViewport();
		}
		
		private void InitMain()
		{
			AddChild(new Background());
						
			Vector2 player1Pos = Vector2.Zero;
			Vector2 player2Pos = Vector2.Zero;
			Vector2 p1Flag = new Vector2(32, (screenHeight + 32) / 2);
			Vector2 p2Flag = new Vector2(screenWidth - 32, (screenHeight + 32) / 2);
			
			string level = "/Application/assets/level" + Info.Rnd.Next(2,6) + ".txt";
			if(AppMain.TYPEOFGAME == "DUAL")
				level = "/Application/assets/level1.txt";
			Tile.Loader(level, ref player1Pos, ref player2Pos, ref p1Flag, ref p2Flag, this);
			
			for (int i = 0; i < Tile.Grid.Count; i++)
			{
				int maxCols = Tile.Grid[i].Count;
				for (int j = 0; j < maxCols; j++)
				{
					if (j < maxCols / 3)
					{
						if (Tile.Grid[i][j].Key == '_')
						{
							Tile.Grid[i][j].Key = 'B';
						}
						player1Tiles.Add(Tile.Grid[i][j]);
					}
					else if (j >= (maxCols / 3) * 2)
					{
						player2Tiles.Add(Tile.Grid[i][j]);
					}
				}
			}
			
			p1baseSprite = new SpriteUV(TextureManager.Get("base"));
			p1baseSprite.Quad.S = p1baseSprite.TextureInfo.TextureSizef;
			p1baseSprite.Position = p1Flag;//new Vector2(p1baseSprite.Quad.S.X/2, (screenHeight + 32) / 2);
			p1baseSprite.CenterSprite();
			
			p2baseSprite = new SpriteUV(TextureManager.Get("base"));
			p2baseSprite.Quad.S = p1baseSprite.TextureInfo.TextureSizef;
			p2baseSprite.Position = p2Flag;//new Vector2(screenWidth - p2baseSprite.Quad.S.X/2, (screenHeight + 32) / 2);
			p2baseSprite.CenterSprite();
			
			AddChild(p1baseSprite);
			AddChild(p2baseSprite);
			
			Info.P1 = player1 = new Player(player1Pos, true, player1Tiles);
			if(AppMain.TYPEOFGAME == "SINGLE")
				Info.P2 = player2 = new AIPlayer(player2Pos, false, player2Tiles, player1);
			else if(AppMain.TYPEOFGAME == "DUAL")
			{
				Info.P2 = player2 = new Player(player2Pos, false, player2Tiles);
				player2.Update(0.0f);
			}
			
			player1.Update(0.0f);
			
			
			AddChild(player1);
			AddChild(player2);
		}
				
		private void InitExtras()
		{
			blockedAreaSprite = new SpriteUV(TextureManager.Get("blockedArea"));
			blockedAreaSprite.Quad.S = blockedAreaSprite.TextureInfo.TextureSizef;
			blockedAreaSprite.Position = new Vector2(screenWidth / 2, 0.0f);

			lblTopLeft = new Label();
			lblTopLeft.FontMap = debugFont;
			lblTopLeft.Text = "";
			lblTopLeft.Position = new Vector2(screenWidth / 2 + 140, screenHeight / 2 + 50);
			
			lblTopRight = new Label();
			lblTopRight.FontMap = debugFont;
			lblTopRight.Text = "Press Start to Continue";
			lblTopRight.Position = new Vector2(screenWidth / 2 + 100, screenHeight / 2 - 150);
			
			ItemManager.Instance.initFlags(this, p1baseSprite.Position, p2baseSprite.Position);
			
			AddChild(blockedAreaSprite);
			AddChild(lblTopLeft);
			AddChild(lblTopRight);
		}

		private void InitUI()
		{
			hudBar = new SpriteUV(TextureManager.Get("hudbar"));			
			hudBar.Quad.S = hudBar.TextureInfo.TextureSizef;
			hudBar.Position = new Vector2(0, 0);
			
			p1HealthSprite = new SpriteUV(TextureManager.Get("health"));			
			p1HealthSprite.Quad.S = new Vector2(122.0f, 26.0f);
			p1HealthSprite.Position = new Vector2(195, screenHeight - 29);
			
			p2HealthSprite = new SpriteUV(TextureManager.Get("health"));
			p2HealthSprite.Quad.S = new Vector2(122.0f, 26.0f);
			p2HealthSprite.Position = new Vector2(643, screenHeight - 29);
			
			p1ShieldhpSprite = new SpriteUV(TextureManager.Get("shieldhp"));			
			p1ShieldhpSprite.Quad.S = new Vector2(122.0f, 26.0f);
			p1ShieldhpSprite.Position = new Vector2(195, screenHeight - 29);
			p1ShieldhpSprite.Visible = false;
			
			p2ShieldhpSprite = new SpriteUV(TextureManager.Get("shieldhp"));
			p2ShieldhpSprite.Quad.S = new Vector2(122.0f, 26.0f);
			p2ShieldhpSprite.Position = new Vector2(643, screenHeight - 29);
			p2ShieldhpSprite.Visible = false;
			
			p1ManaSprite = new SpriteUV(TextureManager.Get("mana"));
			p1ManaSprite.Quad.S = new Vector2(122.0f, 26.0f);
			p1ManaSprite.Position = new Vector2(67, screenHeight - 29);
			
			p2ManaSprite = new SpriteUV(TextureManager.Get("mana"));
			p2ManaSprite.Quad.S = new Vector2(122.0f, 26.0f);
			p2ManaSprite.Position = new Vector2(771, screenHeight - 29);
			
			
			
			AddChild(hudBar);
			AddChild(p1HealthSprite);
			AddChild(p2HealthSprite);
			AddChild(p1ShieldhpSprite);
			AddChild(p2ShieldhpSprite);
			AddChild(p1ManaSprite);
			AddChild(p2ManaSprite);
			
			if(pointerOn)
			{
				playerPointer = new SpriteUV(TextureManager.Get("pointer"));
				playerPointer.Quad.S = playerPointer.TextureInfo.TextureSizef;
				playerPointer.CenterSprite();
				AddChild(playerPointer);
			}
			
			InitUIElements();
			
			//Timer Stuff
			lblTimer = new Label();
			
			FontMap fontl = new FontMap(new Font("Application/assets/LaSegunda.ttf", 28, FontStyle.Regular), 512);
			lblTimer.FontMap = fontl;
			lblTimer.Color = Colors.Grey80;
			lblTimer.Text = ""; // might be worth having a ui to separate class
			lblTimer.Position = new Vector2((screenWidth / 2) - 100, screenHeight - 32);
			AddChild(lblTimer);
			
			timerA = new System.Timers.Timer();
			timerA.Elapsed += new ElapsedEventHandler(tickDown);
			timerA.Interval = 1000;
			timerA.Start();
			timerA.Enabled = true;
			//bool timer.enabled used for pausing
		}

		private void InitUIElements()
		{
			UIElements = new SpriteTile[4];
			for (int i = 0; i < 4; i++)
			{
				UIElements[i] = new SpriteTile();
				UIElements[i].TextureInfo = TextureManager.Get("items");
				UIElements[i].Quad.S = new Vector2(28.0f, 28.0f);
				UIElements[i].TileIndex2D = new Vector2i(0, 0);
				UIElements[i].Color = new Vector4(1.0f, 1.0f, 1.0f, 0.0f);
				UIElements[i].CenterSprite();

				AddChild(UIElements[i]);
			}
			
			UIElements[0].Position = new Vector2(18, screenHeight - 15);
			UIElements[1].Position = new Vector2(48, screenHeight - 15);
			UIElements[2].Position = new Vector2(912, screenHeight - 15);
			UIElements[3].Position = new Vector2(942, screenHeight - 15);

		}

		private void tickDown(object sender, EventArgs e)
		{
			if (levelStage == LevelStage.CombatStage)
			{
				countSecs --;
				//countSecs get seconds to round to 2 figures ie 01, 09
				lblTimer.Text = "Time Left: " + countMins + ":" + countSecs.ToString().PadLeft(2, '0');
				
				if (countSecs < 0 && countMins == 0)
				{
					lblTimer.Text = "Game Over";
					Info.IsGameOver = true;
					timerA.Stop();
					
				}	
				
				if (countSecs < 0)
				{
					countSecs = 59;
					countMins = countMins - 1;
				}
			}
		}

		public override void Update(float dt)
		{
			base.Update(dt);
			
			if (Info.IsGameOver)
			{
				//need to dispose each thing in order to reinit on next game!!!!!!!! TODO
				GameOver go = new GameOver();
				GameSceneManager.currentScene = go;
				Director.Instance.ReplaceScene(go);	
			}
			
			if (levelStage == LevelStage.CombatStage)
			{		
				CombatStage(dt);
			}
			else if (levelStage == LevelStage.BuildStage)
			{
				BuildStage(dt);
			}				
		}
		
		private void CombatStage(float dt)
		{
			p1HealthSprite.Quad.S = new Vector2(player1.Health, 26.0f);
			p2HealthSprite.Quad.S = new Vector2(player2.Health, 26.0f);
			
			p1ShieldhpSprite.Visible = true;
			p2ShieldhpSprite.Visible = true;
			p1ShieldhpSprite.Quad.S = new Vector2(player1.Shield, 26.0f);
			p2ShieldhpSprite.Quad.S = new Vector2(player2.Shield, 26.0f);
			
			p1ManaSprite.Quad.S = new Vector2(player1.Mana, 26.0f);
			p2ManaSprite.Quad.S = new Vector2(player2.Mana, 26.0f);
			
			if(pointerOn)
			{
				playerPointer.Rotation = player1.GetShootingDirection();
				playerPointer.Position = player1.Position;
			}
			
			player1.Update(dt);
			player2.Update(dt);
			
			if (timerA.Enabled == true)
			{
				lblTimer.Text = "Time Left: " + countMins + ":" + countSecs.ToString().PadLeft(2, '0');
			}
			
			// handle bullet update and collision
			ProjectileManager.Instance.Update(dt);
			Projectile collidingProjectile = ProjectileManager.Instance.ProjectileCollision(player1);
			if (collidingProjectile != null)//ProjectileManager.Instance.ProjectileCollision(player1))//.Position, player1.Quad.Bounds2()))
			{
				player1.TakeDamage(collidingProjectile.bulletDamage);
				if(collidingProjectile.getType() == Type.FireAir)
					player1.isSlowed(true);
			}
			
			collidingProjectile = ProjectileManager.Instance.ProjectileCollision(player2);
			if (collidingProjectile != null)//ProjectileManager.Instance.ProjectileCollision(player2))//.Position, player2.Quad.Bounds2()))
			{
				player2.TakeDamage(collidingProjectile.bulletDamage);
				if(collidingProjectile.getType() == Type.FireAir)
					player2.isSlowed(true);
			}

			for (int i = 0; i < Tile.Collisions.Count; i++)
			{
				Tile t = Tile.Collisions[i];
				collidingProjectile = ProjectileManager.Instance.ProjectileCollision(t);
				if (collidingProjectile != null)
				{
					Console.WriteLine(collidingProjectile.GetPlayer().Element); // **can hit more then 1 tile at a time**
					t.TakeDamage(collidingProjectile.GetPlayer().Element);
					
//					char collisionType = collidingProjectile.GetPlayer().Element;//ProjectileManager.Instance.ProjectileTileCollision(t.Position, t.Quad.Bounds2());
//					
//					if (collisionType != 'X')
//					{
//						Console.WriteLine(collisionType); // **can hit more then 1 tile at a time**
//						t.TakeDamage(collidingProjectile.bulletDamage);
//					}
				}
				// Remove from collisions if true
				if (t.WallDamage(dt))
				{
					Tile.Collisions.RemoveAt(i);
					i--;
				}
			}
			
			// Handle player shield collision effect
			player1.ShieldCollision(player2);
			player2.ShieldCollision(player1);
			
			UpdateUIElements();
			ItemManager.Instance.ItemCollision(player1, player2);
			ItemManager.Instance.ScoreGameOver(player1, player2);
		}

		private void BuildStage(float dt)
		{
			if(AppMain.TYPEOFGAME == "SINGLE")
			{
				if (player1Deployed == maxDeployed)
				{
					lblTopLeft.Text = "Maximum Deployed";
				}
				else
				{
					lblTopLeft.Text = "Objects Left: " + (maxDeployed - player1Deployed);
				}
				
				PlaceDefence(player1);
				
	
				if (Input2.GamePad0.Start.Down)
				{
					PostBuildStage();
				}
			}
			else if(AppMain.TYPEOFGAME == "DUAL")
			{
				if(player1Turn)
				{
					lblTopRight.Text = "Press Square for Player 2\n          Deployment";
					if (player1Deployed == maxDeployed)
					{
						lblTopLeft.Text = "Maximum Deployed";
					}
					else
					{
						lblTopLeft.Text = "Objects Left: " + (maxDeployed - player1Deployed);
					}
					PlaceDefence(player1);
		
					if (Input2.GamePad0.Square.Down)
					{
						player1Turn = false;
					}
				}
				else
				{
					blockedAreaSprite.Position = new Vector2(0.0f, 0.0f);			
					lblTopLeft.Position = new Vector2(140, screenHeight / 2 + 50);					
					lblTopRight.Position = new Vector2(100, screenHeight / 2 - 150);
					lblTopRight.Text = "Press Start to Continue";
					
					if (player2Deployed == maxDeployed)
					{
						lblTopLeft.Text = "Maximum Deployed";
					}
					else
					{
						lblTopLeft.Text = "Objects Left: " + (maxDeployed - player2Deployed);
					}
					PlaceDefence(player2);
		
					if (Input2.GamePad0.Start.Down)
					{
						PostBuildStage();
					}
				}
				
				
			}
		}
		private void PlaceDefence(Player player)
		{
			var testtouches = Touch.GetData(0);
			List<Tile> playerTiles = new List<Tile>();
			int playerDeployed = 0;
			if(player == player1)
			{
				playerTiles = player1Tiles;
				playerDeployed = player1Deployed;
			}
			else if(player == player2)
			{
				playerTiles = player2Tiles;
				playerDeployed = player2Deployed;
			}
			
			if (testtouches.Count > 0)
			{
				float screenheight = Director.Instance.GL.Context.GetViewport().Height;
				float screenwidth = Director.Instance.GL.Context.GetViewport().Width;
				float screenx = (testtouches[0].X + 0.5f) * screenwidth;
				float screenY = screenHeight - (testtouches[0].Y + 0.5f) * screenheight;
				Vector2 touchVec = new Vector2(screenx, screenY);
					
				if (testtouches[0].Status == TouchStatus.Down)
				{
					Console.WriteLine("Touched" + touchVec);
					Console.WriteLine(player1Tiles[0].Position);
					
					
					foreach (Tile t in playerTiles)
					{
						if (t.Key == '_')
						{
							t.Key = 'B';
						}

						if (t.Overlaps(touchVec))
						{
							if (t.Key != 'N' && playerDeployed < maxDeployed)
							{
								// returns player 1 flag and checks if touch pos collides with it
								if (!t.Overlaps(p1baseSprite) || !t.Overlaps(p2baseSprite))//!p1Flag.hasCollided(touchVec, new Vector2(6, 6)))
								{
									t.Key = 'N';
									Tile.Collisions.Add(t);
									playerDeployed++;
								}
							}
							else if (t.Key == 'N')
							{
								t.Key = 'B';
								Tile.Collisions.Remove(t);
								playerDeployed--;
							}
						}
					}
				}
			}
			
			if(player == player1)
			{
				player1Tiles = playerTiles;
				player1Deployed = playerDeployed;
			}
			else if(player == player2)
			{
				player2Tiles = playerTiles;
				player2Deployed = playerDeployed;
			}
		}
		
		private void PostBuildStage()
		{
			this.RemoveChild(blockedAreaSprite, true);
			this.RemoveChild(lblTopLeft, true);
			this.RemoveChild(lblTopRight, true);
			levelStage = LevelStage.CombatStage;
			
			for (int i = 0; i < Tile.Grid.Count; i++)
			{
				for (int j = 0; j < Tile.Grid[i].Count; j++)
				{
					Tile t = Tile.Grid[i][j];
					if (t.Key == 'B')
					{
						t.Key = '_';
					}
					else if (t.Key == 'N')
					{
						if (i > 0)
						{
							if (Tile.Grid[i - 1][j].Key == 'N')
							{
								t.Sides += (int)Sides.Top;
							}
						}
						if (j > 0)
						{
							if (Tile.Grid[i][j - 1].Key == 'N')
							{
								t.Sides += (int)Sides.Left;
							}
						}
						if (j < Tile.Grid[i].Count - 1)
						{
							if (Tile.Grid[i][j + 1].Key == 'N')
							{
								t.Sides += (int)Sides.Right;
							}
						}
						if (i < Tile.Grid.Count - 1)
						{
							if (Tile.Grid[i + 1][j].Key == 'N')
							{
								t.Sides += (int)Sides.Bottom;
							}
						}
					}
				}
			}
			
			ItemManager.Instance.initElements(this);
		}

		private void UpdateUIElements()
		{
			UpdateUIElement(0, player1.Element);
			UpdateUIElement(1, player1.Element2);
			UpdateUIElement(2, player2.Element);
			UpdateUIElement(3, player2.Element2);
		}
		
		private void UpdateUIElement(int index, char c)
		{
			if (c != 'N')
			{
				UIElements[index].TileIndex2D.Y = Tile.Elements.Count - Tile.Elements.IndexOf(c);
				UIElements[index].Color = new Vector4(1.0f);
			}
			else
			{
				UIElements[index].Color = new Vector4(0.0f);
			}
		}
	}
}

