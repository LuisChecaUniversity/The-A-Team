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

namespace TheATeam
{
	enum LevelStage
	{
		BuildDefence,
		CombatStage
	}

	public class Level: Scene
	{
		private LevelStage levelStage = LevelStage.BuildDefence;
		Player player1;
		Player player2;
		Label lblTopLeft;
		private Label lblTopRight;
		private Label lblBottomLeft;
		private Label lblBottomRight;
		private Label lblDebugLeft;
		private int screenWidth;
		private int screenHeight;
		Font font;
		FontMap debugFont;
		private SpriteUV blockedAreaSprite;
		private TextureInfo blockedAreaTexInfo;
		// players tiles
		List<Tile> player1Tiles = new List<Tile>();
		List<Tile> player2Tiles = new List<Tile>();
		int maxDeployed = 10;
		int player1Depolyed = 0;
		
		public Level(): base()
		{

			screenWidth = Director.Instance.GL.Context.Screen.Width;
			screenHeight = Director.Instance.GL.Context.Screen.Height;

			font = new Font(FontAlias.System, 25, FontStyle.Bold);
			debugFont = new FontMap(font, 25);

			// Reload the font becuase FontMap disposes of it
			font = new Font(FontAlias.System, 25, FontStyle.Bold);
			Info.LevelClear = false;
			Vector2 cameraCenter = Vector2.Zero;


			AddChild(new Background());
			
			
			Tile.Loader("/Application/assets/level1.txt", ref cameraCenter, this);
			Info.CameraCenter = cameraCenter;
			
			for (int i = 0; i < 8; i++) 
				{
					for (int j = 0; j < 5; j++) 
					{
						player1Tiles.Add(Tile.Grid[i][j]);
					
					}
				}
			
			for (int i = 0; i < 8; i++) 
				{
					for (int j = 10; j < 15; j++) 
					{
						player2Tiles.Add(Tile.Grid[i][j]);
					
					}
				}
			
			player1 = new Player(cameraCenter, true,player1Tiles);
			player2 = new Player(new Vector2(960 - 164, 300), false,player2Tiles);
			
			blockedAreaTexInfo = new TextureInfo("/Application/assets/BlockedArea.png");
			
			blockedAreaSprite = new SpriteUV(blockedAreaTexInfo);
			blockedAreaSprite.Quad.S = blockedAreaTexInfo.TextureSizef;
			blockedAreaSprite.Position = new Vector2(screenWidth / 2, 0.0f);
			
			
			if(AppMain.TYPEOFGAME.Equals("MULTIPLAYER"))
			{

				lblTopLeft = new Label();
				lblTopLeft.FontMap = debugFont;
				lblTopLeft.Text = "Player 1";
				lblTopLeft.Position = new Vector2(100, screenHeight - 200);

				lblTopRight = new Label();
				lblBottomLeft = new Label();
				lblBottomRight = new Label();
				lblDebugLeft = new Label();

				lblTopRight.FontMap = debugFont;
				lblTopRight.Text = "Player 2";
				lblTopRight.Position = new Vector2(screenWidth - 200, screenHeight - 200);

				lblBottomLeft.FontMap = debugFont;
				lblBottomLeft.Text = "Waiting";
				lblBottomLeft.Position = new Vector2(100, 300);

				lblBottomRight.FontMap = debugFont;
				lblBottomRight.Text = "Waiting";
				lblBottomRight.Position = new Vector2(screenWidth - 200, 300);

				lblDebugLeft.FontMap = debugFont;
				lblDebugLeft.Text = "Waiting for both connections";
				lblDebugLeft.Position = new Vector2(430, 200);

				this.AddChild(lblTopRight);
				this.AddChild(lblBottomLeft);
				this.AddChild(lblBottomRight);
				this.AddChild(lblDebugLeft);
				this.AddChild(lblTopLeft);
			}
			else
			{
				lblTopLeft = new Label();
				lblTopLeft.FontMap = debugFont;
				lblTopLeft.Text = "";
				lblTopLeft.Position = new Vector2(screenWidth / 2 + 140, screenHeight / 2 + 50);
				
				lblTopRight = new Label();
				lblTopRight.FontMap = debugFont;
				lblTopRight.Text = "Press Start to Continue";
				lblTopRight.Position = new Vector2(screenWidth / 2 + 100, screenHeight / 2 - 150);
				

			
			this.AddChild(player1);
			this.AddChild(player2);
			this.AddChild(blockedAreaSprite);
			this.AddChild(lblTopLeft);
			this.AddChild(lblTopRight);
			Camera2D.SetViewFromViewport();

			}
//			Schedule((dt) => {
//				Info.TotalGameTime += dt;
//				// Camera2D.SetViewFromHeightAndCenter(Info.CameraHeight, Info.CameraCenter);
//			});
		}

