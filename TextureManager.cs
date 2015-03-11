using Sce.PlayStation.Core.Graphics;
using Sce.PlayStation.HighLevel.GameEngine2D.Base;

namespace TheATeam
{
	public class TextureManager: AssetManager<TextureInfo>
	{
		private const string BASE_PATH = "/Application/assets/";
		
		new public static bool RemoveAsset(string key)
		{
			if(IsAssetLoaded(key))
			{
				resourceMap[key].Dispose();
				AssetManager<TextureInfo>.RemoveAsset(key);
			}
			
			return !IsAssetLoaded(key);
		}

		new public static bool AddAsset(string key, TextureInfo asset)
		{
			if(!IsAssetLoaded(key))
			{
				AssetManager<TextureInfo>.AddAsset(key, asset);
			}
			bool loaded = IsAssetLoaded(key);
			if(loaded)
			{
				resourceMap[key].Texture.SetFilter(Sce.PlayStation.Core.Graphics.TextureFilterMode.Disabled);
			}
			
			return loaded;
		}
		
		public static bool AddAsset(string key, string filename)
		{
			return AddAsset(key, new TextureInfo(BASE_PATH + filename));
		}
		
		public static bool AddAsset(string key, string filename, Vector2i numTiles)
		{
			Texture2D t = new Texture2D(BASE_PATH + filename, true);
			return AddAsset(key, new TextureInfo(t, numTiles));
		}
		
		new public static TextureInfo Get(string key)
		{
			if(resourceMap.Count <= 0)
			{
				if(Initialise())
				{
					return resourceMap[key];
				}
			}
			return resourceMap[key];
		}
		
		public static void Dispose()
		{
			foreach(var k in resourceMap.Keys)
			{
				RemoveAsset(k);
			}
		}
		
		public static bool Initialise()
		{
			// Load and store textures
			AddAsset("background", "Background.png");
			AddAsset("hudbar", "HUDBar.png");
			AddAsset("base", "base.png");
			AddAsset("blockedArea", "BlockedArea.png");
			AddAsset("health", "health.png");
			AddAsset("items", "ItemSpriteSheet.png", new Vector2i(1, 6));
			AddAsset("mana", "mana.png");
			AddAsset("players", "PlayerSpriteSheet.png", new Vector2i(4,6));
			AddAsset("pointer", "pointer.png");
			AddAsset("tiles", "SpriteSheetMaster-Recovered.png", new Vector2i(4, 8));
			return true;
		}
	}
}

