using System;
using System.Net;


using System.Collections.Generic;
using Sce.PlayStation.HighLevel.GameEngine2D;
using Sce.PlayStation.Core;
using Sce.PlayStation.HighLevel.GameEngine2D.Base;
using Sce.PlayStation.Core.Imaging;
using Sce.PlayStation.Core.Input;


using Sce.PlayStation.Core.Audio;
using Sce.PlayStation.Core.Environment;
using Sce.PlayStation.Core.Graphics;
using Sce.PlayStation.HighLevel.UI;
using System.Json;
using System.Text;
using System.IO;
using System.Web;

//using System.Net;
//using System.Net.Sockets;
//using System.IO;
//using System.Threading;


namespace TheATeam
{
//	/**
//	 * SocketListenerInterface
//	 */
//	interface ISocketListener
//	{
//		/**
//		 * Accept
//		 */
//		void OnAccept(IAsyncResult AsyncResult);
//		/**
//		 * Connect
//		 */
//		void OnConnect(IAsyncResult AsyncResult);
//		/**
//		 * Receive
//		 */
//		void OnReceive(IAsyncResult AsyncResult);
//		/**
//		 * Send
//		 */
//		void OnSend(IAsyncResult AsyncResult);
//	}
//	/**
//	 * SocketEventCallback
//	 */
//	class SocketEventCallback
//	{
//		/**
//		 * AcceptCallback
//		 */
//		public static void AcceptCallback(IAsyncResult AsyncResult) 
//		{
//			LocalTCPConnection Server = (LocalTCPConnection)AsyncResult.AsyncState;
//			Server.OnAccept(AsyncResult);
//		}
//
//		/**
//		 * ConnectCallback
//		 */
//		public static void ConnectCallback(IAsyncResult AsyncResult)
//		{
//			LocalTCPConnection Client = (LocalTCPConnection)AsyncResult.AsyncState;
//			Client.OnConnect(AsyncResult);
//		}
//		/**
//		 * ReceiveCallback
//		 */
//		public static void ReceiveCallback(IAsyncResult AsyncResult)
//		{
//			LocalTCPConnection TCPs = (LocalTCPConnection)AsyncResult.AsyncState;
//			TCPs.OnReceive(AsyncResult);
//		}
//
//		/**
//		 * SendCallback
//		 */
//		public static void SendCallback(IAsyncResult AsyncResult)
//		{
//			LocalTCPConnection TCPs = (LocalTCPConnection)AsyncResult.AsyncState;
//			TCPs.OnSend(AsyncResult);
//		}
//	}
//	
//	/**
//	 * Class for SocketTCP local connection
//	 */
//	public class LocalTCPConnection : ISocketListener
//	{
//		/**
//		 * Status
//		 */
//		public enum Status
//		{
//			kNone,		
//			kListen,	// Listen or connecting
//			kConnected,	
//			kUnknown
//		}
///*
//		using (CriticalSection CS = new CriticalSection(syncObject))
//		{
//		
//		}
//		public class CriticalSection : IDisposable
//		{
//			private object syncObject = null;
//			public CriticalSection(object SyncObject)
//			{
//				syncObject = SyncObject;
//				Monitor.Enter(syncObject);
//			}
//
//			public virtual void Dispose()
//			{
//				Monitor.Exit(syncObject);
//				syncObject = null;
//			}
//		}
//*/
//        /**
//         * Object for exclusive  socket access
//         */
//        private object syncObject = new object();
//		/**
//		 * Enter critical section
//		 */
//		private void enterCriticalSection() 
//		{
//			Monitor.Enter(syncObject);
//		}
//		/**
//		 * Leave critical section
//		 */
//		private void leaveCriticalSection() 
//		{
//			Monitor.Exit(syncObject);
//		}
//
//		/**
//		 * Get status
//		 * 
//		 * @return Status
//		 */
//		public Status StatusType
//		{
//			get
//			{
//				try
//				{
//					enterCriticalSection();
//					if (Socket == null){
//						return Status.kNone;
//					}
//					else{
//						if (IsServer){
//							if(ClientSocket == null){
//								return Status.kListen;
//							}
//							return Status.kConnected;
//						}
//						else{
//							if(IsConnect == false){
//								return Status.kListen;
//							}
//							return Status.kConnected;
//						}
//					}
//				}
//				finally
//				{
//					leaveCriticalSection();
//				}
//			}
//		}
//
//        /**
//         * Get status as string
//         * 
//         * @return status string
//         */
//        public string statusString
//		{
//			get
//			{
//				switch (StatusType)
//				{
//					case Status.kNone:
//						return "None";
//
//					case Status.kListen:
//						if (IsServer){
//							return "Listen";
//						}
//						else{
//							return "Connecting";
//						}
//
//					case Status.kConnected:
//						return "Connected";
//				}
//				return "Unknown";
//			}
//		}
//
//		/**
//		 * Get the button string based on status
//		 * 
//		 * @return button string
//		 */
//		public string buttonString
//		{
//			get
//			{
//				switch (StatusType)
//				{
//					case Status.kNone:
//						if (IsServer){
//							return "Listen";
//						}
//						else{
//							return "Connect";
//						}
//					case Status.kListen:
//						return "Disconnect";
//					case Status.kConnected:
//						return "Disconnect";
//				}
//				return "Unknown";
//			}
//		}
//
//        /**
//		 * Process the button that lets us change the status based on 
//		 * current status 
//         */
//        public void ChangeStatus()
//		{
//			switch(StatusType)
//			{
//				case	Status.kNone:
//					if (IsServer){
//						Listen();
//					}
//					else{
//						Connect();
//					}
//					break;
//
//				case	Status.kListen:
//					Disconnect();
//					break;
//				
//				case	Status.kConnected:
//					Disconnect();
//					break;
//			}
//		}
//
//        /**
//         * transceiver buffer
//         */
//        private byte[] sendBuffer = new byte[8];
//		private byte[] recvBuffer = new byte[8];
//		
//		public string testStatus = "Nothing";
//
//		/**
//		 * Our position or the other party's
//		 */
//		private Sce.PlayStation.Core.Vector2 myPosition		= new Sce.PlayStation.Core.Vector2(999, 999);
//		public	Sce.PlayStation.Core.Vector2 MyPosition
//		{
//			get { return myPosition; }
//		}
//		public	void	SetMyPosition(float X, float Y)
//		{
//			myPosition.X = X;
//			myPosition.Y = Y;
//		}
//
//		
//		public Sce.PlayStation.Core.Vector2 networkPosition	= new Sce.PlayStation.Core.Vector2(999, 999);
//		public Sce.PlayStation.Core.Vector2 NetworkPosition
//		{
//			get { return networkPosition; }
//		}
//		
//		/**
//		 * Are we connected
//		 */
//		private bool isConnect = false;
//		public bool IsConnect
//		{
//					get	{	return isConnect; }
//			private set	{	this.isConnect = value;	}
//		}
//
//        /**
//         * Socket  Listen when server  Server connect when client
//         */
//        private Socket socket;
//		public  Socket Socket 
//		{
//			get	{	return socket;	}
//		}
//
//		/**
//		 * Client socket when server
//		 */
//		private Socket clientSocket;
//		public Socket ClientSocket
//		{
//					get	{	return clientSocket;	}
//			private set	{	this.clientSocket = value;	}
//		}
//
//		/**
//		 * Is this a server
//		 */
//		private bool isServer;
//		public bool IsServer
//		{
//			get	{	return isServer;	}
//		}
//
//		/**
//		 * Port number
//		 */
//		private UInt16 port;
//		public UInt16 Port
//		{
//			get	{	return port;	}
//		}
//		
//		private IPAddress ipAddress;
//		
//
//		/**
//		 * Constructor
//		 */
//		public LocalTCPConnection(bool IsServer, IPAddress ip, UInt16 Port)
//		{
//			isServer  = IsServer;
//			port      = Port;
//			ipAddress = ip;
//		}
//
//        /**
//         * Listen
//         * Can only be executed when server
//         */
//        public bool Listen()
//		{
//			if (isServer == false) {
//				return false;
//			}
//			try
//			{
//				enterCriticalSection();
//				if (socket == null) {
//					socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
//					
//					
//					// IPEndPoint EP = new IPEndPoint(IPAddress.Any, port);
//					
//					IPAddress ipAdd = IPAddress.Parse("192.168.43.133");
//					//IPAddress ipAdd = IPAddress.Parse(ip);
//					IPEndPoint EP = new IPEndPoint(ipAdd, port);
//					socket.Bind(EP);
//					socket.Listen(1);
//					socket.BeginAccept(new AsyncCallback(SocketEventCallback.AcceptCallback), this);
//				}
//			}
//			finally
//			{
//				leaveCriticalSection();
//			}
//			return true;
//		}
//
//        /**
//         * Connect to the local host server
//         * 
//         * Can only be executed when client
//         */
//        public bool Connect() 
//		{
//			if (isServer == true){
//				return false;
//			}
//			try
//			{
//				enterCriticalSection();
//				socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
//
//			
//				IPAddress ipAdd = IPAddress.Parse("192.168.43.133");
//				//IPEndPoint EP = new IPEndPoint(IPAddress.Loopback, port);
//				IPEndPoint EP = new IPEndPoint(ipAdd, port);
//				socket.BeginConnect(EP, new AsyncCallback(SocketEventCallback.ConnectCallback), this);
//			}
//			finally
//			{
//				leaveCriticalSection();
//			}
//			return true;
//		}
//
//		/**
//		 * Disconnect
//		 */
//		public void Disconnect() 
//		{
//			try
//			{
//				enterCriticalSection();
//				if (socket != null){
//					if (IsServer){
//						Console.WriteLine("Disconnect Server");
//						if (clientSocket != null){
//							clientSocket.Close();
//							// clientSocket.Shutdown(SocketShutdown.Both);
//							clientSocket = null;
//						}
//					}
//					else{
//						Console.WriteLine("Disconnect Client");
//					}
//					//  socket.Shutdown(SocketShutdown.Both);
//					socket.Close();
//					socket		= null;
//					IsConnect	= false;
//				}
//			}
//			finally
//			{
//				leaveCriticalSection();
//			}
//		}
//
//        /**
//         * Data transceiver 
//         */
//        public bool DataExchange()
//		{
//			try 
//			{
//				try
//				{
//					enterCriticalSection();
//					byte[] ArrayX	= BitConverter.GetBytes(myPosition.X);
//					byte[] ArrayY = BitConverter.GetBytes(myPosition.Y);
//					ArrayX.CopyTo(sendBuffer, 0);
//					ArrayY.CopyTo(sendBuffer, ArrayX.Length);
//					
//				
//					if (isServer){
//						if (clientSocket == null || IsConnect == false){
//							return false;
//						}
//						clientSocket.BeginSend(sendBuffer, 0, sendBuffer.Length, 0, new AsyncCallback(SocketEventCallback.SendCallback), this);
//						clientSocket.BeginReceive(recvBuffer, 0, recvBuffer.Length, 0, new AsyncCallback(SocketEventCallback.ReceiveCallback), this);
//						
//					}
//					else{
//						if (socket == null || IsConnect == false){
//							return false;
//						}
//						socket.BeginSend(sendBuffer, 0, sendBuffer.Length, 0, new AsyncCallback(SocketEventCallback.SendCallback), this);
//						socket.BeginReceive(recvBuffer, 0, recvBuffer.Length, 0, new AsyncCallback(SocketEventCallback.ReceiveCallback), this);
//					}
//				}
//				finally
//				{
//					leaveCriticalSection();
//				}
//			}
//			catch(System.Net.Sockets.SocketException e)
//			{
//				if (e.SocketErrorCode == SocketError.ConnectionReset || e.SocketErrorCode == SocketError.ConnectionAborted){
//					Console.WriteLine("DataExchange 切断検出");
//					Disconnect();
//				}
//				Console.WriteLine("ExchangeError " + e.ToString());
//			}
//			return true;
//		}
//
//
//		/***
//		 * Accept
//		 */
//		public void OnAccept(IAsyncResult AsyncResult)
//		{
//			try
//			{
//				try
//				{
//					enterCriticalSection();
//					if (Socket != null){
//						ClientSocket = Socket.EndAccept(AsyncResult);
//						Console.WriteLine("Accept " + ClientSocket.RemoteEndPoint.ToString());
//						IsConnect = true;
//					}
//				}
//				finally
//				{
//					leaveCriticalSection();
//				}
//			}
//			catch (Exception e)
//			{
//				Console.WriteLine(e.ToString());
//			}
//			Console.WriteLine("OnAccept");
//			testStatus = "OnAccept";
//		}
//		/***
//		 * Connect
//		 */
//		public void OnConnect(IAsyncResult AsyncResult)
//		{
//			try
//			{
//				try
//				{
//					enterCriticalSection();
//					if (Socket != null){
//						// Complete the connection.
//						Socket.EndConnect(AsyncResult);
//						Console.WriteLine("Connect " + Socket.RemoteEndPoint.ToString());
//						IsConnect = true;
//					}
//				}
//				finally
//				{
//					leaveCriticalSection();
//				}
//			}
//			catch (System.Net.Sockets.SocketException e)
//			{
//				if (e.SocketErrorCode == SocketError.ConnectionRefused){
//					Disconnect();
//				}
//			}
//			catch (Exception e)
//			{
//				Console.WriteLine(e.ToString());
//			}
//			Console.WriteLine("OnConnect");
//			testStatus = "OnConnect";
//		}
//		
//		/**
//		 * Receive
//		 */
//		public void OnReceive(IAsyncResult AsyncResult)
//		{
//			int Len = 0;
//			try
//			{
//				try
//				{
//					enterCriticalSection();
//					if (IsServer){
//						if (ClientSocket != null){
//							Len = ClientSocket.EndReceive(AsyncResult);
//							// 切断
//							if (Len <= 0){
//								Disconnect();
//							}
//							else{
//								
//								
//								////////////////////////////////////////////////////////////////////TODO/////////////////////////////////////////
//								
//								
//								
//								
//								networkPosition.X = BitConverter.ToSingle(recvBuffer, 0);
//								networkPosition.Y = BitConverter.ToSingle(recvBuffer, 4);
//								
//								if(networkPosition.X == 0 && networkPosition.Y == 0)
//								{
//									isConnect = true;
//								}
//								Console.WriteLine("Host: OnReceive");
//								testStatus = "Host: OnReceive";
//							}
//						}
//					}
//					else{
//						if (Socket != null){
//							Len = Socket.EndReceive(AsyncResult);
//							// 切断
//							if (Len <= 0){
//								Disconnect();
//							}
//							else{
//								
//
//								
//								networkPosition.X = BitConverter.ToSingle(recvBuffer, 0);
//								networkPosition.Y = BitConverter.ToSingle(recvBuffer, 4);
//								
//								if(networkPosition.X == 0 && networkPosition.Y == 0)
//								{
//									isConnect = true;
//								}
//								Console.WriteLine("Client: OnReceive");
//								
//							}
//						}
//					}
//				}
//				finally
//				{
//					leaveCriticalSection();
//				}
//			}
//			catch (System.Net.Sockets.SocketException e)
//			{
//				if (e.SocketErrorCode == SocketError.ConnectionReset || e.SocketErrorCode == SocketError.ConnectionAborted){
//					Console.WriteLine("ReceiveCallback 切断検出");
//					Disconnect();
//				}
//			}
//			catch (Exception e)
//			{
//				Console.WriteLine(e.ToString());
//			}
//			//Console.WriteLine("OnReceive");
//		}
//		
//		/**
//		 * Send
//		 */
//		public void OnSend(IAsyncResult AsyncResult)
//		{
//			int Len = 0;
////			int a = 0;
//			try
//			{
//				try
//				{
//					enterCriticalSection();
//					if (IsServer){
//						if (ClientSocket != null){
//							Len = ClientSocket.EndSend(AsyncResult);
//						}
//					}
//					else{
//						if (Socket != null){
//							Len = Socket.EndSend(AsyncResult);
//						}
//					}
//                    // Disconnection detection should go here...
//					if (Len <= 0){
//						// send error
//					}
//				}
//				finally
//				{
//					leaveCriticalSection();
//				}
//			}
//			catch (System.Net.Sockets.SocketException e)
//			{
//				if (e.SocketErrorCode == SocketError.ConnectionReset || e.SocketErrorCode == SocketError.ConnectionAborted){
//					Console.WriteLine("SendCallback 切断検出");
//					Disconnect();
//				}
//			}
//			catch (Exception e)
//			{
//				Console.WriteLine(e.ToString());
//			}
////			Console.WriteLine("OnSend");
//		}
//	};
	public class TwoPlayer : Sce.PlayStation.HighLevel.GameEngine2D.Scene
	{
		string whichInternet= "Uni";
		bool isHost = AppMain.ISHOST;
		string homeIP = "192.168.0.10";
		string uniIP = "10.54.152.214";
		string phoneIP = "192.168.43.133";
		string ip ;
//		private Socket connection ; 
//        private Thread readThread ;
//        private NetworkStream socketStream;
//        private BinaryWriter writer;
//        private BinaryReader reader;
//		TcpListener listener;
		
