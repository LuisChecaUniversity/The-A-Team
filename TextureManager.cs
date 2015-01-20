using System;
using Sce.PlayStation.HighLevel.GameEngine2D.Base;

namespace PairedGame
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
		
		public static void Dispose()
		{
			foreach(var k in resourceMap.Keys)
			{
				RemoveAsset(k);
			}
		}
	}
}

