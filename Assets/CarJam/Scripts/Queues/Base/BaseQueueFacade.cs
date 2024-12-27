using System;
using CarJam.Scripts.CarJam;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Zenject;
namespace CarJam.Scripts.Queues
{
    public abstract class BaseQueueFacade<T, TQueue> : IInitializable, IDisposable
        where TQueue : BaseQueue<T>
    {
        protected readonly Vector3 _startPoint;
        protected readonly Vector3 _finishPoint;
        protected readonly Vector3 _spawnPoint;
        
        protected TQueue _queue;
        
        protected abstract float DistanceBetweenObject { get; }
        
        public BaseQueueFacade(Vector3 startPoint, Vector3 finishPoint, Vector3 spawnPoint)
        {
            _startPoint = startPoint;
            _finishPoint = finishPoint;
            _spawnPoint = spawnPoint;
        }

        public void Initialize()
        {
            _queue = (TQueue) Activator.CreateInstance(typeof(TQueue), _startPoint, _finishPoint, DistanceBetweenObject);
        }
        
        public void Dispose()
        {
            _queue.Dispose();
        }

        public async UniTask Enqueue(GameColors color)
        {
            if (!_queue.IsHaveEnoughSpace ||
                _queue.UpdateInProgress) return;

            var character = TFactory(color);
            await _queue.Enqueue(character);
        }
        
        public void DequeueCharacter()
        {
            if (!_queue.IsCanDequeue ||
                _queue.UpdateInProgress) return;
            
            var character = _queue.Dequeue();
            OnDequeue(character);
            _queue.UpdateQueue().Forget();
        }

        protected abstract T TFactory(GameColors color);
        protected abstract void OnDequeue(T obj);
    }
}