		//LocalTCPConnection server;
		//LocalTCPConnection client;
		
		bool testing = true;
		
		public bool             isConnected;
		public bool             readyToSend;
		#region Member Properties - Labels
		private Sce.PlayStation.HighLevel.GameEngine2D.Label lblTopLeft;
		private Sce.PlayStation.HighLevel.GameEngine2D.Label lblTopRight;
		private Sce.PlayStation.HighLevel.GameEngine2D.Label lblBottomLeft;
		private Sce.PlayStation.HighLevel.GameEngine2D.Label lblBottomRight;
		private Sce.PlayStation.HighLevel.GameEngine2D.Label lblDebugLeft;
		private Sce.PlayStation.HighLevel.GameEngine2D.Label lblDebugCenter;
		#endregion
		#region Screen dimensions
		private int screenWidth;
		private int screenHeight;
		#endregion
		
		#region Fonts
		Font font;		
		FontMap debugFont;
		#endregion
		
		public bool isPlayer1Ready;
		public bool isPlayer2Ready;
		public bool startConnection;
	
		public LobbyUI lobbyUI;
		
		public Timer refreshTimer;
		private Timer chatlobbyRefreshTimer;
		public Dictionary<string,string> activePlayers = new Dictionary<string, string>();
		private int activePlayerCount = 0;
		
