using System;
using System.Collections.Generic;

using Sce.PlayStation.Core;
using Sce.PlayStation.Core.Environment;
using Sce.PlayStation.Core.Graphics;
using Sce.PlayStation.HighLevel.GameEngine2D;
using Sce.PlayStation.HighLevel.GameEngine2D.Base;
using Sce.PlayStation.HighLevel.UI;
using Sce.PlayStation.Core.Input;

namespace TheATeam
{
	public class AppMain
	{		
		public static bool 					ISHOST = false;
		public static string 				IPADDRESS;
		public static string 				WHEREWIFI = "PHONE";
		public static bool 					QUITGAME = false;
		private static GameSceneManager 	gsm;
		public static LocalTCPConnection 	client;
		private static Timer 				timer;
		public static Button button;
		public static Button buttonHost;
		public static Button buttonClient;
		public static EditableText textbox;
		private static float prevTime;
		
		private static bool 				runningDirector = false;
		private static GraphicsContext graphics;
		
		public static void Main(string[] args)
		{
			Initialize();
			
			//GamePadData gamePadData = GamePad.GetData(0);
			
			while(!QUITGAME)
			{
			float curTime = (float)timer.Milliseconds();
			
			float dt = curTime - prevTime ;
			
			
				SystemEvents.CheckEvents();	// We check system events (such as pressing PS button, pressing power button to sleep, major and unknown crash!!)				
				if(!runningDirector)
				{
					Update(dt);
					Render();
				}
				else
				{
					Update(dt);
					Director.Instance.Update();
				
					Director.Instance.Render();
				
					Director.Instance.GL.Context.SwapBuffers(); // Swap between back and front buffer
					Director.Instance.PostSwap(); // Must be called after swap buffers - not 100% sure, imagine it resets back buffer to black/white, unallocates tied resources for next swap
				}
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
			
			graphics = new GraphicsContext();
			
//			if(ISHOST)
//			{
//				runningDirector = true;
//					InitDirector();
//			}
				UISystem.Initialize(graphics);
			
				Sce.PlayStation.HighLevel.UI.Scene uiScene = new Sce.PlayStation.HighLevel.UI.Scene();
				UISystem.SetScene(uiScene);
				
				buttonHost = new Button();
				buttonHost.SetPosition(100.0f,250.0f);
				buttonHost.Text = "Host";
			
			
			buttonClient = new Button();
				buttonClient.SetPosition(600.0f,250.0f);
				buttonClient.Text = "Client";
			
			 	textbox = new EditableText();
				textbox.SetPosition(300.0f,250.0f);
				textbox.Text = "0.0.0.0";
				textbox.Visible = false;	
			
				button = new Button();
				button.SetPosition(370.0f,320.0f);
				button.Text = "OK";
				button.Visible = false;
				
				uiScene.RootWidget.AddChildFirst(buttonHost);
				uiScene.RootWidget.AddChildFirst(buttonClient);
				uiScene.RootWidget.AddChildFirst(textbox);
				uiScene.RootWidget.AddChildFirst(button);
			
			

		}
		
		public static void Update (float dt)
		{
			if(runningDirector)
			{
				gsm.Update(dt);
			}
			else
			{
			List<TouchData> touchDataList = Touch.GetData(0);
			UISystem.Update(touchDataList);
			
				if(touchDataList.Count > 0)
				{
					float screenheight = 544.0f;
					float screenwidth = 960.0f;
					float screenx = (touchDataList[0].X +0.5f) * screenwidth;
					float screenY = (touchDataList[0].Y +0.5f) * screenheight;
				if(button.HitTest(new Vector2(screenx,screenY)) && touchDataList[0].Status == TouchStatus.Down)
					{
						IPADDRESS = textbox.Text;
						runningDirector = true;	
						InitDirector();
					}	
					if(buttonHost.HitTest(new Vector2(screenx,screenY)) && touchDataList[0].Status == TouchStatus.Down)
					{
						ISHOST = true;
						runningDirector = true;	
						InitDirector();
					}
					if(buttonClient.HitTest(new Vector2(screenx,screenY)) && touchDataList[0].Status == TouchStatus.Down)
					{
						ISHOST = false;
						buttonHost.Visible = false;
						buttonClient.Visible = false;
						textbox.Visible = true;
						button.Visible = true;
					}
				}
			
			}
			//
		}
		
		public static void Render()
		{
			if(runningDirector)
				return;
			graphics.SetClearColor (0.0f, 0.0f, 0.0f, 0.0f);
            graphics.Clear ();
            
            // Render UI Toolkit
            UISystem.Render ();
            
            // Present the screen
            graphics.SwapBuffers ();	
		}
		
		private static void InitDirector()
		{
			graphics.Dispose();
			Director.Initialize();
			
			gsm = new GameSceneManager();
			
			SplashScreen splashScene = new SplashScreen();
			splashScene.Camera.SetViewFromViewport();
			
			GameSceneManager.currentScene = splashScene;
			
			
			
			//Run the scene.
			Director.Instance.RunWithScene(GameSceneManager.currentScene, true);

		}
	}
}
