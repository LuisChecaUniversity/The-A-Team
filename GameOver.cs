using System;
using Sce.PlayStation.HighLevel.GameEngine2D;
using Sce.PlayStation.Core;
using Sce.PlayStation.HighLevel.GameEngine2D.Base;
using Sce.PlayStation.Core.Imaging;
using Sce.PlayStation.Core.Input;
using Sce.PlayStation.Core.Graphics;
using Sce.PlayStation.HighLevel.UI;


namespace TheATeam
{
	public class GameOver : Sce.PlayStation.HighLevel.GameEngine2D.Scene
	{
		SpriteUV backgroundSprite;
		TextureInfo backgroundTexinfo;

		public GameOver()
		{
			if (Info.Winner != null)
			{
				backgroundTexinfo = TextureManager.Get((Info.Winner.playerIndex == PlayerIndex.PlayerOne ? "gameover1" : "gameover2"));
			}
			else
			{
				backgroundTexinfo = TextureManager.Get("gameover");
			}
			backgroundSprite = new SpriteUV(backgroundTexinfo);
			backgroundSprite.Quad.S = backgroundTexinfo.TextureSizef;
			
			this.AddChild(backgroundSprite);
			
			Camera2D.SetViewFromViewport();
			ScheduleUpdate();
			
			AudioManager.StopMusic();
			if (Info.Winner != null)
			{
				AudioManager.PlaySound((Info.Winner.playerIndex == PlayerIndex.PlayerOne ? "win" : "lose"));
			}
		}

		public override void Update(float dt)
		{
			if (Input2.GamePad0.Start.Press)
			{
//				MainMenu mainMenu = new MainMenu();
//				mainMenu.Camera.SetViewFromViewport();
//				GameSceneManager.currentScene = mainMenu;
//				Director.Instance.ReplaceScene(mainMenu);	
//				TitleScreen titleScreen = new TitleScreen();
//				titleScreen.Camera.SetViewFromViewport();
//				GameSceneManager.currentScene = titleScreen;
//				
//				Director.Instance.ReplaceScene(titleScreen);
				//UISystem.Terminate();
				//Director.Instance.Dispose();
				AppMain.runningDirector = false;
				//AppMain.graphics = new GraphicsContext();
				//UISystem.Initialize(AppMain.graphics);
				//AppMain.mainMenuUI = new ECUIMainMenu();
				UISystem.SetScene(new ECUIMainMenu());
				
				
			}
		}
	}
	
	
	
}

