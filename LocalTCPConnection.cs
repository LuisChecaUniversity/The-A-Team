using System;
using System.Net;
using System.Net.Sockets;
using System.IO;
using System.Threading;

namespace TheATeam
{
	/**
	 * SocketListenerInterface
	 */
	interface ISocketListener
	{
		/**
		 * Accept
		 */
		void OnAccept(IAsyncResult AsyncResult);
		/**
		 * Connect
		 */
		void OnConnect(IAsyncResult AsyncResult);
		/**
		 * Receive
		 */
		void OnReceive(IAsyncResult AsyncResult);
		/**
		 * Send
		 */
		void OnSend(IAsyncResult AsyncResult);
	}
	/**
	 * SocketEventCallback
	 */
	class SocketEventCallback
	{
		/**
		 * AcceptCallback
		 */
		public static void AcceptCallback(IAsyncResult AsyncResult) 
		{
			LocalTCPConnection Server = (LocalTCPConnection)AsyncResult.AsyncState;
			Server.OnAccept(AsyncResult);
		}

		/**
		 * ConnectCallback
		 */
		public static void ConnectCallback(IAsyncResult AsyncResult)
		{
			LocalTCPConnection Client = (LocalTCPConnection)AsyncResult.AsyncState;
			Client.OnConnect(AsyncResult);
		}
		/**
		 * ReceiveCallback
		 */
		public static void ReceiveCallback(IAsyncResult AsyncResult)
		{
			LocalTCPConnection TCPs = (LocalTCPConnection)AsyncResult.AsyncState;
			TCPs.OnReceive(AsyncResult);
		}

		/**
		 * SendCallback
		 */
		public static void SendCallback(IAsyncResult AsyncResult)
		{
			LocalTCPConnection TCPs = (LocalTCPConnection)AsyncResult.AsyncState;
			TCPs.OnSend(AsyncResult);
		}
	}
	
	
	
	/**
	 * Class for SocketTCP local connection
	 */
	public class LocalTCPConnection : ISocketListener
	{
		/**
		 * Status
		 */
		public enum Status
		{
			kNone,		
			kListen,	// Listen or connecting
			kConnected,	
			kUnknown
		}
/*
		using (CriticalSection CS = new CriticalSection(syncObject))
		{
		
		}
		public class CriticalSection : IDisposable
		{
			private object syncObject = null;
			public CriticalSection(object SyncObject)
			{
				syncObject = SyncObject;
				Monitor.Enter(syncObject);
			}

			public virtual void Dispose()
			{
				Monitor.Exit(syncObject);
				syncObject = null;
			}
		}
*/
        /**
         * Object for exclusive  socket access
         */
        private object syncObject = new object();
		/**
		 * Enter critical section
		 */
		private void enterCriticalSection() 
		{
			Monitor.Enter(syncObject);
		}
		/**
		 * Leave critical section
		 */
		private void leaveCriticalSection() 
		{
			Monitor.Exit(syncObject);
		}
		
		

		/**
		 * Get status
		 * 
		 * @return Status
		 */
		public Status StatusType
		{
			get
			{
				try
				{
					enterCriticalSection();
					if (Socket == null){
						return Status.kNone;
					}
					else{
						if (IsServer){
							if(ClientSocket == null)
							{
								return Status.kListen;
							}
							return Status.kConnected;
						}
						else{
							if(IsConnect == false){
								return Status.kListen;
							}
							return Status.kConnected;
						}
					}
				}
				finally
				{
					leaveCriticalSection();
				}
			}
		}

        /**
         * Get status as string
         * 
         * @return status string
         */
        public string statusString
		{
			get
			{
				switch (StatusType)
				{
					case Status.kNone:
						return "None";

					case Status.kListen:
						if (IsServer){
							return "Listen";
						}
						else{
							return "Connecting";
						}

					case Status.kConnected:
						return "Connected";
				}
				return "Unknown";
			}
		}

		/**
		 * Get the button string based on status
		 * 
		 * @return button string
		 */
		public string buttonString
		{
			get
			{
				switch (StatusType)
				{
					case Status.kNone:
						if (IsServer){
							return "Listen";
						}
						else{
							return "Connect";
						}
					case Status.kListen:
						return "Disconnect";
					case Status.kConnected:
						return "Disconnect";
				}
				return "Unknown";
			}
		}

