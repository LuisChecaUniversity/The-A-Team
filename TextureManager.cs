using Sce.PlayStation.Core.Graphics;
using Sce.PlayStation.HighLevel.GameEngine2D.Base;

namespace TheATeam
{
	public class TextureManager: AssetManager<TextureInfo>
	{
		new public static bool Add(string key, TextureInfo asset)
		{
			if(!IsAssetLoaded(key))
			{
				AssetManager<TextureInfo>.Add(key, asset);
			}
			bool loaded = IsAssetLoaded(key);
			if(loaded)
			{
				resourceMap[key].Texture.SetFilter(Sce.PlayStation.Core.Graphics.TextureFilterMode.Disabled);
			}
			
			return loaded;
		}
		
		public static bool Add(string key, string filename, Vector2i numTiles)
		{
			Texture2D t = new Texture2D(BASE_PATH + filename, true);
			return Add(key, new TextureInfo(t, numTiles));
		}
		
		new public static TextureInfo Get(string key)
		{
			if(resourceMap.Count <= 0 || !IsAssetLoaded(key))
			{
				if(Initialise() && IsAssetLoaded(key))
				{
					return resourceMap[key];
				}
				return default(TextureInfo);
			}
			return resourceMap[key];
		}
		
		public static bool Initialise()
		{
			// Load and store textures
			Add("background", "Background.png");
			Add("hudbar", "HUDBar.png");
			Add("base", "Base.png");
			Add("blockedArea", "BlockedArea.png");
			Add("health", "health.png");
			Add("shieldhp", "shieldhp.png");
			Add("items", "ItemSpriteSheet.png", new Vector2i(1, 6));
			Add("mana", "mana.png");
			Add("players", "PlayerSpriteSheet+.png", new Vector2i(4,6));
			Add("pointer", "pointer.png");
			Add("tiles", "WallSpriteSheet.png", new Vector2i(4, 7));
			Add("shields", "ShieldSpriteSheet.png", new Vector2i(1, 3));
			Add("rings", "RingSpriteSheet.png", new Vector2i(1, 6));
			return true;
		}
	}
}

