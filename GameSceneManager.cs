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
	public class GameSceneManager :  Sce.PlayStation.HighLevel.GameEngine2D.Scene
	{
		
		public static Sce.PlayStation.HighLevel.GameEngine2D.Scene currentScene;
		
		
		public GameSceneManager (){	}
		
		public  void Update(float deltaTime)
		{
			currentScene.Update(deltaTime);	
		}

		
		public  Sce.PlayStation.HighLevel.GameEngine2D.Scene GetCurrectScene()
		{
			return 	currentScene;
		}
	}
}

