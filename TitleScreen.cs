using System;
using System.Collections.Generic;

using Sce.PlayStation.Core;
using Sce.PlayStation.Core.Environment;
using Sce.PlayStation.Core.Graphics;
using Sce.PlayStation.Core.Input;
using Sce.PlayStation.Core.Audio;

using Sce.PlayStation.HighLevel.GameEngine2D;
using Sce.PlayStation.HighLevel.GameEngine2D.Base;
using Sce.PlayStation.HighLevel.UI;

namespace TheATeam
{
	public class TitleScreen : Sce.PlayStation.HighLevel.GameEngine2D.Scene
	{
		private SpriteUV 	sprite;
		private TextureInfo	titleTextureInfo;
		//private MainMenu 	mainMenu;
		
		public TitleScreen ()
		{
			sprite = new SpriteUV();
			
			titleTextureInfo  		= new TextureInfo("/Application/assets/TitleScreen.png");
			sprite 			= new SpriteUV(titleTextureInfo);
			sprite.Quad.S 	= titleTextureInfo.TextureSizef;
			sprite.Position = new Vector2(0.0f, 0.0f);

			this.AddChild(sprite);
		}
		
		public override void Update(float deltaTime)
		{
			
			if(Input2.GamePad0.Start.Press)

				{
					
					MainMenu mainMenu = new MainMenu();
					mainMenu.Camera.SetViewFromViewport();
					GameSceneManager.currentScene = mainMenu;
					Director.Instance.ReplaceScene(mainMenu);
				}
		}
		
		
	}
}

