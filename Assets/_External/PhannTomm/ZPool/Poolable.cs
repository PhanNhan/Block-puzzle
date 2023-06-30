using System.Collections.Generic;
using UnityEngine;

namespace ZPool
{
    public class Poolable : MonoBehaviour
    {
        [System.NonSerialized]
        ObjectPool _poolInstanceForPrefab;

        public bool DontReparent = false;

        public T GetPooledInstance<T>() where T : Poolable
        {
            if (!_poolInstanceForPrefab)
            {
                _poolInstanceForPrefab = ObjectPool.GetPool(this);
            }
            return (T)_poolInstanceForPrefab.GetObject();
        }

        public ObjectPool Pool { get; set; }

        public void ReturnToPool()
        {
            if (Pool)
            {
                Pool.AddObject(this);
            }
            else
            {
                Debug.LogWarning("XXXXWarrning: Object added poolable component, but cannot pool " + this.gameObject.name);
                Destroy(gameObject);
            }
        }
    }
}
