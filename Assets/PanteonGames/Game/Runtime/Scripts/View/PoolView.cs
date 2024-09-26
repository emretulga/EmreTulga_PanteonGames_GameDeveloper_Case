using System;
using System.Collections.Generic;
using UnityEngine;

namespace PanteonGames.Game.Runtime.View
{
    [Serializable]
    class PoolObjectSettings
    {
        public string Key;
        public GameObject prefab;
        public int Amount;
    }

    public class PoolView : MonoBehaviour
    {
        public static PoolView Instance;
        [SerializeField]
        private List<PoolObjectSettings> _poolObjects;
        private Dictionary<string, List<PoolObject>> _pool;

        private void Awake()
        {
            Instance = this;
            _pool = new();

            foreach(PoolObjectSettings poolObject in _poolObjects)
            {
                string key = poolObject.Key;
                GameObject prefab = poolObject.prefab;
                int objectAmount = poolObject.Amount;
                List<PoolObject> newList = new();
                _pool.Add(key, newList);

                for(int objectIndex = 0; objectIndex < objectAmount; objectIndex++)
                {
                    GameObject createdObject = Instantiate(prefab, prefab.transform.position, Quaternion.identity);
                    PoolObject newPoolObject = createdObject.GetComponent<PoolObject>();
                    newList.Add(newPoolObject);
                    SendObjectToPool(newPoolObject);
                }
            }
        }

        public PoolObject PullObjectFromPool(string key)
        {
            PoolObject resultObject = null;

            foreach(PoolObject poolObject in _pool[key])
            {
                if(poolObject.IsInPool)
                {
                    poolObject.OnPulledFromPool();
                    resultObject = poolObject;
                    break;
                }
            }

            return resultObject;
        }

        public void SendObjectToPool(PoolObject poolObject)
        {
            poolObject.OnPooled();
        }


    }
}