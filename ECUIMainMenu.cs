using System;
using System.Collections.Generic;
using Sce.PlayStation.Core;
using Sce.PlayStation.Core.Imaging;
using Sce.PlayStation.Core.Environment;
using Sce.PlayStation.HighLevel.UI;

namespace TheATeam
{
    public partial class ECUIMainMenu : Scene
    {
		static bool Initialised = false;
		//Button btnSolo;
        //Button btnOnline;
       // Button btnDual;
       // Button btnQuit;
		Button okButton;
        public ECUIMainMenu()
        {

				InitializeWidget();
			AudioManager.StopMusic();
			//AudioManager.PlayMusic("bgm", true, 0.4f);
					
			btnSolo.TouchEventReceived += HandleBtnSoloTouchEventReceived;
			btnDual.TouchEventReceived += HandleBtnDualTouchEventReceived;
			btnOnline.TouchEventReceived += HandleBtnOnlineTouchEventReceived;
			btnQuit.TouchEventReceived += HandleBtnQuitTouchEventReceived;
			//}
        }

        void HandleBtnQuitTouchEventReceived (object sender, TouchEventArgs e)
        {
        	AppMain.QUITGAME = true;
        }

        void HandleBtnOnlineTouchEventReceived (object sender, TouchEventArgs e)
        {
			AppMain.TYPEOFGAME="MULTIPLAYER";
			PushTransition push = new PushTransition();
			push.MoveDirection = FourWayDirection.Left;
			UISystem.SetScene(new OnlineName(), push);
        }

        void HandleBtnDualTouchEventReceived (object sender, TouchEventArgs e)
        {
        	AppMain.ChangeGame("Dual");
			Dispose();
        }

        void HandleBtnSoloTouchEventReceived (object sender, TouchEventArgs e)
        {
        	AppMain.ChangeGame("Solo");
			Dispose();
        }
		
		public void Dispose()
		{
			foreach (var item in this.RootWidget.Children)
			{
				this.RootWidget.RemoveChild(item);
				Console.WriteLine("Removed " + item);
			}
			this.RootWidget.Dispose();
			if(okButton != null)okButton.Dispose();
			if(btnDual != null)btnDual.Dispose();
			if(btnSolo!= null)btnSolo.Dispose();	
			if(btnOnline!= null)btnOnline.Dispose();
			if(btnQuit!= null)btnQuit.Dispose();
			if(ImageBox_1 != null) ImageBox_1.Dispose();
			
		}
    }
}
