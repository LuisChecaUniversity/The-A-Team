using System;
using Sce.PlayStation.Core;
using Sce.PlayStation.HighLevel.GameEngine2D;
using Sce.PlayStation.HighLevel.GameEngine2D.Base;
using Sce.PlayStation.Core.Imaging;
using Sce.PlayStation.Core.Audio;
using Sce.PlayStation.Core.Environment;
using Sce.PlayStation.Core.Graphics;

namespace TheATeam
{
	public class Level: Scene
	{

		Player player1;
		Player player2;
		Label lblTopLeft;
		private Label lblTopRight;
		private Label lblBottomLeft;
		private Label lblBottomRight;
		private Label lblDebugLeft;
		private int screenWidth;
		private int screenHeight;
		Font font;
		FontMap debugFont;

		public Level(): base()
		{

			screenWidth = Director.Instance.GL.Context.Screen.Width;
			screenHeight = Director.Instance.GL.Context.Screen.Height;

			font = new Font(FontAlias.System, 25, FontStyle.Bold);
			debugFont = new FontMap(font, 25);

			// Reload the font becuase FontMap disposes of it
			font = new Font(FontAlias.System, 25, FontStyle.Bold);
			Info.LevelClear = false;
			Vector2 cameraCenter = Vector2.Zero;


            AddChild(new Background());

			Tile.Loader("/Application/assets/level3.txt", ref cameraCenter, this);
			Info.CameraCenter = cameraCenter;

			player1 = new Player(cameraCenter, true);
			player2 = new Player(new Vector2(960 - 164, 300), false);

			if(AppMain.TYPEOFGAME.Equals("MULTIPLAYER"))
			{

				lblTopLeft = new Label();
				lblTopLeft.FontMap = debugFont;
				lblTopLeft.Text = "Player 1";
				lblTopLeft.Position = new Vector2(100, screenHeight - 200);

				lblTopRight = new Label();
				lblBottomLeft = new Label();
				lblBottomRight = new Label();
				lblDebugLeft = new Label();

				lblTopRight.FontMap = debugFont;
				lblTopRight.Text = "Player 2";
				lblTopRight.Position = new Vector2(screenWidth - 200, screenHeight - 200);

				lblBottomLeft.FontMap = debugFont;
				lblBottomLeft.Text = "Waiting";
				lblBottomLeft.Position = new Vector2(100, 300);

				lblBottomRight.FontMap = debugFont;
				lblBottomRight.Text = "Waiting";
				lblBottomRight.Position = new Vector2(screenWidth - 200, 300);

				lblDebugLeft.FontMap = debugFont;
				lblDebugLeft.Text = "Waiting for both connections";
				lblDebugLeft.Position = new Vector2(430, 200);

				this.AddChild(lblTopRight);
				this.AddChild(lblBottomLeft);
				this.AddChild(lblBottomRight);
				this.AddChild(lblDebugLeft);
				this.AddChild(lblTopLeft);
			}
			this.AddChild(player1);
			this.AddChild(player2);
			Camera2D.SetViewFromViewport();


//			Schedule((dt) => {
//				Info.TotalGameTime += dt;
//				// Camera2D.SetViewFromHeightAndCenter(Info.CameraHeight, Info.CameraCenter);
//			});
		}

		public override void Update(float dt)
		{
			base.Update(dt);

			if(AppMain.TYPEOFGAME.Equals("MULTIPLAYER"))
			{
				string status = AppMain.client.statusString;
				if(status.Equals("None"))
				{
					AppMain.client.ChangeStatus();
					lblDebugLeft.Text = "Changing";
				}
				else
					lblDebugLeft.Text = status;

				if(AppMain.ISHOST)
				{
					player1.Update(dt);
					AppMain.client.DataExchange();
					player2.Update(dt);
				}
				else
				{
					player2.Update(dt);
					AppMain.client.DataExchange();
					player1.Update(dt);
				}
			}
			else if(AppMain.TYPEOFGAME.Equals("SINGLE"))
			{
				player1.Update(dt);
				player2.UpdateAI(dt, player1);
			}


			// handle bullet update and collision
			ProjectileManager.Instance.Update(dt);

			if(ProjectileManager.Instance.ProjectileCollision(player1.Position, player1.Quad.Bounds2()))
				Console.WriteLine("Player 1 got hit");
			if(ProjectileManager.Instance.ProjectileCollision(player2.Position, player2.Quad.Bounds2()))
				Console.WriteLine("Player 2 got hit");


			foreach(Tile t in Tile.Collisions)
			{
				if(ProjectileManager.Instance.ProjectileCollision(t.Position, t.Quad.Bounds2()))
					Console.WriteLine("bullet hit tile"); // add tile.damage(); here **can hit more then 1 tile at a time**
			}

			ItemManager.Instance.Update(dt);
			ItemManager.Instance.ItemCollision(player1.Position, player1.Quad.Bounds2());

		}
	}
}



