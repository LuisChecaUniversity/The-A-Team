using System;
using Sce.PlayStation.Core;
using Sce.PlayStation.HighLevel.GameEngine2D;
using Sce.PlayStation.HighLevel.GameEngine2D.Base;

namespace TheATeam
{
	public class Level: Scene
	{
		Player player1;
		Player player2;
		
		public Level(): base()
		{
			Info.LevelClear = false;
			Vector2 cameraCenter = Vector2.Zero;
			//AddChild(new TextureInfo("/Application/assets/Background.png"));
			Tile.Loader("/Application/assets/level1.txt", ref cameraCenter, this);
			Info.CameraCenter = cameraCenter;
			
			player1 = new Player(new Vector2(64,300),true);
			player2 = new Player(new Vector2(960 - 64,300),false);
			
			this.AddChild(player1);
			this.AddChild(player2);
			Camera2D.SetViewFromViewport();
//			Schedule((dt) => {
//				Info.TotalGameTime += dt;
//				// Camera2D.SetViewFromHeightAndCenter(Info.CameraHeight, Info.CameraCenter);
//			});
		}
		
		public override void Update (float dt)
		{
			base.Update (dt);
			
//			if(AppMain.ISHOST)
//			{
//				player1.Update(dt);
//				AppMain.client.SetMyPosition(player1.Position.X,player1.Position.Y) ;
//				AppMain.client.DataExchange();
//				player2.Position = AppMain.client.networkPosition;
//			}
//			else
//			{
//				player2.Update(dt);
//				AppMain.client.SetMyPosition(player2.Position.X,player2.Position.Y) ;
//				AppMain.client.DataExchange();
//				player1.Position = AppMain.client.networkPosition;
//			}
			
			
		}
	}
}