		//chat lobby ... update
		bool lobbychat1dot = true;
		bool lobbychat2dot;
		bool lobbychat3dot;
		public TwoPlayer ()
		{
			
			
			PushTransition push = new PushTransition();
			push.MoveDirection = FourWayDirection.Up;
			lobbyUI = new LobbyUI(this);
			UISystem.SetScene(lobbyUI, push);

		refreshTimer = new Timer();
		chatlobbyRefreshTimer = new Timer();
//			if(!testing)
//				ip = accomIP;
			this.Camera.SetViewFromViewport();	// Check documentation - defines a 2D view that matches the viewport(viewport == display region, in our case, the vita screen)
			
//			#region Set screen width and height variables
//			screenWidth = Director.Instance.GL.Context.Screen.Width;
//			screenHeight = Director.Instance.GL.Context.Screen.Height;
//			#endregion
//			
//			#region Instantiate Fonts
//			font = new Font(FontAlias.System, 25, FontStyle.Bold);
//			debugFont = new FontMap(font, 25);
//			
//			// Reload the font becuase FontMap disposes of it
//			font = new Font(FontAlias.System, 25, FontStyle.Bold);
//			#endregion
//			#region Instantiate label objects
//			lblTopLeft = new Sce.PlayStation.HighLevel.GameEngine2D.Label();
//			
//			lblTopRight = new Sce.PlayStation.HighLevel.GameEngine2D.Label();
//			lblBottomLeft = new Sce.PlayStation.HighLevel.GameEngine2D.Label();
//			lblBottomRight = new Sce.PlayStation.HighLevel.GameEngine2D.Label();
//			lblDebugLeft = new Sce.PlayStation.HighLevel.GameEngine2D.Label();
//			lblDebugCenter = new Sce.PlayStation.HighLevel.GameEngine2D.Label();
//			#endregion
//			
//			#region Assign label values
////			lblTopLeft.FontMap = debugFont;
////			lblTopLeft.Text = "Player 1";
////			lblTopLeft.Position = new Vector2 (100, screenHeight - 200);
//			
////			lblTopLeft.FontMap = debugFont;
////			lblTopLeft.Text = "My Details";
////			lblTopLeft.Position = new Vector2 (100, screenHeight - 100);
////			
////			lblTopRight.FontMap = debugFont;
////			lblTopRight.Text = "Available Players";
////			lblTopRight.Position = new Vector2(screenWidth - 300, screenHeight - 100);
////			
////			lblBottomLeft.FontMap = debugFont;
////			lblBottomLeft.Text = "Waiting";
////			lblBottomLeft.Position = new Vector2(100, 300);
////			
////			lblBottomRight.FontMap = debugFont;
////			lblBottomRight.Text = "Waiting";
////			lblBottomRight.Position = new Vector2(screenWidth -200, 300);
//			
////			lblDebugLeft.FontMap = debugFont;
////			lblDebugLeft.Text = "Waiting for both connections";
////			lblDebugLeft.Position = new Vector2(430, 200);
//			
////			lblDebugCenter.FontMap = debugFont;
////			lblDebugCenter.Text = "----";
////			lblDebugCenter.Position = new Vector2(430, 100);
//			
//			
//			
//			
//			#endregion
//			textureInfo  = new TextureInfo("/Application/assets/bullet.png");
//			sprite	 		= new SpriteUV();
//			sprite 			= new SpriteUV(textureInfo);	
//			sprite.Quad.S 	= textureInfo.TextureSizef;
//			sprite.Position = new Vector2(50.0f,Director.Instance.GL.Context.GetViewport().Height*0.2f);
//			
//			texture2Info  = new TextureInfo("/Application/assets/bullet.png");
//			sprite2	 		= new SpriteUV();
//			sprite2 			= new SpriteUV(textureInfo);	
//			sprite2.Quad.S 	= textureInfo.TextureSizef;
//			sprite2.Position = new Vector2(450.0f,Director.Instance.GL.Context.GetViewport().Height*0.2f);
//			
//			sprite.Visible = false;
//					sprite2.Visible = false;
//			
//			
//			Sce.PlayStation.HighLevel.UI.EditableText text= new Sce.PlayStation.HighLevel.UI.EditableText();
//				text.Text = "Input IP";
//				text.SetPosition(300,300);
//			
//			#region Add labels to scene (Debug Overlay)
//			this.AddChild(lblTopLeft);
//			this.AddChild(lblTopRight);
//			this.AddChild(lblBottomLeft);
//			this.AddChild(lblBottomRight);
//			this.AddChild(lblDebugLeft);
//			this.AddChild(lblDebugCenter);
//			this.AddChild(sprite);
//			this.AddChild(sprite2);
//		
//		//	uiScene.RootWidget.AddChildFirst(buttonClient);
//		//	uiScene.RootWidget.AddChildFirst(text);
//			
		//	#endregion
			
			
			Scheduler.Instance.ScheduleUpdateForTarget(this, 1, false);	// Tells the director that this "node" (a.k.a scene - see doc) requires to be updated
			Director.Instance.DebugFlags = Director.Instance.DebugFlags | DebugFlags.DrawGrid;
			this.DrawGridStep = 20.0f;
			
			
			
//			var request = (HttpWebRequest)WebRequest.Create("http://localhost:9010/newemployee");
//			
//			var postData = "firstname=" +AppMain.PLAYERNAME;
//			postData += "&surname=Smith";
//			postData += "&email=danda@sfsndf.com";
//			var data = Encoding.ASCII.GetBytes(postData);
//			
//			request.Method = "POST";
//			request.ContentType = "application/x-www-form-urlencoded";
//			request.ContentLength = data.Length;
//
//			using (var stream = request.GetRequestStream())
//			{
//			    stream.Write(data, 0, data.Length);
//			}
//			
//			var response = (HttpWebResponse)request.GetResponse();
//			
//			var responseString = new StreamReader(response.GetResponseStream()).ReadToEnd();
//				
			
//			if(testing)
//			{
//				if(isHost)
//				{
//					AppMain.client = new LocalTCPConnection(true,11000);
//					//server = new LocalTCPConnection(true, 11000);
//					if(AppMain.client.Listen())
//					{
//						Console.WriteLine("working ");
//						lblBottomLeft.Text = "Ready";
//						isPlayer1Ready = true;
//						lblDebugCenter.Text = "IP Address = " + AppMain.client.GetIP ;	
//					}
//					else
//					{
//						Console.WriteLine("Fucked ");
//					}
//				}
//				else
//				{
//					AppMain.client = new LocalTCPConnection(false,11000);
//					AppMain.client.SetIPAddress(AppMain.IPADDRESS);
//					//client = new LocalTCPConnection(false, 11000);
//					lblDebugCenter.Text = "Client";
//				}
//				
//			}
		}
		
