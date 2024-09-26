using System.Collections.Generic;

namespace PanteonGames.Game.Runtime.Factory
{
    public interface IPool<T>
    {
        void PoolObject(T poolingInstance);
    }

    public abstract class Pool<T>: IPool<T>
    {
        protected readonly List<T> _pool = new ();
        protected readonly List<T> _pulledObjects = new ();
        
        private int _desiredInstanceAmount;

        public Pool(int instanceAmount)
        {
            _desiredInstanceAmount = instanceAmount;

            FillPoolWithNewInstances(instanceAmount);
        }

        protected abstract T CreateInstance();

        private void FillPoolWithNewInstances(int amount)
        {
            for(int objectIndex = 0; objectIndex < amount; objectIndex++)
            {
                _pool.Add(CreateInstance());
            }
        }

        private void ClearInstanceAmountOfPool(int amount)
        {
            _pool.RemoveRange(_pool.Count - amount, amount);
        }

        public T PullObject()
        {
            TryToFillPoolWithNewInstances();

            int lastIndexOfPool = _pool.Count - 1;
            var pulledObject = _pool[lastIndexOfPool];
            _pool.RemoveAt(lastIndexOfPool);
            _pulledObjects.Add(pulledObject);

            return pulledObject;
        }

        private void TryToFillPoolWithNewInstances()
        {
            bool isPoolEmpty = _pool.Count == 0;
            if(isPoolEmpty) FillPoolWithNewInstances(_pulledObjects.Count);
        }

        public void PoolObject(T poolingInstance)
        {
            bool isInstancePulled = _pulledObjects.Contains(poolingInstance);
            if(!isInstancePulled) return;
            
            _pulledObjects.Remove(poolingInstance);
            _pool.Add(poolingInstance);

            TryToHalvePool();
        }

        private void TryToHalvePool()
        {
            int totalInstanceAmount = _pool.Count + _pulledObjects.Count;
            if(_desiredInstanceAmount == totalInstanceAmount) return;

            int halfOfInstanceAmount = totalInstanceAmount / 2;
            if(_pool.Count < halfOfInstanceAmount) return;

            ClearInstanceAmountOfPool(halfOfInstanceAmount);
        }

    }
}