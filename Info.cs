using System;

namespace TheATeam
{
	public static class Info
	{
		private static Random rnd = new Random();
		
		public static Random Rnd { get { return rnd; } }
		
		public static bool IsGameOver { get; set; }
		
		public static Player P1 { get; set;}
		
		public static Player P2 { get; set; }
	}
}
