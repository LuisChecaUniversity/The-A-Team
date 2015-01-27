using System;

namespace TheATeam
{
	public static class Info
	{
		private static Random rnd = new Random();
		
		public static double ItemSpawnRate { get; set; }
		
		public static float TotalGameTime { get; set; }
		
		public static Random Rnd { get { return rnd; } }
		
		public static float CameraHeight { get; set; }
		
		public static Sce.PlayStation.Core.Vector2 CameraCenter { get; set; }
		
		public static bool LevelClear { get; set; }

		public static int LevelNumber { get; set; }

		public static int MaxLevels { get { return 3; } }
	}
}