        /**
		 * Process the button that lets us change the status based on 
		 * current status 
         */
        public void ChangeStatus()
		{
			switch(StatusType)
			{
				case	Status.kNone:
					if (IsServer){
						Listen();
					}
					else{
						Connect();
					}
					break;

				case	Status.kListen:
					Disconnect();
					break;
				
				case	Status.kConnected:
					Disconnect();
					break;
			}
		}

        /**
         * transceiver buffer
         */
		private char actionMsg = 'D';
		public void SetActionMessage(char c)
		{
			actionMsg = c;	
		}
		public char ActionMsg { get { return actionMsg;}}
        private byte[] sendBuffer = new byte[26];
		private byte[] recvBuffer = new byte[26];

		public string testStatus = "Nothing";

		/**
		 * Our position or the other party's
		 */
		private Sce.PlayStation.Core.Vector2 myPosition		= new Sce.PlayStation.Core.Vector2(400, 200);
		public	Sce.PlayStation.Core.Vector2 MyPosition
		{
			get { return myPosition; }
		}
		public	void	SetMyPosition(float X, float Y)
		{
			myPosition.X = X;
			myPosition.Y = Y;
		}

		
		public Sce.PlayStation.Core.Vector2 networkPosition	= new Sce.PlayStation.Core.Vector2(400, 200);
		public Sce.PlayStation.Core.Vector2 NetworkPosition
		{
			get { return networkPosition; }
		}
		public Sce.PlayStation.Core.Vector2 networkShootDir	= new Sce.PlayStation.Core.Vector2(400, 200);
		public Sce.PlayStation.Core.Vector2 NetworkShootDir
		{
			get { return networkShootDir; }
		}
		
		
		private bool hasShot = false;
		public bool HasShot { get { return hasShot;}}
		public void SetHasShot(bool t)
		{
		hasShot = t;	
		}
		
		private Sce.PlayStation.Core.Vector2 myShootingDirection = new Sce.PlayStation.Core.Vector2(0.0f,0.0f);
		public	void	SetMyShootingDirection(float X, float Y)
		{
			myShootingDirection.X = X;
			myShootingDirection.Y = Y;
		}
		public Sce.PlayStation.Core.Vector2 MyShootingDirection
		{
			get { return myShootingDirection;}	
		}
		
		private Sce.PlayStation.Core.Vector2 myDirection = new Sce.PlayStation.Core.Vector2(0.0f,0.0f);
		public	void	SetMyDirection(float X, float Y)
		{
			myDirection.X = X;
			myDirection.Y = Y;
		}
		public Sce.PlayStation.Core.Vector2 MyDirection
		{
			get { return myDirection;}	
		}
		
		private Sce.PlayStation.Core.Vector2 networkDirection = new Sce.PlayStation.Core.Vector2(0.0f,0.0f);
		public Sce.PlayStation.Core.Vector2 NetworkDirection
		{
			get { return networkDirection;}	
		}
		private char networkActionMsg = 'D';
		
		public char NetworkActionMsg { get { return networkActionMsg; }}
		
		/**
		 * Are we connected
		 */
		private bool isConnect = false;
		public bool IsConnect
		{
					get	{	return isConnect; }
			private set	{	this.isConnect = value;	}
		}

        /**
         * Socket  Listen when server  Server connect when client
         */
        private Socket socket;
		public  Socket Socket 
		{
			get	{	return socket;	}
		}

		/**
		 * Client socket when server
		 */
		private Socket clientSocket;
		public Socket ClientSocket
		{
					get	{	return clientSocket;	}
			private set	{	this.clientSocket = value;	}
		}

		/**
		 * Is this a server
		 */
		private bool isServer;
		public bool IsServer
		{
			get	{	return isServer;	}
		}

		/**
		 * Port number
		 */
		private UInt16 port;
		public UInt16 Port
		{
			get	{	return port;	}
		}
		
		private IPAddress ipAddress;
		public void SetIPAddress(string ip)
		{
		ipAddress = IPAddress.Parse(ip);	
		}
		public String GetIP
		{
			get { return ipAddress.ToString();}	
		}

		/**
		 * Constructor
		 */
		public LocalTCPConnection(bool IsServer, UInt16 Port)
		{
			isServer  = IsServer;
			port      = Port;
			
		}

