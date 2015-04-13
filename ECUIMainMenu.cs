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
        public ECUIMainMenu()
        {
//			if(!Initialised)
//			{
				InitializeWidget();
				Initialised = true;
				btnSolo.TouchEventReceived += HandleBtnSoloTouchEventReceived;
			//}
        }

        void HandleBtnSoloTouchEventReceived (object sender, TouchEventArgs e)
        {
        	AppMain.ChangeGame("Solo");
			Dispose();
        }
		
		public void Dispose()
		{
			if(btnDual != null)btnDual.Dispose();
			if(btnSolo!= null)btnSolo.Dispose();	
			if(btnOnline!= null)btnOnline.Dispose();
			if(btnQuit!= null)btnQuit.Dispose();
			if(ImageBox_1 != null) ImageBox_1.Dispose();
			this.RootWidget.Dispose();
		}
    }
}
