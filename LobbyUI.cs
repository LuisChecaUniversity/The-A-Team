using System;
using System.Collections.Generic;
using Sce.PlayStation.Core;
using Sce.PlayStation.Core.Imaging;
using Sce.PlayStation.Core.Environment;
using Sce.PlayStation.HighLevel.UI;
using Sce.PlayStation.HighLevel.GameEngine2D;


//TODO close sockets when you press main menu or back
// try get a delete all fields before going to the company

namespace TheATeam
{
    public partial class LobbyUI : Sce.PlayStation.HighLevel.UI.Scene
    {
		public bool p1Ready = false;
		public bool p2Ready = false;
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
					
					if(AppMain.client.Listen())
					{
						p1Ready = true;
						lblLobbyChat.Text += ("\n \n \n Connected Waiting for Player  ");
						//twoPlayer.PostRequest();
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
				
				lblLobbyChat.Text += ("\n Choose Player to the right \n" +
				 	" Then click Join Game below to Start");
				
			}
			
			
        }

        void HandleBtnJoinGameTouchEventReceived (object sender, TouchEventArgs e)
        {
			if(e.TouchEvents[0].Type == TouchEventType.Down)
			{
				btnJoinGame.Enabled = false;
        		AppMain.client.Connect();
				twoPlayer.PostRequest();
				p2Ready = true;
				Console.WriteLine("Player 2 is " + p2Ready);
				
			}
        }

        void HandleBtnBackTouchEventReceived (object sender, TouchEventArgs e)
        {
			if(AppMain.client!= null) AppMain.client.Disconnect();
        	PushTransition push = new PushTransition();
			push.MoveDirection = FourWayDirection.Right;
			UISystem.SetScene(new OnlineHostJoin(), push);
        }
		
			public void Dispose()
		{
				foreach (var item in this.RootWidget.Children)
			{
				this.RootWidget.RemoveChild(item);
				Console.WriteLine("Removed " + item);
			}
			this.RootWidget.Dispose();
			
			if(btnMainMenu != null)btnMainMenu.Dispose();
			if(btnJoinGame != null)btnJoinGame.Dispose();
			if(pnlLobbyChat!= null)pnlLobbyChat.Dispose();	
			if(pnlActivePlayers!= null)pnlActivePlayers.Dispose();
			if(lblLobbyChat!= null)lblLobbyChat.Dispose();
			if(ImageBox_1 != null) ImageBox_1.Dispose();
		}
    }
}
