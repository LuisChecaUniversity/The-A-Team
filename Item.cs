using System;
using Sce.PlayStation.Core;
using Sce.PlayStation.Core.Graphics;

using Sce.PlayStation.HighLevel.GameEngine2D;
using Sce.PlayStation.HighLevel.GameEngine2D.Base;

namespace TheATeam
{

	//public class Item
	//{
	//	private SpriteUV	sprite;
	//	private TextureInfo	textureInfoFlag;
	//	
	//	public Rectangle Extents
	//	{
	//		get {return new Rectangle(X + 32, Y + 32, 32, 32);} //done so player must be over the flag/ touch the middle
	//	}
	//	
	//	public float X {get { return sprite.Position.X;}}
	//	public float Y {get { return sprite.Position.Y;}}
	//	
	//	public Item (Scene scene)
	//	{
	//		textureInfo = new TextureInfo("/Application/assets/FlagTemp.png");
	//		sprite = new SpriteUV(textureFlag);	
	//		sprite.Quad.S = textureInfo.TextureSizef;
	//		sprite.Position = new Vector2(50.0f,(Director.Instance.GL.Context.GetViewport().Height*0.5f)-30.0f);
	//		sprite.Visible = true;
	//		scene.AddChild (sprite);
	//	}
	//	
	//	public void Grabbed()
	//	{
	//		//if the player collides with item, item position(x,y,z) = player position(x,y,z)
	//		//when the player dies the item lets go because no more collision
	//		
	//		//preferably:
	//		//if collided
	//		//item sprite.Visible = false 
	//		//player sprite switch to new sprite (holding item)
	//		//if player dies
	//		//sprite position = players last position...
	//	}
	//}
	

	public class Item
	{
		public TextureInfo iTex;
		public SpriteUV iSprite;
		public Vector2 position;
		public bool collided;
		
		public Item (Scene scene, Vector2 pos)
		{
			iTex = new TextureInfo("/Application/Assets/FlagTemp.png");
			iSprite = new SpriteUV(iTex);
			iSprite.Quad.S = iTex.TextureSizef;	
			iSprite.CenterSprite();
			
			position = pos;
			collided = false;
			iSprite.Visible = true;
			scene.AddChild(iSprite);
		}
		
		public void Dispose()
		{
			Director.Instance.CurrentScene.RemoveChild(iSprite, true);
			iSprite.RegisterDisposeOnExitRecursive();			
		}
		
		public void Update(float dt)
		{
			position = new Vector2(position.X, position.Y);
			iSprite.Position = position;
		}
		
		public bool hasCollided(Vector2 objectPosition, Vector2 objectSize)
		{
			// Collision for objects that are centred
			
			Bounds2 iBounds = iSprite.Quad.Bounds2();
			float iWidth = iBounds.Point11.X;
			float iHeight = iBounds.Point11.Y;
			
			float objectWidth = objectSize.X;
			float objectHeight = objectSize.Y;
			
			if((position.X ) < objectPosition.X - objectWidth)
				return false;
			else if(position.X - iWidth > (objectPosition.X ))
				return false;
			else if((position.Y) < objectPosition.Y - objectHeight)
				return false;
			else if(position.Y - iHeight > (objectPosition.Y ))
				return false;
			else 
				return true;
			
		}
	//	public bool offScreen()
	//	{
	//		float width = Director.Instance.GL.Context.GetViewport().Width;
	//		float height = Director.Instance.GL.Context.GetViewport().Height;
	//		if(position.X > width)
	//			return true;
	//		else if(position.X < 0.0f)
	//			return true;
	//		else if(position.Y < 0.0f)
	//			return true;
	//		else if(position.Y  > height)
	//			return true;
	//		else 
	//			return false;
	//	}
	}
	
	
	
	
	
	
	
	
	
}

