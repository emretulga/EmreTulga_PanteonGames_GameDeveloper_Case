using UnityEngine;

namespace PanteonGames.Game.Runtime.View
{
    public class PoolObject : MonoBehaviour
    {
        public bool IsInPool { get; protected set; }
        
        public virtual void OnPooled()
        {
            IsInPool = true;
            transform.gameObject.SetActive(false);
        }

        public virtual void OnPulledFromPool()
        {
            IsInPool = false;
            transform.gameObject.SetActive(true);
        }
    }
}