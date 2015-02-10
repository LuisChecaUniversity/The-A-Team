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
	
	public class MultiplayerLevel: Scene
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
		int playerDepolyed = 0;
		
		bool startDeploying;
		bool playerReady = false;
		float timeLeft = 30.0f;
		
		public MultiplayerLevel()
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
			blockedAreaSprite.Quad.S 	= blockedAreaTexInfo.TextureSizef;
			
			if(!AppMain.ISHOST)
			{
				blockedAreaSprite.Position = new Vector2(screenWidth/2, 0.0f);
				lblTopLeft = new Label();
				lblTopLeft.FontMap = debugFont;
				lblTopLeft.Text = "";
				lblTopLeft.Position = new Vector2(screenWidth/2 + 140, screenHeight/2 + 50 );
				
				lblTopRight = new Label();
				lblTopRight.FontMap = debugFont;
				lblTopRight.Text = "Press Start to Deploy!";
				lblTopRight.Position = new Vector2(screenWidth/2 + 100, screenHeight/2- 150);
			}
			else
			{
				blockedAreaSprite.Position = new Vector2(0, 0.0f);
				lblTopLeft = new Label();
				lblTopLeft.FontMap = debugFont;
				lblTopLeft.Text = "";
				lblTopLeft.Position = new Vector2(140, screenHeight/2 + 50 );
				
				lblTopRight = new Label();
				lblTopRight.FontMap = debugFont;
				lblTopRight.Text = "Press Start to Deploy!";
				lblTopRight.Position = new Vector2(100, screenHeight/2- 150);
			}

				
				

			
			
			this.AddChild(player1);
			this.AddChild(player2);
			this.AddChild(blockedAreaSprite);
			this.AddChild(lblTopLeft);
			this.AddChild(lblTopRight);
			Camera2D.SetViewFromViewport();


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
					AppMain.client.DataExchange();
					
					string status = AppMain.client.statusString;
					if(status.Equals("None"))
					{
						AppMain.client.ChangeStatus();
						lblTopLeft.Text = "Changing";
					}
					else
						lblTopLeft.Text = status;
					
					//check if both players pressed start;
					
						if(!startDeploying)
						{
							if(!playerReady)
							{
								if(Input2.GamePad0.Start.Down)
								{
									AppMain.client.SetActionMessage('R');
									playerReady = true;
								}
								
							}
								if(AppMain.ISHOST)
									lblTopRight.Text = "HOST : " + AppMain.client.ActionMsg.ToString() + " Client: " + AppMain.client.NetworkActionMsg.ToString();
								else
									lblTopRight.Text = "Client : " + AppMain.client.ActionMsg.ToString() + "HOST : " + AppMain.client.NetworkActionMsg.ToString();
							
							if(AppMain.client.NetworkActionMsg.Equals('R') && AppMain.client.ActionMsg.Equals('R'))
							{
									startDeploying = true;
									AppMain.client.SetActionMessage('D');
							}
						}
						else if(startDeploying)
						{
							if(Input2.GamePad0.Start.Down)
							{
								AppMain.client.SetActionMessage('R');
							}
						
							if(AppMain.client.NetworkActionMsg.Equals('R') && AppMain.client.ActionMsg.Equals('R'))
							{
									
								AppMain.client.SetActionMessage('D');
								levelStage = LevelStage.CombatStage;
							}
						
							if(AppMain.ISHOST)
								DeployUpdate(player1,dt);
							else
								DeployUpdate(player2,dt);
						}
				}
			}
		}
		
		private void DeployUpdate(Player player, float dt)
		{
			var testtouches = Touch.GetData(0);
					if(playerDepolyed == maxDeployed)
						lblTopLeft.Text = "";
					else
						lblTopLeft.Text = "Objects Left: " + (maxDeployed - playerDepolyed);
					
					
					if(testtouches.Count > 0)
					{
						float screenheight = Director.Instance.GL.Context.GetViewport().Height;
						float screenwidth = Director.Instance.GL.Context.GetViewport().Width;
						float screenx = (testtouches[0].X + 0.5f) * screenwidth;
						float screenY = screenHeight - (testtouches[0].Y + 0.5f) * screenheight;
						Vector2 touchVec = new Vector2(screenx, screenY);
						
						if(testtouches[0].Status == TouchStatus.Down)
						{
							foreach(Tile t in player.playerTiles)
							{
								if(t.Key == 'E')
									t.Key = 'A';
								
								if(touchVec.X > t.Position.X && touchVec.X < t.Position.X + 64 &&
								   touchVec.Y > t.Position.Y && touchVec.Y < t.Position.Y + 64)
								{
									if(t.Key != 'N')
									{
										if(playerDepolyed < maxDeployed)
										{
											// returns player 1 flag and checks if touch pos collides with it
											if(!ItemManager.Instance.GetItem(ItemType.flag, "Player1Flag").hasCollided(touchVec, new Vector2(5, 5)))
											{
												t.Key = 'N';
												Tile.Collisions.Add(t);
												playerDepolyed++;
											}
										}
									}
									else if(t.Key == 'N')
									{
										t.Key = 'A';
										Tile.Collisions.Remove(t);
										playerDepolyed--;
									}
								}
							}
						}
						
						ItemManager.Instance.Update(dt);
						
					}	
		}
		
		
	}
}