		public override void Draw ()
		{
			
			base.Draw ();
			
		}
		
		public override void Update (float dt)
		{
		
			base.Update (dt);
			
			
			
			
			if(isPlayer1Ready && isPlayer2Ready)
			{
				Console.WriteLine("Players READY");	
			}
			else
			{
				#region chatlobbyrefreshdots
				if(chatlobbyRefreshTimer.Milliseconds() > 1000 && lobbychat1dot )
				{
					lobbychat1dot = false;
					lobbychat2dot = true;
					string lob = lobbyUI.LblLobbyChat.Text;
					//string lob = lobbyUI.lblLobbyChat.Text;
					lob += " .";
					lobbyUI.LblLobbyChat.Text = lob;
				}
				if(chatlobbyRefreshTimer.Milliseconds() > 2000 && lobbychat2dot )
				{
					lobbychat2dot = false;
					lobbychat3dot = true;
					string lob = lobbyUI.LblLobbyChat.Text;
					lob += " .";
					lobbyUI.LblLobbyChat.Text = lob;
				}
				if(chatlobbyRefreshTimer.Milliseconds() > 3000 && lobbychat3dot )
				{
					lobbychat3dot = false;
					
					string lob = lobbyUI.LblLobbyChat.Text;
					lob += " .";
					lobbyUI.LblLobbyChat.Text = lob;
				}
				if(chatlobbyRefreshTimer.Milliseconds() > 4000)
				{
					lobbychat1dot=true;
					lobbyUI.LblLobbyChat.Text = lobbyUI.LblLobbyChat.Text.Substring(0,lobbyUI.LblLobbyChat.Text.Length -6);//
				chatlobbyRefreshTimer.Reset();	
				}
				#endregion
				
				//Console.WriteLine(refreshTimer.Milliseconds());
				if( refreshTimer.Milliseconds() > 3000)
				{
					//Console.WriteLine("REFRESHED \n Checking Players : P1 =" + lobbyUI.p1Ready + " AND P2 = " + lobbyUI.p2Ready);
					GetRequest();
					if(activePlayers.Count > activePlayerCount)
					{
						int i = 1;
					 	foreach( var item in activePlayers)
						{
							if(!item.Value.Equals(AppMain.PLAYERNAME) && !item.Key.Equals(AppMain.IPADDRESS))
							{
								Button button = new Button();
								button.SetPosition(80,70 * i);
								button.Text = item.Value;
								button.TouchEventReceived += HandleButtonTouchEventReceived;
								i++;
								lobbyUI.PnlActivePlayers.AddChildFirst(button);
							}
						}
						
						activePlayerCount = activePlayers.Count;
					}
					
					refreshTimer.Reset();
				}
				
				if(AppMain.ISHOST)
				{
					if(AppMain.client.IsConnect)
					{
						lobbyUI.p2Ready = true;
						Console.WriteLine("Player 2 Ready");	
					}
				}
				else
				{
					if(AppMain.client.IsConnect)
					{
						lobbyUI.p1Ready = true;
						Console.WriteLine("Player 1 Ready");	
					}
					
				}
			}
			#region notTesting
			if(!testing)
			{
//				if(startConnection)
//				{
//					if(lblBottomLeft.Text.Equals("Waiting"))
//					{
//						FadeText(dt,lblBottomLeft,1,p1FadeUp,p1FadeDown);
//						
//					}
//					if(lblBottomRight.Text.Equals("Waiting"))
//					{
//						FadeText(dt,lblBottomRight,2,p2FadeUp,p2FadeDown);
//					}
//				}
//				else if (!startConnection)
//				{
//					if(isHost)
//					{
//						lblBottomLeft.Text = "Press X";
//						lblBottomRight.Text = "";
//					}
//					else
//					{
//						lblBottomRight.Text = "Press X";
//						lblBottomLeft.Text = "";
//					}
//				}
//				
//				if(Input2.GamePad0.Cross.Press)
//				{
//					if(!startConnection)
//					{
//						if(!isHost)
//						{
//							lblBottomRight.Text = "Waiting";
//					 		Thread readThread = new Thread(new ThreadStart(RunClient));
//		            		readThread.Start();	
//							startConnection = true;
//						}
//						else if(isHost)
//						{
//							lblBottomLeft.Text = "Waiting";
//							Thread readThread = new Thread(new ThreadStart(RunServer));
//		            		readThread.Start();	
//							startConnection = true;
//						}
//					}
//				}
//				
//				if(isPlayer1Ready && isPlayer2Ready)
//				{
//					lblDebugLeft.Text = "Press start to continue";	
//					if(Input2.GamePad0.Start.Press)
//					{
//						
//					}
//				}
			}
			#endregion
			else
			{
				
				
				 
				
//				float screenheight = 544.0f;
//					float screenwidth = 960.0f;
//				
//				List<TouchData> touchDataList = Touch.GetData(0);
//			UISystem.Update(touchDataList);
//				//UISystem.Render();
//			
//				if(touchDataList.Count > 0)
//				{
//					float screenx = (touchDataList[0].X +0.5f) * screenwidth;
//					float screenY = (touchDataList[0].Y +0.5f) * screenheight;
//				if(buttonClient.HitTest(new Vector2(screenx,screenY)) && touchDataList[0].Status == TouchStatus.Down)
//						{
//							
//							if(textbox.Text.Length > 0)
//							{
//								
//								
//							}
//							
//						}
//				}
//				if(isHost)
//				{
//					
//					if(AppMain.client.IsConnect)
//					{
//						lblBottomRight.Text = "Ready";
//						isPlayer2Ready = true;
//					}
//					
//				}
//				else
//				{
//					lblDebugCenter.Text = AppMain.client.GetIP;
//					if(Input2.GamePad0.Circle.Press)
//					{
//						AppMain.client.Connect();
//						lblBottomRight.Text = "Ready";
//						isPlayer2Ready = true;
//					}
//					
////				
//					if(AppMain.client.IsConnect)
//					{
//						lblBottomLeft.Text = "Ready";
//						isPlayer1Ready = true;
//					}
//				}
//				
//				if(isPlayer1Ready && isPlayer2Ready)
//				{
//					//sprite.Visible = true;
//					//sprite2.Visible = true;
//					lblBottomLeft.Visible = false;
//					lblBottomRight.Visible = false;
//					lblTopLeft.Visible = false;
//					lblTopRight.Visible = false;
//					lblDebugLeft.Visible = false;
//					lblDebugCenter.Visible = false;
//					
////					TextureManager.AddAsset("tiles", new TextureInfo(new Texture2D("/Application/assets/tiles.png", false),
////			                                                 new Vector2i(10, 3)));
////					TextureManager.AddAsset("entities", new TextureInfo(new Texture2D("/Application/assets/dungeon_objects.png", false),
////			                                                 new Vector2i(9, 14)));
////					TextureManager.AddAsset("background", new TextureInfo("/Application/assets/Background.png"));
//					
//					TextureManager.AddAsset("tiles", new TextureInfo(new Texture2D("/Application/assets/SpriteSheetMaster-Recovered.png", false),
//			                                                 new Vector2i(4, 8)));
//					TextureManager.AddAsset("entities", new TextureInfo(new Texture2D("/Application/assets/dungeon_objects.png", false),
//			                                                 new Vector2i(9, 14)));
//					TextureManager.AddAsset("background", new TextureInfo("/Application/assets/Background.png"));
//					
//					Info.TotalGameTime = 0f;
//					
//					MultiplayerLevel level = new MultiplayerLevel();
//					level.Camera.SetViewFromViewport();
//					GameSceneManager.currentScene = level;
//					Director.Instance.ReplaceScene(level);
//				}
			}
		}

