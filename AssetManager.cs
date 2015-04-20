using System.Collections.Generic;

namespace TheATeam
{
	public class AssetManager<T>
	{
		// Path where the files will be located
		protected const string BASE_PATH = "/Application/assets/";
		// The structure holding the resource objects
		protected static Dictionary<string, T> resourceMap = new Dictionary<string, T>();
		// Add a resource with a key
		public static bool Add(string key, T asset)
		{
			if(!IsAssetLoaded(key))
			{
				resourceMap.Add(key, asset);
			}
			return IsAssetLoaded(key);
		}
		// Load and add a resource with a key
		public static bool Add(string key, string filename)
		{
			bool isLoaded = IsAssetLoaded(key);
			if (!isLoaded)
			{
				return Add(key, (T)System.Activator.CreateInstance(typeof(T), BASE_PATH + filename));
			}
			return isLoaded;
		}
		
		public static void Dispose()
		{
			foreach(var k in resourceMap.Keys)
			{
				System.IDisposable disposable = resourceMap[k] as System.IDisposable;
				if (disposable != null)
				{
					disposable.Dispose();
				}
			}
			resourceMap.Clear();
		}
		
		public static T Get(string key)
		{
			if(IsAssetLoaded(key))
			{
				return resourceMap[key];
			}
			return default(T);
		}
		
		public static bool IsAssetLoaded(string key)
		{
			return resourceMap.ContainsKey(key);	
		}
		
		public static bool Remove(string key)
		{
			if(IsAssetLoaded(key))
			{
				System.IDisposable disposable = resourceMap[key] as System.IDisposable;
				if (disposable != null)
				{
					disposable.Dispose();
				}
				resourceMap.Remove(key);
			}
			
			return !IsAssetLoaded(key);
		}
	}
}

