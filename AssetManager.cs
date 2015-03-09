using System.Collections.Generic;

namespace TheATeam
{
	public abstract class AssetManager<T>
	{
		protected static Dictionary<string, T> resourceMap = new Dictionary<string, T>();
		
		public static bool AddAsset(string key, T asset)
		{
			if(!IsAssetLoaded(key))
			{
				resourceMap.Add(key, asset);
			}	
			return IsAssetLoaded(key);
		}
		
		public static bool RemoveAsset(string key)
		{
			if(IsAssetLoaded(key))
			{
				resourceMap.Remove(key);
			}
			
			return !IsAssetLoaded(key);
		}
		
		public static bool IsAssetLoaded(string key)
		{
			return resourceMap.ContainsKey(key);	
		}
		
		public static T Get(string key)
		{
			return resourceMap[key];
		}
	}
}

