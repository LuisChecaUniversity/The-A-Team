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
		private SpriteUV blockedAreaSprite;
		private SpriteUV p1baseSprite;
		private SpriteUV p2baseSprite;
		private SpriteUV p1HealthSprite;
		private SpriteUV p2HealthSprite;
		private SpriteUV p1ManaSprite;
		private SpriteUV p2ManaSprite;
		private SpriteUV playerPointer;
		private SpriteUV hudBar;
		
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

			Tile.Loader("/Application/assets/level2.txt", ref player1Pos, ref player2Pos, this);
			
			for (int i = 0; i < Tile.Grid.Count; i++)
			{
				int maxCols = Tile.Grid[i].Count;
				for (int j = 0; j < maxCols; j++)
				{
					if (j < maxCols / 3)
					{
						if (Tile.Grid[i][j].Key == 'E')
						{
							Tile.Grid[i][j].Key = 'A';
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
			p1baseSprite.Position = new Vector2(0, (screenHeight - 32) / 2);
			
			p2baseSprite = new SpriteUV(TextureManager.Get("base"));
			p2baseSprite.Quad.S = p1baseSprite.TextureInfo.TextureSizef;
			p2baseSprite.Position = new Vector2(screenWidth - 64, (screenHeight - 32) / 2);
			
			AddChild(p1baseSprite);
			AddChild(p2baseSprite);
			
			player1 = new Player(player1Pos, true, player1Tiles);
			player2 = new AIPlayer(player2Pos, false, player2Tiles, player1);
			
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
			ItemManager.Instance.initFlags(this);
			
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
			p1HealthSprite.Quad.S = new Vector2(124.0f, 30.0f);
			p1HealthSprite.Position = new Vector2(194, screenHeight - 31);
			
			p2HealthSprite = new SpriteUV(TextureManager.Get("health"));
			p2HealthSprite.Quad.S = new Vector2(124.0f, 30.0f);
			p2HealthSprite.Position = new Vector2(642, screenHeight - 31);
			
			p1ManaSprite = new SpriteUV(TextureManager.Get("mana"));
			p1ManaSprite.Quad.S = new Vector2(124.0f, 30.0f);
			p1ManaSprite.Position = new Vector2(66, screenHeight - 31);
			
			p2ManaSprite = new SpriteUV(TextureManager.Get("mana"));
			p2ManaSprite.Quad.S = new Vector2(124.0f, 30.0f);
			p2ManaSprite.Position = new Vector2(770, screenHeight - 31);
			
			playerPointer = new SpriteUV(TextureManager.Get("pointer"));
			playerPointer.Quad.S = playerPointer.TextureInfo.TextureSizef;
			playerPointer.CenterSprite();
			
			AddChild(hudBar);
			AddChild(p1HealthSprite);
			AddChild(p2HealthSprite);
			AddChild(p1ManaSprite);
			AddChild(p2ManaSprite);
			AddChild(playerPointer);
			
			//Timer Stuff
			lblTimer = new Label();
			lblTimer.FontMap = debugFont;
			lblTimer.Text = ""; // might be worth having a ui to separate class
			lblTimer.Position = new Vector2((screenWidth / 2) - 100, screenHeight - 30);
			AddChild(lblTimer);
			
			timerA = new System.Timers.Timer();
			timerA.Elapsed += new ElapsedEventHandler(tickDown);
			timerA.Interval = 1000;
			timerA.Start();
			timerA.Enabled = true;
			//bool timer.enabled used for pausing
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
			p1HealthSprite.Quad.S = new Vector2(player1.health, 31.0f);
			p2HealthSprite.Quad.S = new Vector2(player2.health, 31.0f);
			
			p1ManaSprite.Quad.S = new Vector2(player1.mana, 31.0f);
			p2ManaSprite.Quad.S = new Vector2(player2.mana, 31.0f);
				
			playerPointer.Rotation = player1.ShootingDirection;
			playerPointer.Position = player1.Position;
			
			player1.Update(dt);
			player2.Update(dt);
			
			if (timerA.Enabled == true)
			{
				lblTimer.Text = "Time Left: " + countMins + ":" + countSecs.ToString().PadLeft(2, '0');
			}
			
			// handle bullet update and collision
			ProjectileManager.Instance.Update(dt);
				
			if (ProjectileManager.Instance.ProjectileCollision(player1.Position, player1.Quad.Bounds2()))
			{
				player1.TakeDamage(10);
			}
			if (ProjectileManager.Instance.ProjectileCollision(player2.Position, player2.Quad.Bounds2()))
			{
				player2.TakeDamage(10);
			}

			for (int i = 0; i < Tile.Collisions.Count; i++)
			{
				Tile t = Tile.Collisions[i];
				char collisionType = ProjectileManager.Instance.ProjectileTileCollision(t.Position, t.Quad.Bounds2());
				if (collisionType != 'X')
				{
					Console.WriteLine(collisionType); // **can hit more then 1 tile at a time**
					t.TakeDamage(collisionType);
				}
				// Remove from collisions if true
				if (t.WallDamage())
				{
					Tile.Collisions.RemoveAt(i);
					i--;
				}
			}

			ItemManager.Instance.Update(dt);
			ItemManager.Instance.ItemCollision(player1, player2);
			ItemManager.Instance.ScoreGameOver(player1, player2);
		}
		
		private void BuildStage(float dt)
		{
			var testtouches = Touch.GetData(0);
			var p1Flag = ItemManager.Instance.GetItem(ItemType.flag, "Player1Flag");
			
			if (player1Deployed == maxDeployed)
			{
				lblTopLeft.Text = "";
			}
			else
			{
				lblTopLeft.Text = "Objects Left: " + (maxDeployed - player1Deployed);
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
					
					foreach (Tile t in player1Tiles)
					{
						if (t.Key == 'E')
						{
							t.Key = 'A';
						}

						if (t.Overlaps(touchVec))
						{
							if (t.Key != 'N' && player1Deployed < maxDeployed)
							{
								// returns player 1 flag and checks if touch pos collides with it
								if (!p1Flag.hasCollided(touchVec, new Vector2(5, 5)))
								{
									t.Key = 'N';
									Tile.Collisions.Add(t);
									player1Deployed++;
								}
							}
							else if (t.Key == 'N')
							{
								t.Key = 'A';
								Tile.Collisions.Remove(t);
								player1Deployed--;
							}
						}
					}
				}

				ItemManager.Instance.Update(dt);
			}

			if (Input2.GamePad0.Start.Down)
			{
				PostBuildStage();
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
					if (t.Key == 'A')
					{
						t.Key = 'E';
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
	}
}

