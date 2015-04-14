using System;
using System.Collections.Generic;
using Sce.PlayStation.Core;
using Sce.PlayStation.Core.Imaging;
using Sce.PlayStation.Core.Environment;
using Sce.PlayStation.HighLevel.UI;
using Sce.PlayStation.HighLevel.GameEngine2D;

namespace TheATeam
{
    public partial class LobbyUI : Sce.PlayStation.HighLevel.UI.Scene
    {
	
		TwoPlayer twoPlayer;
        public LobbyUI()
        {
			//twoPlayer = new TwoPlayer();
			//Director.Instance.ReplaceScene(twoPlayer);
			
            InitializeWidget();
			
		 	btnBack.TouchEventReceived += HandleBtnBackTouchEventReceived;
			
			lblLobbyChat.Text += " " +AppMain.PLAYERNAME;
			
			
			if(AppMain.ISHOST)
			{
				btnJoinGame.Enabled = false;
				btnJoinGame.Visible = false;
				
				AppMain.client = new LocalTCPConnection(true,11000);
					//server = new LocalTCPConnection(true, 11000);
					if(AppMain.client.Listen())
					{
						lblLobbyChat.Text += ("\n \n \n Connected Waiting for Player  ");
						//twoPlayer.isPlayer1Ready = true;
						//twoPlayer.PostRequest();
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
				lblLobbyChat.Text += ("\n working ");
			}
			
			
        }

        void HandleBtnBackTouchEventReceived (object sender, TouchEventArgs e)
        {
        	PushTransition push = new PushTransition();
			push.MoveDirection = FourWayDirection.Right;
			UISystem.SetScene(new OnlineHostJoin(), push);
        }
		
		public void Update()
		{
			
		}
    }
}
