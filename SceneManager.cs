using System;

namespace PairedGame
{
	public sealed class SceneManager
	{
		private SceneManager()
		{
		}
		
		public static Sce.PlayStation.HighLevel.GameEngine2D.Scene CurrentScene
		{
			get{ return Sce.PlayStation.HighLevel.GameEngine2D.Director.Instance.CurrentScene; } 
		}

		public static void ReplaceScene(Sce.PlayStation.HighLevel.GameEngine2D.Scene scene)
		{
			Sce.PlayStation.HighLevel.GameEngine2D.Director.Instance.ReplaceScene(scene);
		}
		
		public static Sce.PlayStation.HighLevel.UI.Scene CurrentUIScene
		{
			get{ return Sce.PlayStation.HighLevel.UI.UISystem.CurrentScene; } 
		}

		public static void ReplaceUIScene(Sce.PlayStation.HighLevel.UI.Scene scene = null)
		{
			if(scene == null)
				Sce.PlayStation.HighLevel.UI.UISystem.PopScene();
			else if(CurrentUIScene.GetType() == scene.GetType())
				return;
			else
				Sce.PlayStation.HighLevel.UI.UISystem.PushScene(scene);
		}
		
		public static void PauseScene()
		{
			Sce.PlayStation.HighLevel.GameEngine2D.Director.Instance.Pause();
		}
		
		public static void ResumeScene()
		{
			Sce.PlayStation.HighLevel.GameEngine2D.Director.Instance.Resume();
		}
	}
}

