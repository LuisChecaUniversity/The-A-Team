using System;
using System.Collections.Generic;

using Sce.PlayStation.Core;
using Sce.PlayStation.Core.Audio;
using Sce.PlayStation.Core.Environment;
using Sce.PlayStation.Core.Graphics;
using Sce.PlayStation.Core.Input;

using Sce.PlayStation.HighLevel.GameEngine2D;
using Sce.PlayStation.HighLevel.GameEngine2D.Base;
using Sce.PlayStation.HighLevel.UI;

namespace TheATeam
{
	public class MainMenu : Sce.PlayStation.HighLevel.GameEngine2D.Scene
	{
		
		private SpriteUV 		sprite;
		private SpriteUV 		selectIcon;
		private TextureInfo 	selectTexture;
		private TextureInfo		textureInfo;
		private int 			option;
		private float[] 		optionsPos = new float[4];
		//private Sce.PlayStation.HighLevel.GameEngine2D.Scene[] scenes = new Sce.PlayStation.HighLevel.GameEngine2D.Scene[3];
		//private Sce.PlayStation.HighLevel.GameEngine2D.Scene playerSelectScene;
		//private OptionsScene optionsScene;
		//private Sce.PlayStation.HighLevel.GameEngine2D.Scene howToPlayScene;
		
		
		public MainMenu()
		{
			textureInfo = new TextureInfo("/Application/assets/MainMenu.png");
			sprite = new SpriteUV(textureInfo);
			sprite.Quad.S = textureInfo.TextureSizef;
			sprite.Position = new Vector2(0.0f, 0.0f);
			
			selectTexture = new TextureInfo("/Application/assets/selectIcon.png");
			option = 1;
	
			optionsPos[0] = 240.5f; 
			optionsPos[1] = 312.0f;
			optionsPos[2] = 384.0f;
			optionsPos[3] = 452.0f;
			
			selectIcon = new SpriteUV(selectTexture);
			selectIcon.Quad.S = selectTexture.TextureSizef;
			selectIcon.Scale = new Vector2(1.0f, 1.0f);
			selectIcon.Position = new Vector2(280.0f, Director.Instance.GL.Context.GetViewport().Height - optionsPos[0]);
			
			this.AddChild(sprite);
			this.AddChild(selectIcon);
		}

		public override void Update(float dt)
		{
			if(Input2.GamePad0.Down.Press)
			{
				option++;
				
				if(option > 4)
					option = 1;
				selectIcon.Position = new Vector2(280.0f, Director.Instance.GL.Context.GetViewport().Height - optionsPos[option - 1]);

			}
				
			
			if(Input2.GamePad0.Up.Press)
			{
				option--;
				
				if(option < 1)
					option = 4;	
				selectIcon.Position = new Vector2(280.0f, Director.Instance.GL.Context.GetViewport().Height - optionsPos[option - 1]);
			}
				
			
			if(Input2.GamePad0.Square.Press)
			{
				switch(option)
				{
				case 1:
					
// Load and store textures
//
					TextureManager.AddAsset("tiles", new TextureInfo(new Texture2D("/Application/assets/tiles.png", false),
			                                                 new Vector2i(10, 2)));
//
					TextureManager.AddAsset("entities", new TextureInfo(new Texture2D("/Application/assets/dungeon_objects.png", false),
			                                                 new Vector2i(9, 14)));
//			
//			// Initial Values;
					Info.TotalGameTime = 0f;
					Info.LevelNumber = 1;
//			
//			// Tell the UISystem to run an empty scene
//			//UISystem.SetScene(new GameUI(), null);
//			// Tell the Director to run our scene
//
					Level level = new Level();
					level.Camera.SetViewFromViewport();
					GameSceneManager.currentScene = level;
			Director.Instance.ReplaceScene(level);
			//Director.Instance.ReplaceScene(new Level());
					break;
					
				case 2:
				
					TwoPlayer networkingTest = new TwoPlayer();
					networkingTest.Camera.SetViewFromViewport();
					GameSceneManager.currentScene = networkingTest;
					Director.Instance.ReplaceScene(networkingTest);
//					optionsScene = new OptionsScene();
//					optionsScene.Camera.SetViewFromViewport();
//					GameSceneManager.currentScene = optionsScene;
//					Director.Instance.ReplaceScene(GameSceneManager.currentScene);
					break;
					
				case 3:
//					bgmPlayer.Stop();
//					bgmPlayer.Dispose();
//					HowToPlay howToPlay = new HowToPlay();
//					howToPlay.Camera.SetViewFromViewport();
//					GameSceneManager.currentScene = howToPlay;
//					Director.Instance.ReplaceScene(GameSceneManager.currentScene);
					TextureManager.AddAsset("tiles", new TextureInfo(new Texture2D("/Application/assets/tiles.png", false),
			                                                 new Vector2i(10, 2)));
//
					TextureManager.AddAsset("entities", new TextureInfo(new Texture2D("/Application/assets/dungeon_objects.png", false),
			                                                 new Vector2i(9, 14)));
//			
//			// Initial Values;
					Info.TotalGameTime = 0f;
					Info.LevelNumber = 1;
//			
//			// Tell the UISystem to run an empty scene
//			//UISystem.SetScene(new GameUI(), null);
//			// Tell the Director to run our scene
//
					Level placingTest = new Level();
					placingTest.Camera.SetViewFromViewport();
					GameSceneManager.currentScene = placingTest;
					Director.Instance.ReplaceScene(placingTest);
					
					break;
					
				case 4:
					AppMain.QUITGAME = true;
					break;
				default:
					break;
				}
				
					
					
					
					
			}
			
//			var menuTouches = Touch.GetData(0);
//			
//			if(menuTouches.Count > 0 && menuTouches[0].Status == TouchStatus.Down)
//				{
//					float screenheight = Director.Instance.GL.Context.GetViewport().Height;
//					float screenwidth = Director.Instance.GL.Context.GetViewport().Width;
//					float screenx = (menuTouches[0].X +0.5f) * screenwidth;
//					float screenY = (menuTouches[0].Y +0.5f) * screenheight;
//					//Console.WriteLine(screenx + " " + screenY );
//					if(screenx >= 357 && screenx <= 583
//					   && screenY >= 204 && screenY <= 238)
//					{
//						Console.WriteLine("Start");
//					}	
//					else if(screenx >= 357 && screenx <= 583
//					   && screenY >= 250 && screenY <= 285)
//					{
//						Console.WriteLine("Options");
//					}	
//					else if(screenx >= 357 && screenx <= 583
//					   && screenY >= 300 && screenY <= 334)
//					{
//						Console.WriteLine("HTP");
//					}	
//					else if(screenx >= 357 && screenx <= 583
//					   && screenY >= 349 && screenY <= 386)
//					{
//						//quitGame = true;
//					}	
//				else
//					Console.WriteLine(screenx + " " + screenY );
//				}
			base.Update(dt);
		}
	}
}

