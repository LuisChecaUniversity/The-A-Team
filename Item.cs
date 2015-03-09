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
		public ItemType Type { get; private set; }
		public string Name { get; private set; }
		public SpriteTile iSprite;
		public Vector2 position;
		private Vector2 initialPosition;
		public bool collided;
		
		public Item (Scene scene, Vector2 pos, Vector2i spriteIndex2D, ItemType type, string name)
		{
			iSprite = new SpriteTile();
			iSprite.TextureInfo = TextureManager.Get("items");
			iSprite.Quad.S = iSprite.TextureInfo.TileSizeInPixelsf;
			iSprite.TileIndex2D = spriteIndex2D;
			iSprite.Position = pos;
			iSprite.CenterSprite();
			
			initialPosition = position = pos;
			Type = type;
			Name = name;
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
			iSprite.Position = position;
		}
		public void ResetFlag()
		{
			if(position != initialPosition && Type == ItemType.flag)
			{
				position = initialPosition;
			}
		}
		
		public bool hasCollided(Vector2 objectPosition, Vector2 objectSize)
		{
			// Collision for objects that are centred
			
			Bounds2 iBounds = iSprite.Quad.Bounds2();
			float iWidth = iBounds.Point11.X;
			float iHeight = iBounds.Point11.Y;
			
			float objectWidth = objectSize.X;
			float objectHeight = objectSize.Y;
//			
//			if((position.X) < objectPosition.X - objectWidth)
//				return false;
//			else if(position.X - iWidth > (objectPosition.X ))
//				return false;
//			else if((position.Y) < objectPosition.Y - objectHeight)
//				return false;
//			else if(position.Y - iHeight > (objectPosition.Y ))
//				return false;
//			else 
//				return true;
			
			if(position.X - iWidth> objectPosition.X + objectWidth)
				return false;
			else if(position.X + iHeight < objectPosition.X )
				return false;
			else if(position.Y - iWidth > objectPosition.Y + objectHeight)
				return false;
			else if(position.Y + iHeight < objectPosition.Y )
				return false;
			else 
				return true;
			
		}	
	}
	

	
	
	
	
	
	
	
}

