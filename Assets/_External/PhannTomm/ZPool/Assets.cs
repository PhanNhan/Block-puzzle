using System.Collections.Generic;
using UnityEngine;

namespace ZPool
{
    public static class Assets
    {
        private static Dictionary<string, Object> _resourceCache = new Dictionary<string, Object>();

        public static T GetAsset<T>(string assetName) where T : Object
        {
            if (!_resourceCache.ContainsKey(assetName))
                _resourceCache[assetName] = Resources.Load<T>(assetName);
            return (T)_resourceCache[assetName];
        }

        public static void ClearAllCacheAssets()
        {
            Resources.UnloadUnusedAssets();
            _resourceCache.Clear();
        }

        public static T Clone<T>(T obj) where T : Object
        {
            if (obj is GameObject)
            {
                var gameObj = obj as GameObject;
                var poolable = gameObj.GetComponent<Poolable>();
                if (poolable != null)
                {
                    var t = poolable.GetPooledInstance<Poolable>();
                    return t.gameObject as T;
                }
                return Object.Instantiate(obj);
            }
            return Object.Instantiate(obj);
        }

		public static void PreCreateObject<T>(T obj, int number) where T : Object
		{
			for (int i = 0; i < number; i++) {
				if (obj is GameObject) {
					var gameObj = obj as GameObject;
					var poolable = gameObj.GetComponent<Poolable> ();
					if (poolable != null) {
						var t = poolable.GetPooledInstance<Poolable> ();
						t.ReturnToPool ();
					}
				}
			}
		}

        public static T Instantiate<T>(string assetName) where T : Object
        {
            return Clone(GetAsset<T>(assetName));
        }

        public static void Destroy(Object obj)
        {
            bool needCallDestroy = true;

            if (obj is GameObject)
            {
                var gameObj = obj as GameObject;
                var poolable = gameObj.GetComponent<Poolable>();
                if (poolable != null)
                {
                    poolable.ReturnToPool();
                    needCallDestroy = false;
                }
            }

            if (needCallDestroy)
            {
                Object.Destroy(obj);
            }
        }

        public static void Destroy(Object obj, float delay)
        {
            Object.Destroy(obj, delay);
        }
    }
}
