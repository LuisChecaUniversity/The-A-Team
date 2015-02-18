using Sce.PlayStation.Core.Graphics;
using Sce.PlayStation.HighLevel.GameEngine2D.Base;

namespace TheATeam
{
	public class TextureManager: AssetManager<TextureInfo>
	{
		new public static void RemoveAsset(string key)
		{
			if(!IsAssetLoaded(key))
				return;
			resourceMap[key].Dispose();
			AssetManager<TextureInfo>.RemoveAsset(key);
		}

		new public static void AddAsset(string key, TextureInfo asset)
		{
			AssetManager<TextureInfo>.AddAsset(key, asset);
			if(IsAssetLoaded(key))
				resourceMap[key].Texture.SetFilter(Sce.PlayStation.Core.Graphics.TextureFilterMode.Disabled);
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
			TextureManager.AddAsset("tiles", new TextureInfo(new Texture2D("/Application/assets/SpriteSheetMaster-Recovered.png", false),
	                                                 new Vector2i(4, 8)));
			TextureManager.AddAsset("background", new TextureInfo("/Application/assets/Background.png"));
			return true;
		}
	}
}

