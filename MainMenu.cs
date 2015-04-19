using Sce.PlayStation.Core;
using Sce.PlayStation.HighLevel.GameEngine2D;
using Sce.PlayStation.HighLevel.GameEngine2D.Base;

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
	
		public MainMenu()
		{
			Initialise();
		}

		public override void Update(float dt)
		{
			if(Input2.GamePad0.Down.Press)
			{
				option++;
				
				if(option > 4)
					option = 1;
				selectIcon.Position = new Vector2(340.0f, Director.Instance.GL.Context.GetViewport().Height - optionsPos[option - 1]);

			}
				
			
			if(Input2.GamePad0.Up.Press)
			{
				option--;
				
				if(option < 1)
					option = 4;	
				selectIcon.Position = new Vector2(340.0f, Director.Instance.GL.Context.GetViewport().Height - optionsPos[option - 1]);
			}
				
			
			if(Input2.GamePad0.Cross.Press)
			{
				switch(option)
				{
				case 1:	
					AppMain.TYPEOFGAME = "SINGLE";
					Level level = new Level();
					level.Camera.SetViewFromViewport();
					GameSceneManager.currentScene = level;
					Director.Instance.ReplaceScene(level);
					break;
				case 2:
				
					TwoPlayer networkingTest = new TwoPlayer();
					networkingTest.Camera.SetViewFromViewport();
					GameSceneManager.currentScene = networkingTest;
					Director.Instance.ReplaceScene(networkingTest);

					break;
					
				case 3:
					AppMain.TYPEOFGAME = "DUAL";
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
			base.Update(dt);
		}
		
		private void Initialise()
		{
			textureInfo = new TextureInfo("/Application/assets/MainMenu.png");
			sprite = new SpriteUV(textureInfo);
			sprite.Quad.S = textureInfo.TextureSizef;
			sprite.Position = new Vector2(0.0f, 0.0f);
			
			selectTexture = new TextureInfo("/Application/assets/selectIcon.png");
			option = 1;
	
			optionsPos[0] = 222.0f; 
			optionsPos[1] = 298.0f;
			optionsPos[2] = 374.0f;
			optionsPos[3] = 450.0f;
			
			selectIcon = new SpriteUV(selectTexture);
			selectIcon.Quad.S = selectTexture.TextureSizef;
			selectIcon.Scale = new Vector2(1.0f, 1.0f);
			selectIcon.Position = new Vector2(340.0f, Director.Instance.GL.Context.GetViewport().Height - optionsPos[0]);
			
			this.AddChild(sprite);
			this.AddChild(selectIcon);
		}
	}
}