        /**
         * Listen
         * Can only be executed when server
         */
        public bool Listen()
		{
			if (isServer == false) {
				return false;
			}
			try
			{
				enterCriticalSection();
				if (socket == null) {
					socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
					
					
				
				
					IPHostEntry host;
		   			host = Dns.GetHostEntry(Dns.GetHostName());
					
					if(AppMain.ISCOMPUTER)
					{
						ipAddress = host.AddressList[1];
							
					}
					else
					{
			   			foreach (IPAddress ipp in host.AddressList)
			   			{
							
			     			if (ipp.AddressFamily == AddressFamily.InterNetwork)
			     			{
						       	ipAddress = ipp;
			       				break;
			     			}
			   			}
				
					}
					AppMain.IPADDRESS = ipAddress.ToString();
			//	ipAddress = IPAddress.Parse("192.168.43.40");
					//IPEndPoint EP = new IPEndPoint(IPAddress.Loopback, port);
					
					//IPAddress ipAdd = IPAddress.Parse("192.168.43.133");
					//IPAddress ipAdd = IPAddress.Parse(myIP);
					//ipAddress = ipAdd;
					IPEndPoint EP = new IPEndPoint(ipAddress, port);
					socket.Bind(EP);
					socket.Listen(1);
					socket.BeginAccept(new AsyncCallback(SocketEventCallback.AcceptCallback), this);
				}
			}
			finally
			{
				leaveCriticalSection();
			}
			return true;
		}

        /**
         * Connect to the local host server
         * 
         * Can only be executed when client
         */
        public bool Connect() 
		{
			if (isServer == true){
				return false;
			}
			try
			{
				enterCriticalSection();
				socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
//				IPAddress ipAdd;
//				//IPAddress ipAdd = IPAddress.Parse("192.168.1.105");
//				//PHONE 
//				//IPAddress ipAdd = IPAddress.Parse("192.168.43.105");
//				// HOME WIFI 
//				if(AppMain.WHEREWIFI.Equals("HOME"))
//					ipAdd = IPAddress.Parse("192.168.0.12");
//				   
//				else if (AppMain.WHEREWIFI.Equals("UNI"))
//					ipAdd = IPAddress.Parse("10.54.152.29");
//				else if (AppMain.WHEREWIFI.Equals("PHONE"))
//					ipAdd = IPAddress.Parse("192.168.43.105");
//				else
//					ipAdd = null;
				IPAddress ipAdd = IPAddress.Parse(AppMain.CONNECTINGHOSTIPADDRESS);
				//IPAddress ipAdd = IPAddress.Parse("192.168.0.26");
				//IPEndPoint EP = new IPEndPoint(IPAddress.Loopback, port);
				
//					IPHostEntry host;
//		   			host = Dns.GetHostEntry(Dns.GetHostName());
//					
//					if(AppMain.ISCOMPUTER)
//					{
//						ipAddress = host.AddressList[1];
//							
//					}
//					else
//					{
//			   			foreach (IPAddress ipp in host.AddressList)
//			   			{
//							
//			     			if (ipp.AddressFamily == AddressFamily.InterNetwork)
//			     			{
//						       	ipAddress = ipp;
//			       				break;
//			     			}
//			   			}
//				
//					}
//					AppMain.IPADDRESS = ipAddress.ToString();
				
				IPEndPoint EP = new IPEndPoint(ipAdd, port);
				socket.BeginConnect(EP, new AsyncCallback(SocketEventCallback.ConnectCallback), this);
			}
			finally
			{
				leaveCriticalSection();
			}
			return true;
		}

		/**
		 * Disconnect
		 */
		public void Disconnect() 
		{
			try
			{
				enterCriticalSection();
				if (socket != null){
					if (IsServer){
						//Console.WriteLine("Disconnect Server");
						if (clientSocket != null){
							clientSocket.Close();
							// clientSocket.Shutdown(SocketShutdown.Both);
							clientSocket = null;
						}
					}
					else{
						//Console.WriteLine("Disconnect Client");
					}
					//  socket.Shutdown(SocketShutdown.Both);
					socket.Close();
					socket		= null;
					IsConnect	= false;
					//Console.WriteLine("Disconnected");
				}
			}
			finally
			{
				leaveCriticalSection();
			}
		}

