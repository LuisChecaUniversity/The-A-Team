using System;
using Sce.PlayStation.Core;
using Sce.PlayStation.HighLevel.GameEngine2D;
using Sce.PlayStation.HighLevel.GameEngine2D.Base;

namespace TheATeam
{
	public class Level: Scene
	{
		public Level(): base()
		{
			Info.LevelClear = false;
			Vector2 cameraCenter = Vector2.Zero;
			//AddChild(new TextureInfo("/Application/assets/Background.png"));
			Tile.Loader("/Application/assets/level3.txt", ref cameraCenter, this);
			Info.CameraCenter = cameraCenter;
			
			Camera2D.SetViewFromViewport();
			Schedule((dt) => {
				Info.TotalGameTime += dt;
				// Camera2D.SetViewFromHeightAndCenter(Info.CameraHeight, Info.CameraCenter);
			});
		}
	}
}



