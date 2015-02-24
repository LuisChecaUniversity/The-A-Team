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
		BuildDefence,
		CombatStage
	}

	public class Level: Scene
	{
		private LevelStage levelStage = LevelStage.BuildDefence;
		Player player1;
		Player player2;
		Label lblTopLeft;
		System.Timers.Timer timerA;
		Label lblTimer;
		int countSecs = 300; //5  mins = 300 // convert to have mins:secs 
		private Label lblTopRight;
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
		
		private SpriteUV p1HealthSprite;
		private TextureInfo p1HealthTexInfo;
		private SpriteUV p2HealthSprite;
		private TextureInfo p2HealthTexInfo;
		
		private SpriteUV p1ManaSprite;
		private TextureInfo p1ManaTexInfo;
		private SpriteUV p2ManaSprite;
		private TextureInfo p2ManaTexInfo;
		
		private SpriteUV playerPointer;
		private TextureInfo pointerTex;
		private bool isGameOver = false;
		
		public Level(): base()
		{

			screenWidth = Director.Instance.GL.Context.Screen.Width;
			screenHeight = Director.Instance.GL.Context.Screen.Height;

			font = new Font(FontAlias.System, 25, FontStyle.Bold);
			debugFont = new FontMap(font, 25);

			// Reload the font becuase FontMap disposes of it
			font = new Font(FontAlias.System, 25, FontStyle.Bold);
			Info.LevelClear = false;
			Vector2 player1Pos = Vector2.Zero;
			Vector2 player2Pos = Vector2.Zero;

		 	
			AddChild(new Background());
			
			Tile.Loader("/Application/assets/level2.txt", ref player1Pos, ref player2Pos, this);
			
			for (int i = 0; i < 8; i++) 
				{
					for (int j = 0; j < 5; j++) 
					{						
						if(Tile.Grid[i][j].Key == 'E')
							Tile.Grid[i][j].Key = 'A';
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
			
			player1 = new Player(player1Pos, true,player1Tiles);
			player2 = new Player(player2Pos, false,player2Tiles);
			
			blockedAreaTexInfo = new TextureInfo("/Application/assets/BlockedArea.png");
			
			blockedAreaSprite = new SpriteUV(blockedAreaTexInfo);
			blockedAreaSprite.Quad.S = blockedAreaTexInfo.TextureSizef;
			blockedAreaSprite.Position = new Vector2(screenWidth / 2, 0.0f);
		
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
			
			//extra for video to use later
			p1HealthTexInfo = new TextureInfo("/Application/assets/health.png");
			p1HealthSprite = new SpriteUV(p1HealthTexInfo);
			
			p1HealthSprite.Quad.S = new Vector2(100.0f,30.0f);
			p1HealthSprite.Position = new Vector2(200, screenHeight -30);
			
			p2HealthTexInfo = new TextureInfo("/Application/assets/health.png");
			p2HealthSprite = new SpriteUV(p2HealthTexInfo);
			
			p2HealthSprite.Quad.S = new Vector2(100.0f,30.0f);
			p2HealthSprite.Position = new Vector2(600, screenHeight -30);
			//
			p1ManaTexInfo = new TextureInfo("/Application/assets/mana.png");
			p1ManaSprite = new SpriteUV(p1ManaTexInfo);
			p1ManaSprite.Quad.S = new Vector2(100.0f,30.0f);
			p1ManaSprite.Position = new Vector2(50, screenHeight -30);
			
			p2ManaTexInfo = new TextureInfo("/Application/assets/mana.png");
			p2ManaSprite = new SpriteUV(p2ManaTexInfo);
			p2ManaSprite.Quad.S = new Vector2(100.0f,30.0f);
			p2ManaSprite.Position = new Vector2(750, screenHeight -30);
			
			pointerTex = new TextureInfo("/Application/assets/pointer.png");
			playerPointer = new SpriteUV(pointerTex);
			playerPointer.Quad.S = pointerTex.TextureSizef;
			playerPointer.CenterSprite();
			
			this.AddChild(p1HealthSprite);
			this.AddChild(p2HealthSprite);
			this.AddChild(p1ManaSprite);
			this.AddChild(p2ManaSprite);
			this.AddChild(playerPointer);
			//Timer Stuff
			lblTimer = new Label();
			lblTimer.FontMap = debugFont;
			lblTimer.Text = ""; // might be worth having a ui to separate class
			lblTimer.Position = new Vector2((screenWidth/2) - 80, screenHeight - 30);
					
			this.AddChild(lblTimer);
			
			timerA = new System.Timers.Timer();
			timerA.Elapsed += new ElapsedEventHandler(tickDown);
			timerA.Interval = 1000;
			timerA.Start();
			timerA.Enabled = true;
			//bool timer.enable used for pausing
			
			
			
			Camera2D.SetViewFromViewport();
		}
		
		private void tickDown(object sender, EventArgs e)
		{
			if(levelStage == LevelStage.CombatStage)
			{
				countSecs --;
				lblTimer.Text = "Time Left: " + countSecs;
				if(countSecs == 0)
				{
					lblTimer.Text = "Game Over";
					isGameOver = true;
					timerA.Stop();
					
				}	
			}
		}
		
		public override void OnEnter()
		{
			base.OnEnter();			
			ItemManager.Instance.initFlags(this);
			this.AddChild(blockedAreaSprite);
			this.AddChild(lblTopLeft);
				this.AddChild(lblTopRight);
		}
		public override void Update(float dt)
		{
			base.Update(dt);
			
				if(isGameOver)
				{
					//need to dispose each thing in order to reinit on next game!!!!!!!! TODO
					GameOver go = new GameOver();
					GameSceneManager.currentScene = go;
					Director.Instance.ReplaceScene(go);	
				}	
				if(levelStage == LevelStage.CombatStage)
				{		
					
					p1HealthSprite.Quad.S = new Vector2(player1.health,30.0f);
					p2HealthSprite.Quad.S = new Vector2(player2.health,30.0f);
				
					p1ManaSprite.Quad.S = new Vector2(player1.mana,30.0f);
					p2ManaSprite.Quad.S = new Vector2(player2.mana,30.0f);
					
					playerPointer.Rotation = player1.ShootingDirection;
					playerPointer.Position = player1.Position;
				
					player1.Update(dt);
					player2.UpdateAI(dt, player1);
				
					if(timerA.Enabled == true)
						lblTimer.Text = "Time Left: " + countSecs;
				
				
					// handle bullet update and collision
					ProjectileManager.Instance.Update(dt);
					
					if(ProjectileManager.Instance.ProjectileCollision(player1.Position, player1.Quad.Bounds2()))
						player1.TakeDamage(10);
					if(ProjectileManager.Instance.ProjectileCollision(player2.Position, player2.Quad.Bounds2()))
						player2.TakeDamage(10);
		
		
					for(int i = 0; i < Tile.Collisions.Count; i++)
					{
						Tile t = Tile.Collisions[i];
						char collisionType = ProjectileManager.Instance.ProjectileTileCollision(t.Position, t.Quad.Bounds2());
						if(collisionType != 'X')
						{
							Console.WriteLine(collisionType); // **can hit more then 1 tile at a time**
							t.TakeDamage(collisionType);
						}
						// Remove from collisions if true
						if(t.WallDamage())
						{
							Tile.Collisions.RemoveAt(i);
							i--;
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
						ItemManager.Instance.initElements(this);
					}
					
				}
				
			}
		}
}



