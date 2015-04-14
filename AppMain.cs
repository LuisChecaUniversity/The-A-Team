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
		public static bool 					ISCOMPUTER = false;
		public static string 				TYPEOFGAME;
		public static bool 					ISHOST = false;
		public static string 				IPADDRESS;
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
//				if(!runningDirector)
//				{
//					Update(dt);
//					Render();
//				}
//				else
//				{
					Update(dt);
//					if(runningDirector)
//					{
					Director.Instance.Update();
					
					Director.Instance.Render();
					UISystem.Render();
					
					Director.Instance.GL.Context.SwapBuffers(); // Swap between back and front buffer
					Director.Instance.PostSwap(); // Must be called after swap buffers - not 100% sure, imagine it resets back buffer to black/white, unallocates tied resources for next swap
					//}
				//}
				
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
			
			timer = new Timer();
		
//			graphics = new GraphicsContext();
//			
//			UISystem.Initialize(graphics);
//			Sce.PlayStation.HighLevel.UI.Scene uiScene = new Sce.PlayStation.HighLevel.UI.Scene();
//			
//			UISystem.SetScene(uiScene);
				
//				buttonHost = new Button();
//				buttonHost.SetPosition(100.0f,250.0f);
//				buttonHost.SetSize(200.0f,100.0f);
//				buttonHost.Text = "Single Player";
//			
//			
//				buttonClient = new Button();
//				buttonClient.SetPosition(630.0f,250.0f);
//				buttonClient.SetSize(200.0f,100.0f);
//				buttonClient.Text = "Continue";
//			
//				buttonMulti = new Button();
//				buttonMulti.SetPosition(360.0f,350.0f);
//				buttonMulti.SetSize(200.0f,100.0f);
//				buttonMulti.Text = "Multiplayer";
//			
//			 	textbox = new EditableText();
//				textbox.SetPosition(300.0f,250.0f);
//				textbox.Text = "Enter Name";//"192.168.43.133"; //vita13 //144
//				textbox.Visible = false;	
//			
//				button = new Button();
//				button.SetPosition(370.0f,320.0f);
//				button.Text = "OK";
//				button.Visible = false;
//				
//				uiScene.RootWidget.AddChildFirst(buttonHost);
//				uiScene.RootWidget.AddChildFirst(buttonClient);
//				uiScene.RootWidget.AddChildFirst(textbox);
//				uiScene.RootWidget.AddChildFirst(button);
//				uiScene.RootWidget.AddChildFirst(buttonMulti);
			
				InitDirector();
			
			//TYPEOFGAME = "SINGLE";
							//graphics.Dispose();
							//runningDirector = true;
							//dInitDirector();

		}
		
		public static void Update (float dt)
		{
			if(runningDirector)
			{
				if(gsm != null)
				gsm.Update(dt);
			}
			else
			{
				List<TouchData> touchDataList = Touch.GetData(0);
					UISystem.Update(touchDataList);
				
				//if(touchDataList.Count > 0)
//				{
//					float screenheight = 544.0f;
//					float screenwidth = 960.0f;
//					float screenx = (touchDataList[0].X +0.5f) * screenwidth;
//					float screenY = (touchDataList[0].Y +0.5f) * screenheight;
//					
//					
//					switch (state)
//					{
//					case State.ChooseTypeGame:
//					if(buttonClient.HitTest(new Vector2(screenx,screenY)) && touchDataList[0].Status == TouchStatus.Down)
//					{
//						Console.WriteLine("Duo Player");
//							TYPEOFGAME = "DUAL";
//							graphics.Dispose();
//							runningDirector = true;
//							InitDirector();
//					}	
//					if(buttonHost.HitTest(new Vector2(screenx,screenY)) && touchDataList[0].Status == TouchStatus.Down)
//					{
//						Console.WriteLine("Single Player");
//							TYPEOFGAME = "SINGLE";
//							graphics.Dispose();
//							runningDirector = true;
//							InitDirector();
//					}
//					if(buttonMulti.HitTest(new Vector2(screenx,screenY)) && touchDataList[0].Status == TouchStatus.Down)
//					{
//						Console.WriteLine("MultiPlayer");
//							TYPEOFGAME = "MULTIPLAYER";
//							buttonMulti.Visible = false;
////							buttonHost.Text = "Host";
////							buttonClient.Text = "Client";
//							textbox.Visible = true;
//						buttonHost.Visible = false;
//						buttonClient.SetPosition(360.0f,350.0f);
//							state = State.ClientSide;
//					}
//						break;
//					case State.ChooseHostClient:
//						if(buttonHost.HitTest(new Vector2(screenx,screenY)) && touchDataList[0].Status == TouchStatus.Down)
//					{
//						Console.WriteLine("Host");
//						ISHOST = true;
//						TYPEOFGAME = "MULTIPLAYER";
//						graphics.Dispose();
//						runningDirector = true;
//						InitDirector();
//							
//					}
//						else if(buttonClient.HitTest(new Vector2(screenx,screenY)) && touchDataList[0].Status == TouchStatus.Down)
//					{
//						Console.WriteLine("OK");
//						ISHOST = false;
//						textbox.Visible = true;
//						buttonHost.Visible = false;
//						buttonClient.SetPosition(360.0f,350.0f);
//						state = State.ClientSide;
//					}
//						break;
//					case State.ClientSide:
//						if(buttonClient.HitTest(new Vector2(screenx,screenY)) && touchDataList[0].Status == TouchStatus.Down)
//						{
//							
//							if(textbox.Text.Length > 0)
//							{
//								//graphics.Dispose();
//								AppMain.PLAYERNAME = textbox.Text;
//								runningDirector = true;
//								InitDirector();	
//							}
//							
//						}
//						break;
//					default:
//						break;
//					}
//				
//				}
//			
			}
			
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
				//Console.WriteLine("Single Player");
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
			default:
				break;
			}	
		}
	}
}
