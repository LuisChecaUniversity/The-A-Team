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
		public bool p1Ready;
		public bool p2Ready;
		TwoPlayer twoPlayer;
		
		public Panel PnlActivePlayers {
			get {
				return this.pnlActivePlayers;
			}
			set {
				pnlActivePlayers = value;
			}
		}
			
		public Sce.PlayStation.HighLevel.UI.Label LblLobbyChat {
			get {
				return this.lblLobbyChat;
			}
			set {
				lblLobbyChat = value;
			}
		}
		
		public Button BtnJoinGame {
			get {
				return this.btnJoinGame;
			}
			set {
				btnJoinGame = value;
			}
		}

		public Button BtnMainMenu {
			get {
				return this.btnMainMenu;
			}
			set {
				btnMainMenu = value;
			}
		}
		
        public LobbyUI(TwoPlayer twoPlayerScene)
        {
			//twoPlayer = new TwoPlayer();
			//Director.Instance.ReplaceScene(twoPlayer);
			twoPlayer = twoPlayerScene;
				
            InitializeWidget();
			
			
		
			
		 	btnMainMenu.TouchEventReceived += HandleBtnBackTouchEventReceived;
			btnJoinGame.TouchEventReceived += HandleBtnJoinGameTouchEventReceived;
			lblLobbyChat.Text += " " +AppMain.PLAYERNAME;
			
			
			if(AppMain.ISHOST)
			{
				btnJoinGame.Enabled = false;
				btnJoinGame.Visible = false;
				
				AppMain.client = new LocalTCPConnection(true,11000);
					//server = new LocalTCPConnection(true, 11000);
					if(AppMain.client.Listen())
					{
						p1Ready =true;
						lblLobbyChat.Text += ("\n \n \n Connected Waiting for Player  ");
						twoPlayer.PostRequest();
					}
					else
					{
						lblLobbyChat.Text += ("\n ERROR ");
					}
			}
			else
			{
				btnJoinGame.Enabled = false;
				btnJoinGame.Visible = false;
				
				AppMain.client = new LocalTCPConnection(false,11000);
				//AppMain.client.SetIPAddress(AppMain.IPADDRESS);
				lblLobbyChat.Text += ("\n Choose Player to the right \n" +
				 	" Then click Join Game below to Start");
				
			}
			
			
        }

        void HandleBtnJoinGameTouchEventReceived (object sender, TouchEventArgs e)
        {
			if(e.TouchEvents[0].Type == TouchEventType.Down)
			{
        		AppMain.client.Connect();
				twoPlayer.PostRequest();
				p2Ready = true;
				Console.WriteLine("Player 2 is " + p2Ready);
			}
        }

        void HandleBtnBackTouchEventReceived (object sender, TouchEventArgs e)
        {
        	PushTransition push = new PushTransition();
			push.MoveDirection = FourWayDirection.Right;
			UISystem.SetScene(new OnlineHostJoin(), push);
        }
		
		
    }
}
