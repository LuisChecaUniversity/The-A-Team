using System;
using Sce.PlayStation.HighLevel.GameEngine2D;
using Sce.PlayStation.Core;
using Sce.PlayStation.HighLevel.GameEngine2D.Base;
using Sce.PlayStation.Core.Imaging;
using Sce.PlayStation.Core.Input;
using Sce.PlayStation.Core.Graphics;
using Sce.PlayStation.HighLevel.UI;
using ECUI;


namespace TheATeam
{
	public class GameOver : Sce.PlayStation.HighLevel.GameEngine2D.Scene
	{
		SpriteUV backgroundSprite;
		TextureInfo backgroundTexinfo;
		public GameOver ()
		{
			
			backgroundTexinfo = new TextureInfo("/Application/assets/Game Over.png");
			backgroundSprite 			= new SpriteUV(backgroundTexinfo);	
			backgroundSprite.Quad.S 	= backgroundTexinfo.TextureSizef;
			
			this.AddChild(backgroundSprite);
			
			//Font font = new Font(FontAlias.System, 25, FontStyle.Bold);
			//FontMap debugFont = new FontMap(font, 25);
			
			//Label lblPressto = new Label();
			//FontMap fontl = new FontMap(new Font("Application/assets/LaSegunda.ttf", 28, FontStyle.Regular), 48);
			//lblPressto.FontMap = fontl;
			//lblPressto.Text = "Press Start to return to main menu";
			//lblPressto.Color = Colors.White;
			//lblPressto.Position = new Vector2(Director.Instance.GL.Context.GetViewport().Width*0.5f - 220.0f,Director.Instance.GL.Context.GetViewport().Height-50.0f);
			//this.AddChild(lblPressto);
			
			Camera2D.SetViewFromViewport();
			ScheduleUpdate();
		}
	public override void Update (float dt)
		{
			if(Input2.GamePad0.Start.Press)
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
				
				Director.Instance.Dispose();
				AppMain.runningDirector = false;
				AppMain.graphics = new GraphicsContext();
				
				UISystem.Initialize(AppMain.graphics);
				UISystem.SetScene(new ECUIMainMenu());
				
				
			}
		}
	}
	
	
	
}