		void HandleButtonTouchEventReceived (object sender, TouchEventArgs e)
		{
			if(e.TouchEvents[0].Type == TouchEventType.Down)
			{
				Button button = (Button)sender;
				//Console.WriteLine(button.Text);
				lobbyUI.BtnJoinGame.Enabled = true;
				lobbyUI.BtnJoinGame.Visible = true;
				
				foreach(var item in activePlayers)
				{
					if(item.Value.Equals(button.Text))
					{
						Console.WriteLine(item.Value + " : " + item.Key);
						AppMain.IPADDRESS = item.Key;
						                 
					}
				}
			}
		}
		
		private void FadeText(float dt,Sce.PlayStation.HighLevel.GameEngine2D.Label l,int player , bool fadeUp, bool fadeDown)
		{
			
//					if(!fadeUp && fadeDown)
//				{
//					l.Color.A -= dt;
//					if(l.Color.A < 0)
//					{
//						if(player == 1)
//						{	
//							p1FadeDown = false;
//							p1FadeUp = true;
//						}
//						else
//						{
//							p2FadeDown = false;
//							p2FadeUp = true;
//						}
//					}
//				}
//			else if(fadeUp && !fadeDown)
//				{
//					l.Color.A += dt;
//					if(l.Color.A > 1)
//					{
//						if(player == 1)
//						{	
//							p1FadeDown = true;
//							p1FadeUp = false;
//						}
//						else 
//						{
//							p2FadeDown = true;
//							p2FadeUp = false;
//						}	
//					}
//				}
			
		}
		
