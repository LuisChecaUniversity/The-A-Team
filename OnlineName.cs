using System;
using System.Collections.Generic;
using Sce.PlayStation.Core;
using Sce.PlayStation.Core.Imaging;
using Sce.PlayStation.Core.Environment;
using Sce.PlayStation.HighLevel.UI;

namespace TheATeam
{
    public partial class OnlineName : Scene
    {
        public OnlineName()
        {
            InitializeWidget();
			
				EditableText_1.Text = "";
			EditableText_1.DefaultText = "Enter Name Here";
		btnBack.TouchEventReceived += Handle_btnBackTouchEventReceived;
			btnEnter.TouchEventReceived += HandleBtnEnterTouchEventReceived;
        }

        void HandleBtnEnterTouchEventReceived (object sender, TouchEventArgs e)
        {
        	if(EditableText_1.Text.Length > 0)
			{
				AppMain.PLAYERNAME = EditableText_1.Text;
				UISystem.SetScene(new OnlineHostJoin(), new PushTransition());	
			}
        }

       

        void Handle_btnBackTouchEventReceived (object sender, TouchEventArgs e)
        {
			PushTransition push = new PushTransition();
			push.MoveDirection = FourWayDirection.Right;
        	UISystem.SetScene(new ECUIMainMenu(), push);	
        }
    }
}
