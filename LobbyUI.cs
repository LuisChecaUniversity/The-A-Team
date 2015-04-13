using System;
using System.Collections.Generic;
using Sce.PlayStation.Core;
using Sce.PlayStation.Core.Imaging;
using Sce.PlayStation.Core.Environment;
using Sce.PlayStation.HighLevel.UI;

namespace TheATeam
{
    public partial class LobbyUI : Scene
    {
		bool isPlayer1Ready = false;
        public LobbyUI()
        {
            InitializeWidget();
			
		 	btnBack.TouchEventReceived += HandleBtnBackTouchEventReceived;
			
			if(AppMain.ISHOST)
			{
				btnJoinGame.Enabled = false;
				btnJoinGame.Visible = false;
				
				AppMain.client = new LocalTCPConnection(true,11000);
					//server = new LocalTCPConnection(true, 11000);
					if(AppMain.client.Listen())
					{
						lblLobbyChat.Text += ("\n working ");
						isPlayer1Ready = true;
					}
					else
					{
						lblLobbyChat.Text += ("\n ERROR ");
					}
			}
			else
			{
				AppMain.client = new LocalTCPConnection(false,11000);
				AppMain.client.SetIPAddress(AppMain.IPADDRESS);
				lblLobbyChat.Text += ("\n WORKING ");
			}
			
			lblLobbyChat.Text += " " +AppMain.PLAYERNAME;
        }

        void HandleBtnBackTouchEventReceived (object sender, TouchEventArgs e)
        {
        	PushTransition push = new PushTransition();
			push.MoveDirection = FourWayDirection.Right;
			UISystem.SetScene(new OnlineHostJoin(), push);
        }
    }
}
