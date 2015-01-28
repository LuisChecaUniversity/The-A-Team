using Sce.PlayStation.Core;
using Sce.PlayStation.Core.Environment;
using Sce.PlayStation.Core.Graphics;
using Sce.PlayStation.HighLevel.GameEngine2D;
using Sce.PlayStation.HighLevel.GameEngine2D.Base;

namespace TheATeam
{
	public class AppMain
	{		
		public static bool 					QUITGAME = false;
		private static GameSceneManager 	gsm;
		private static Timer 				timer;
		
		private static float prevTime;
		
		public static void Main(string[] args)
		{
			Initialize();
			
			//GamePadData gamePadData = GamePad.GetData(0);
			
			while(!QUITGAME)
			{
			
				SystemEvents.CheckEvents();	// We check system events (such as pressing PS button, pressing power button to sleep, major and unknown crash!!)				
				Update();
				Director.Instance.Update();
				//UISystem.Update(Touch.GetData(0), ref gamePadData); // Update UI Manager
				Director.Instance.Render();
				//UISystem.Render(); // Render UI Manager
				Director.Instance.GL.Context.SwapBuffers(); // Swap between back and front buffer
				Director.Instance.PostSwap(); // Must be called after swap buffers - not 100% sure, imagine it resets back buffer to black/white, unallocates tied resources for next swap
			}
			TextureManager.Dispose();
			AudioManager.StopMusic();
			AudioManager.StopSounds();
			Director.Terminate();	// Kill (terminate) the director, hence ending 2D scene program, once we are done with the scene (clicking red X button)
			//UISystem.Terminate();
		}

		public static void Initialize()
		{
//			// Initialises the GameEngine2D supplied by Sony.
//			Director.Initialize();
//			// Initialises the UI Framework supplied by Sony.
//			//UISystem.Initialize(Director.Instance.GL.Context);
//			// Load and store textures
//
//			TextureManager.AddAsset("tiles", new TextureInfo(new Texture2D("/Application/assets/tiles.png", false),
//			                                                 new Vector2i(7, 1)));
//
//			TextureManager.AddAsset("entities", new TextureInfo(new Texture2D("/Application/assets/dungeon_objects.png", false),
//			                                                 new Vector2i(9, 14)));
//			
//			// Initial Values;
//			Info.TotalGameTime = 0f;
//			Info.LevelNumber = 1;
//			
//			// Tell the UISystem to run an empty scene
//			//UISystem.SetScene(new GameUI(), null);
//			// Tell the Director to run our scene
//
//			Director.Instance.RunWithScene(new Level(), true);
			
			timer = new Timer();
			
			Director.Initialize();
			
			gsm = new GameSceneManager();
			
			SplashScreen splashScene = new SplashScreen();
			splashScene.Camera.SetViewFromViewport();
			
			GameSceneManager.currentScene = splashScene;
			
			//Run the scene.
			//Director.Instance.RunWithScene(GameSceneManager.currentScene, true);
			
			// pete lazyness - skip to game
			TextureManager.AddAsset("tiles", new TextureInfo(new Texture2D("/Application/assets/tiles.png", false),
			                                                 new Vector2i(7, 1)));
			TextureManager.AddAsset("entities", new TextureInfo(new Texture2D("/Application/assets/dungeon_objects.png", false),
			                                                 new Vector2i(9, 14)));
			Info.TotalGameTime = 0f;
			Info.LevelNumber = 1;
			Level l = new Level();
			GameSceneManager.currentScene = l;
			Director.Instance.RunWithScene(l, true);
			// end of pete lazyness
		}
		
		public static void Update ()
		{
			
			float curTime = (float)timer.Milliseconds();
			
			float dt = curTime - prevTime ;
			
			prevTime = curTime;
			gsm.Update(dt);
		}
	}
}
