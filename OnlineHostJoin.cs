using System;
using System.Collections.Generic;
using Sce.PlayStation.Core;
using Sce.PlayStation.Core.Imaging;
using Sce.PlayStation.Core.Environment;
using Sce.PlayStation.HighLevel.UI;

namespace TheATeam
{
    public partial class OnlineHostJoin : Scene
    {
        public OnlineHostJoin()
        {
            InitializeWidget();
			
			btnMainMenu.TouchEventReceived += HandleBtnMainMenuTouchEventReceived;
			btnHostGame.TouchEventReceived += HandleBtnHostGameTouchEventReceived;
			btnJoinGame.TouchEventReceived += HandleBtnJoinGameTouchEventReceived;
        }

        void HandleBtnJoinGameTouchEventReceived (object sender, TouchEventArgs e)
        {
        	AppMain.ISHOST = false;
			PushTransition push = new PushTransition();
			push.MoveDirection = FourWayDirection.Up;
			UISystem.SetScene(new LobbyUI(), push);
        }

        void HandleBtnHostGameTouchEventReceived (object sender, TouchEventArgs e)
        {
			AppMain.ISHOST = true;
			PushTransition push = new PushTransition();
			push.MoveDirection = FourWayDirection.Up;
			UISystem.SetScene(new LobbyUI(), push);
			
        }

        void HandleBtnMainMenuTouchEventReceived (object sender, TouchEventArgs e)
        {
        	PushTransition push = new PushTransition();
			push.MoveDirection = FourWayDirection.Right;
			UISystem.SetScene(new ECUIMainMenu(), push);
        }
    }
}
