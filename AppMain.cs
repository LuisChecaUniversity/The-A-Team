using System;
using System.Collections.Generic;
using System.Net;
using Sce.PlayStation.Core;
using Sce.PlayStation.Core.Environment;
using Sce.PlayStation.Core.Graphics;
using Sce.PlayStation.HighLevel.GameEngine2D;
using Sce.PlayStation.HighLevel.GameEngine2D.Base;
using Sce.PlayStation.HighLevel.UI;
using Sce.PlayStation.Core.Input;
using System.Text;
using System.IO;

namespace TheATeam
{
	enum State
	{
		ChooseTypeGame,
		ChooseHostClient,
		ClientSide
	}
	public class AppMain
	{	
		public static bool 					ISCOMPUTER = true;
		public static string 				TYPEOFGAME;
		public static bool 					ISHOST = false;
		public static string 				IPADDRESS;
		public static string 				CONNECTINGHOSTIPADDRESS;
		public static string 				WHEREWIFI = "PHONE";
		public static string 				PLAYERNAME ;
		public static bool 					QUITGAME = false;
		private static GameSceneManager 	gsm;
		public static LocalTCPConnection 	client;
		private static Timer 				timer;
		public static Button 				button;
		public static Button 				buttonHost;
		public static Button 				buttonClient;
		public static Button 				buttonMulti;		
		public static EditableText 			textbox;
		private static float 				prevTime;
		private static State 				state = State.ChooseTypeGame;
		public static bool 				runningDirector = false;
		public static GraphicsContext 		graphics;
		
		public static ECUIMainMenu			mainMenuUI;
		
		
		public static void Main(string[] args)
		{
			Initialize();
			
			while(!QUITGAME)
			{
				float curTime = (float)timer.Milliseconds();
				
				float dt = curTime - prevTime ;
			
				
				SystemEvents.CheckEvents();	// We check system events (such as pressing PS button, pressing power button to sleep, major and unknown crash!!)				
				Update(dt);
				Director.Instance.Update();
				Director.Instance.Render();
				UISystem.Render();
				Director.Instance.GL.Context.SwapBuffers(); // Swap between back and front buffer
				Director.Instance.PostSwap(); // Must be called after swap buffers - not 100% sure, imagine it resets back buffer to black/white, unallocates tied resources for next swap
	
				prevTime = curTime;
				
			}
			TextureManager.Dispose();
			AudioManager.StopMusic();
			AudioManager.StopSounds();
			Director.Terminate();	// Kill (terminate) the director, hence ending 2D scene program, once we are done with the scene (clicking red X button)
			//UISystem.Terminate();
		}

		public static void Initialize()
		{
//			string mess = "QA245S43QF567S23QE100S343QW25S43QL500S10";
//			int i = 0;
//			int finalLoop = 1;
//			int phase = 1;
//			while ((i = mess.IndexOf('Q',i)) != -1)
//			{
//				char element = mess[i+1];
//				int pos = mess.IndexOf('S',phase);
//				phase = pos;
//				string t = mess.Substring(i+2,pos - (i+2));
//				float xPos = float.Parse(t);
//				
//				string tester = "";
//				int startTest = mess.IndexOf('S',pos );
//				if(finalLoop==5)
//				{
//					int len = mess.Length - startTest;
//					 tester = mess.Substring(startTest+1,len-1);
//				}
//				else
//				{
//					int endTest = -1;
//					if(mess[startTest +3].Equals('Q'))
//						endTest = startTest+2;
//					else if(mess[startTest+4].Equals('Q'))
//						endTest = startTest+3;
//					tester = mess.Substring(startTest+1,endTest-startTest);
//				}
//				float yPos = float.Parse(tester);
//				
//				switch (element)
//				{
//				case 'A':
//					
//					break;
//				case 'E':
//					
//					break;
//				case 'F':
//					
//					break;
//				case 'W':
//					
//					break;
//				case 'L':
//					
//					break;
//				default:
//					break;
//				}
//				
//				i++;
//				phase++;
//				finalLoop++;
//			}
			timer = new Timer();
			InitDirector();
			
		}
		
		public static void Update (float dt)
		{
			if(gsm != null)
				gsm.Update(dt);
		
			List<TouchData> touchDataList = Touch.GetData(0);
			UISystem.Update(touchDataList);	
		}
		
		public static void Render()
		{
			if(runningDirector)
				return;
			// Clear the screen
			graphics.SetClearColor (0.0f, 0.0f, 0.0f, 0.0f);
			graphics.Clear ();

			// Present the screen
			UISystem.Render();
			graphics.SwapBuffers ();
		}
		
		private static void InitDirector()
		{
			Director.Initialize();
			UISystem.Initialize(Director.Instance.GL.Context);
			runningDirector = true;
			gsm = new GameSceneManager();
			
			SplashScreen splashScene = new SplashScreen();
			splashScene.Camera.SetViewFromViewport();
		
			GameSceneManager.currentScene = splashScene;
			
			Director.Instance.RunWithScene(splashScene, true);
			
			

		}
		
		public static void ChangeGame(string typeOfGame)
		{
			switch (typeOfGame)
			{
			case "Solo":
				TYPEOFGAME = "SINGLE";
				runningDirector = true;
				Info.TotalGameTime = 0f;
				Level level = new Level();
				GameSceneManager.currentScene = level;
				Director.Instance.ReplaceScene(level);
				
				
				break;
			case "Dual":
				TYPEOFGAME = "DUAL";
				Info.TotalGameTime = 0f;
				runningDirector = true;
				Level placingTest = new Level();
				placingTest.Camera.SetViewFromViewport();
				GameSceneManager.currentScene = placingTest;
				Director.Instance.ReplaceScene(placingTest);
				break;
			case "MULTIPLAYER":
				runningDirector = true;
				Info.TotalGameTime = 0f;
				Level multiLevel = new Level();
				GameSceneManager.currentScene = multiLevel;
				Director.Instance.ReplaceScene(multiLevel);
				break;
			default:
				break;
			}	
		}
	}
}
