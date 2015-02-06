using System;
using System.Collections.Generic;

using Sce.PlayStation.Core;
using Sce.PlayStation.Core.Environment;
using Sce.PlayStation.Core.Graphics;
using Sce.PlayStation.Core.Input;

using Sce.PlayStation.HighLevel.GameEngine2D;
using Sce.PlayStation.HighLevel.GameEngine2D.Base;
using Sce.PlayStation.HighLevel.UI;

namespace TheATeam
{
	public class SplashScreen: Sce.PlayStation.HighLevel.GameEngine2D.Scene
	{
		private SpriteUV 	sprite;
		private TextureInfo	textureInfo;
		private SpriteUV 	whiteBGsprite;
		private TextureInfo	whiteBGTextureInfo;
		private bool fadeUp, fadeDown,finishedFade;
		
		public SplashScreen ()
		{
			sprite = new SpriteUV();
				
			textureInfo  		= new TextureInfo("/Application/assets/TheATeam.png");
			sprite 			= new SpriteUV(textureInfo);
			sprite.Quad.S 	= textureInfo.TextureSizef;
			sprite.Position = new Vector2(0.0f, 0.0f);
			sprite.Color = new Vector4(1f,1f,1f,0f);
			
			whiteBGsprite = new SpriteUV();
				
			whiteBGTextureInfo  		= new TextureInfo("/Application/assets/WhiteBG.png");
			whiteBGsprite 			= new SpriteUV(whiteBGTextureInfo);
			whiteBGsprite.Quad.S 	= whiteBGTextureInfo.TextureSizef;
			whiteBGsprite.Position = new Vector2(0.0f, 0.0f);
			whiteBGsprite.Color = new Vector4(1f,1f,1f,1f);
			
			fadeUp = true;
			fadeDown = false;
			finishedFade = false;
			
			this.AddChild(whiteBGsprite);
			this.AddChild(sprite);
			
			
			
		}
		public override void Cleanup ()
		{
			textureInfo.Dispose();
			base.Cleanup ();
		}
		
		public override void Update(float deltaTime)
		{
		FadeSprite(deltaTime);	
		}
		
		public bool FinishedFade()
		{
			return finishedFade;	
		}	
	
		private void FadeSprite(float deltaTime)
		{
			if(fadeUp && !finishedFade)
			{
				
				if(sprite.Color.A < 1.0f)
				{
					sprite.Color += new Vector4(0f,0f,0f,deltaTime * 0.0007f);
						
				}
				else
				{
					fadeUp = false;
					fadeDown = true;
				}
			}
			else if(fadeDown && !finishedFade)
			{
				if(sprite.Color.A > 0.0f)
				{
					sprite.Color += new Vector4(0f,0f,0f,-deltaTime * 0.0007f);
				}
				else
				{
					fadeUp = false;
					fadeDown = false;
					finishedFade = true;
				}
			}
			
			if(FinishedFade())
			{
				TitleScreen titleScreen = new TitleScreen();
				titleScreen.Camera.SetViewFromViewport();
				GameSceneManager.currentScene = titleScreen;
				
				Director.Instance.ReplaceScene(titleScreen);
			}
		}
	}
}