        /**
         * Data transceiver 
         */
        public bool DataExchange()
		{
			try 
			{
				try
				{
					enterCriticalSection();
					
						
						byte[] action = BitConverter.GetBytes(actionMsg);
						byte[] ArrayX	= BitConverter.GetBytes(myPosition.X);
						byte[] ArrayY = BitConverter.GetBytes(myPosition.Y);
						byte[] DirectionX = BitConverter.GetBytes(myDirection.X);
						byte[] DirectionY = BitConverter.GetBytes(myDirection.Y);
						byte[] ShootDirX = BitConverter.GetBytes(myShootingDirection.X);
						byte[] ShootDirY = BitConverter.GetBytes(myShootingDirection.Y);
					
						action.CopyTo(sendBuffer,0);
						ArrayX.CopyTo(sendBuffer, action.Length);
						ArrayY.CopyTo(sendBuffer, action.Length + ArrayX.Length);
						DirectionX.CopyTo(sendBuffer, action.Length + ArrayX.Length+ ArrayY.Length);
						DirectionY.CopyTo(sendBuffer, action.Length + ArrayX.Length+ ArrayY.Length + DirectionX.Length);
						ShootDirX.CopyTo(sendBuffer, action.Length + ArrayX.Length+ ArrayY.Length + DirectionX.Length + DirectionY.Length);
						ShootDirY.CopyTo(sendBuffer, action.Length + ArrayX.Length+ ArrayY.Length + DirectionX.Length + DirectionY.Length +ShootDirX.Length );
					
						if (isServer)
						{
							if (clientSocket == null || IsConnect == false)
							{
								return false;
							}
							if(!actionMsg.Equals('I'))
								clientSocket.BeginSend(sendBuffer, 0, sendBuffer.Length, 0, new AsyncCallback(SocketEventCallback.SendCallback), this);
							
							clientSocket.BeginReceive(recvBuffer, 0, recvBuffer.Length, 0, new AsyncCallback(SocketEventCallback.ReceiveCallback), this);
							
						}
						else
						{
							if (socket == null || IsConnect == false){
								return false;
							}
							if(!actionMsg.Equals('I'))
								socket.BeginSend(sendBuffer, 0, sendBuffer.Length, 0, new AsyncCallback(SocketEventCallback.SendCallback), this);
							
						socket.BeginReceive(recvBuffer, 0, recvBuffer.Length, 0, new AsyncCallback(SocketEventCallback.ReceiveCallback), this);
							//Console.WriteLine("SENT AND RECIVED");
						}
					
				}
				finally
				{
					leaveCriticalSection();
				}
			}
			catch(System.Net.Sockets.SocketException e)
			{
				if (e.SocketErrorCode == SocketError.ConnectionReset || e.SocketErrorCode == SocketError.ConnectionAborted){
					//Console.WriteLine("DataExchange 切断検出");
					Disconnect();
				}
				//Console.WriteLine("ExchangeError " + e.ToString());
			}
			return true;
		}


		/***
		 * Accept
		 */
		public void OnAccept(IAsyncResult AsyncResult)
		{
			try
			{
				try
				{
					enterCriticalSection();
					if (Socket != null){
						ClientSocket = Socket.EndAccept(AsyncResult);
						//Console.WriteLine("Accept " + ClientSocket.RemoteEndPoint.ToString());
						IsConnect = true;
					}
				}
				finally
				{
					leaveCriticalSection();
				}
			}
			catch (Exception e)
			{
				//Console.WriteLine(e.ToString());
			}
			//Console.WriteLine("OnAccept");
			testStatus = "OnAccept";
		}
		/***
		 * Connect
		 */
		public void OnConnect(IAsyncResult AsyncResult)
		{
			try
			{
				try
				{
					enterCriticalSection();
					if (Socket != null){
						// Complete the connection.
						Socket.EndConnect(AsyncResult);
						//Console.WriteLine("Connect " + Socket.RemoteEndPoint.ToString());
						IsConnect = true;
					}
				}
				finally
				{
					leaveCriticalSection();
				}
			}
			catch (System.Net.Sockets.SocketException e)
			{
				if (e.SocketErrorCode == SocketError.ConnectionRefused){
					Disconnect();
				}
			}
			catch (Exception e)
			{
				//Console.WriteLine(e.ToString());
			}
			//Console.WriteLine("OnConnect");
			testStatus = "OnConnect";
		}
		
