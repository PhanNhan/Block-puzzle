using System.Collections.Generic;
using UnityEngine;

namespace ZPool
{
    public class ObjectPool : MonoBehaviour
    {
        private Poolable _prefab;
        private List<Poolable> _availableObjects = new List<Poolable>();

        public static ObjectPool GetPool(Poolable prefab)
        {
            GameObject obj;
            ObjectPool pool;
            if (Application.isEditor)
            {
                obj = GameObject.Find(prefab.name + " Pool");
                if (obj)
                {
                    pool = obj.GetComponent<ObjectPool>();
                    if (pool)
                    {
                        return pool;
                    }
                }
            }
            obj = new GameObject(prefab.name + " Pool");
            pool = obj.AddComponent<ObjectPool>();
            pool._prefab = prefab;
            return pool;
        }

        public Poolable GetObject()
        {
            Poolable obj;
            int lastAvailableIndex = _availableObjects.Count - 1;
            if (lastAvailableIndex >= 0)
            {
                obj = _availableObjects[lastAvailableIndex];
                _availableObjects.RemoveAt(lastAvailableIndex);
                obj.gameObject.SetActive(true);
            }
            else
            {
                obj = Instantiate<Poolable>(_prefab);
                obj.transform.SetParent(transform, false);
                obj.Pool = this;
            }
            var poolListeners = obj.GetComponents<IPoolListener>();
            if (poolListeners != null && poolListeners.Length > 0)
            {
                for (int i = 0, c = poolListeners.Length; i < c; ++i)
                {
                    poolListeners[i].OnSpawn();
                }
            }
            return obj;
        }

        public void AddObject(Poolable obj)
        {
            var poolListeners = obj.gameObject.GetComponents<IPoolListener>();
            if (poolListeners != null && poolListeners.Length > 0)
            {
                for (int i = 0, c = poolListeners.Length; i < c; ++i)
                {
                    poolListeners[i].OnRecycle();
                }
            }
            if (!obj.DontReparent)
            {
                obj.transform.SetParent(transform, false);
            }
            obj.gameObject.SetActive(false);
            if (!_availableObjects.Contains(obj))
            {
                _availableObjects.Add(obj);
            }
        }
    }
}