		public void RunServer()
		{
//			try {
//				
//				IPAddress local = IPAddress.Parse(ip);
//				
//				listener = new TcpListener(local,5000);
//				listener.Start();
//				
//				
//				while(true)
//				{
//					connection = listener.AcceptSocket();
//					new Thread(new ParameterizedThreadStart(HandleClient)).Start(connection);
//				}
//			} catch (Exception ex) {
//				lblBottomLeft.Text="Player 1 Error" + ip.ToString();
//			}
		}
		public void RunClient()
		{
//			 try
//            {
//				//lblBottomRight.Text="Player 2 Not Ready";
//                //creates new client
//                TcpClient client = new TcpClient();
//                //sets ip address and port
//				
//                client.Connect(ip, 5000);
//				
//				socketStream = client.GetStream();//
//				
//				writer = new BinaryWriter(socketStream);
//				reader = new BinaryReader(socketStream);
//				isConnected = true;
//				readyToSend = true;
//				while(isConnected)
//               {
//                   if(readyToSend)
//                   {
//                    	writer.Write ("Connected");
//						isPlayer2Ready = true;
//						lblBottomRight.Text = "Ready";
//						lblBottomRight.Color.A = 1;
//						readyToSend = false;
//                   }
//					
//					
//				string message = reader.ReadString();
//					if(message.Equals("Ready"))
//					{
//						lblBottomLeft.Text = "Ready";
//                  		lblBottomLeft.Color.A = 1;
//						isPlayer1Ready = true;
//					}
//               }
//			
//			}
//			catch (Exception ex) {
//				lblBottomRight.Color.A = 1;
//				lblBottomRight.Text="Client ERROR";
//			}
//			
			
		}
		