		/**
		 * Receive
		 */
		public void OnReceive(IAsyncResult AsyncResult)
		{
			int Len = 0;
			try
			{
				try
				{
					enterCriticalSection();
					if (IsServer){
						if (ClientSocket != null){
							Len = ClientSocket.EndReceive(AsyncResult);
							// 切断
							if (Len <= 0){
								//Disconnect();
							}
							else{
								
								
								////////////////////////////////////////////////////////////////////TODO/////////////////////////////////////////
								
								
								char action = BitConverter.ToChar(recvBuffer,0);
								networkActionMsg = action;
								if(action.Equals('S'))
									hasShot = true;
							
								else if(action.Equals('M'))
								{
									networkPosition.X = BitConverter.ToSingle(recvBuffer, 2);
									networkPosition.Y = BitConverter.ToSingle(recvBuffer, 6);
								}
								
								networkDirection.X = BitConverter.ToSingle(recvBuffer,10);
								networkDirection.Y = BitConverter.ToSingle(recvBuffer,14);
								
								networkShootDir.X = BitConverter.ToSingle(recvBuffer,18);
								networkShootDir.Y = BitConverter.ToSingle(recvBuffer,22);
								//Console.WriteLine ("RECIEVED = " + networkPosition.X + " : " + networkPosition.Y);
//								if(networkPosition.X == 0 && networkPosition.Y == 0)
//								{
//									isConnect = true;
//								}
								//Console.WriteLine("Host: OnReceive");
								testStatus = "Host: OnReceive";
							}
						}
					}
					else{
						if (Socket != null){
							Len = Socket.EndReceive(AsyncResult);
							// 切断
							if (Len <= 0){
								//Disconnect();
							}
							else{
								
								char action = BitConverter.ToChar(recvBuffer,0);
								networkActionMsg = action;
								if(action.Equals('S'))
									hasShot = true;
							
								else if(action.Equals('M'))
								{
									networkPosition.X = BitConverter.ToSingle(recvBuffer, 2);
									networkPosition.Y = BitConverter.ToSingle(recvBuffer, 6);
								}
								
								networkDirection.X = BitConverter.ToSingle(recvBuffer,10);
								networkDirection.Y = BitConverter.ToSingle(recvBuffer,14);
								
								networkShootDir.X = BitConverter.ToSingle(recvBuffer,18);
								networkShootDir.Y = BitConverter.ToSingle(recvBuffer,22);
//								networkPosition.X = BitConverter.ToSingle(recvBuffer, 0);
//								networkPosition.Y = BitConverter.ToSingle(recvBuffer, 4);
								//Console.WriteLine ("RECIEVED = " + networkPosition.X + " : " + networkPosition.Y);
								if(networkPosition.X == 0 && networkPosition.Y == 0)
								{
									isConnect = true;
								}
								//Console.WriteLine("Client: OnReceive");
								
							}
						}
					}
				}
				finally
				{
					leaveCriticalSection();
				}
			}
			catch (System.Net.Sockets.SocketException e)
			{
				if (e.SocketErrorCode == SocketError.ConnectionReset || e.SocketErrorCode == SocketError.ConnectionAborted){
					//Console.WriteLine("ReceiveCallback 切断検出");
					Disconnect();
				}
			}
			catch (Exception e)
			{
				//Console.WriteLine(e.ToString());
			}
			//Console.WriteLine("OnReceive");
		}
		
		/**
		 * Send
		 */
		public void OnSend(IAsyncResult AsyncResult)
		{
			int Len = 0;
//			int a = 0;
			try
			{
				try
				{
					enterCriticalSection();
					if (IsServer){
						if (ClientSocket != null){
							Len = ClientSocket.EndSend(AsyncResult);
						}
					}
					else{
						if (Socket != null){
							Len = Socket.EndSend(AsyncResult);
						}
					}
                    // Disconnection detection should go here...
					if (Len <= 0){
						// send error
					}
				}
				finally
				{
					leaveCriticalSection();
				}
			}
			catch (System.Net.Sockets.SocketException e)
			{
				if (e.SocketErrorCode == SocketError.ConnectionReset || e.SocketErrorCode == SocketError.ConnectionAborted){
					//Console.WriteLine("SendCallback 切断検出");
					Disconnect();
				}
			}
			catch (Exception e)
			{
				//Console.WriteLine(e.ToString());/
			}
			//Console.WriteLine("OnSend");
		}
	}
}