		public override void Update(float dt)
		{
			base.Update(dt);

			if(AppMain.TYPEOFGAME.Equals("MULTIPLAYER"))
			{
				if(levelStage == LevelStage.CombatStage)
				{
					string status = AppMain.client.statusString;
					if(status.Equals("None"))
					{
						AppMain.client.ChangeStatus();
						lblDebugLeft.Text = "Changing";
					}
					else
						lblDebugLeft.Text = status;
	
					if(AppMain.ISHOST)
					{
						player1.Update(dt);
						AppMain.client.DataExchange();
						player2.Update(dt);
					}
					else
					{
						player2.Update(dt);
						AppMain.client.DataExchange();
						player1.Update(dt);
					}
				}
				else if(levelStage == LevelStage.BuildDefence)
				{
					
				}
			}
			else if(AppMain.TYPEOFGAME.Equals("SINGLE"))
			{
				if(levelStage == LevelStage.CombatStage)
				{
//					if(Input2.GamePad0.Triangle.Down)
//						ChangeTiles("Fire");
					
					player1.Update(dt);
					player2.UpdateAI(dt, player1);
					
					// handle bullet update and collision
					ProjectileManager.Instance.Update(dt);
		
					if(ProjectileManager.Instance.ProjectileCollision(player1.Position, player1.Quad.Bounds2()))
						Console.WriteLine("Player 1 got hit");
					if(ProjectileManager.Instance.ProjectileCollision(player2.Position, player2.Quad.Bounds2()))
						Console.WriteLine("Player 2 got hit");
		
		
					foreach(Tile t in Tile.Collisions)
					{
						char collisionType = ProjectileManager.Instance.ProjectileTileCollision(t.Position, t.Quad.Bounds2());
						if(collisionType != 'X')
						{
							Console.WriteLine(collisionType); // **can hit more then 1 tile at a time**
							t.TakeDamage(collisionType);
	
						}
						
					}
					
					ItemManager.Instance.Update(dt);
					ItemManager.Instance.ItemCollision(player1, player2);
					
						
				}
				else if(levelStage == LevelStage.BuildDefence)
				{
					var testtouches = Touch.GetData(0);
					if(player1Depolyed == maxDeployed)
						lblTopLeft.Text = "";
					else
						lblTopLeft.Text = "Objects Left: " + (maxDeployed - player1Depolyed);
					
					
					if(testtouches.Count > 0)
					{
						float screenheight = Director.Instance.GL.Context.GetViewport().Height;
						float screenwidth = Director.Instance.GL.Context.GetViewport().Width;
						float screenx = (testtouches[0].X + 0.5f) * screenwidth;
						float screenY = screenHeight - (testtouches[0].Y + 0.5f) * screenheight;
						Vector2 touchVec = new Vector2(screenx, screenY);
						
						if(testtouches[0].Status == TouchStatus.Down)
						{
							Console.WriteLine("Touched" + touchVec);
							
							Console.WriteLine(player1Tiles[0].Position);
							foreach(Tile t in player1Tiles)
							{
								if(t.Key == 'E')
									t.Key = 'A';
								
								if(touchVec.X > t.Position.X && touchVec.X < t.Position.X + 64 &&
								   touchVec.Y > t.Position.Y && touchVec.Y < t.Position.Y + 64)
								{
									if(t.Key != 'N')
									{
										if(player1Depolyed < maxDeployed)
										{
											// returns player 1 flag and checks if touch pos collides with it
											if(!ItemManager.Instance.GetItem(ItemType.flag, "Player1Flag").hasCollided(touchVec, new Vector2(5, 5)))
											{
												t.Key = 'N';
												Tile.Collisions.Add(t);
												player1Depolyed++;
											}
										}
									}
									else if(t.Key == 'N')
									{
										t.Key = 'A';
										Tile.Collisions.Remove(t);
										player1Depolyed--;
									}
								}
							}
						}
						
						ItemManager.Instance.Update(dt);
						
					}
					
					if(Input2.GamePad0.Start.Down)
					{
						this.RemoveChild(blockedAreaSprite, true);
						this.RemoveChild(lblTopLeft, true);
						this.RemoveChild(lblTopRight, true);
						levelStage = LevelStage.CombatStage;
						foreach(Tile t in player1Tiles)
						{
							if(t.Key == 'A')
								t.Key = 'E';
						}
						ItemManager.Instance.initElements();
						ItemManager.Instance.initFlags();
					}
					
				}
				
			}


			

		}

	}
}