		public void HandleClient(object obj)
		{
			
//			Socket clientSocket = obj as Socket;
//            if(clientSocket != null)
//            {
//                //create a socketstream and a reader which will be used for recieving messages
//                NetworkStream socketStream = new NetworkStream(clientSocket);
//           		writer = new BinaryWriter(socketStream);		
//                BinaryReader reader = new BinaryReader(socketStream);
//              
//
//              string message = reader.ReadString();
//				
//				
//          if(message.Equals("Connected"))
//				{
//					lblBottomRight.Text = "Ready";
//					lblBottomRight.Color.A = 1;
//					
//					isPlayer2Ready = true;
//					writer.Write("Ready");
//					isPlayer1Ready = true;
//					lblBottomLeft.Text = "Ready";
//					lblBottomLeft.Color.A = 1;
//				}
//				
//			}
		}
		
		public void PostRequest()
		{
			
			
				var request = (HttpWebRequest)WebRequest.Create("https://ec-server.herokuapp.com/adduser");
			
				
					var postData = "username=" +AppMain.PLAYERNAME;
					postData += "&ipaddress=" + AppMain.IPADDRESS;
			
					var data = Encoding.ASCII.GetBytes(postData);
				
					request.Method = "POST";
					request.ContentType = "application/x-www-form-urlencoded";
					request.ContentLength = data.Length;
	
					using (var stream = request.GetRequestStream())
					{
					    stream.Write(data, 0, data.Length);
					}
					
					var response = (HttpWebResponse)request.GetResponse();
					var responseString = new StreamReader(response.GetResponseStream()).ReadToEnd();	
					var res = JsonObject.Parse(responseString);
		}
		
		public void GetRequest()
		{
			
			
			var request = (HttpWebRequest)WebRequest.Create("https://ec-server.herokuapp.com/userlist");

				var response = (HttpWebResponse)request.GetResponse();

				var responseString = new StreamReader(response.GetResponseStream()).ReadToEnd();	
				var res = JsonValue.Parse(responseString);
			
				foreach (var item in res)
				{
					string ip = item.Value.GetValue("ipaddress").ToString();
					ip = ip.Trim('"');
					string user = item.Value.GetValue("username").ToString();
					user = user.Trim('"');
//					Console.WriteLine(item.Value.GetValue("username"));
					if(!activePlayers.ContainsKey(ip))
					{
						activePlayers.Add(ip,user);
					}
				}
		}
	}
}
