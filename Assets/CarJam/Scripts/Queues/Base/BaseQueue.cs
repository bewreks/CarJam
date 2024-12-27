using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
namespace CarJam.Scripts.Queues.Base
{
    public abstract class BaseQueue<T> : IDisposable
    {
        protected readonly Vector3 _startPoint;
        protected readonly Vector3 _finishPoint;
        
        protected Vector3 _currentPosition;
        protected Vector3 _queueDirection;
        protected float DistanceBetweenVehicles;
        
        protected List<T> _objects = new List<T>();

        public abstract bool IsCanDequeue { get; }
        public abstract bool UpdateInProgress
        {
            get;
            protected set;
        }
        
        public bool IsHaveEnoughSpace => Vector3.Distance(_currentPosition, _startPoint) > DistanceBetweenVehicles;

        protected BaseQueue(Vector3 startPoint, Vector3 finishPoint, float distanceBetweenVehicles)
        {
            DistanceBetweenVehicles = distanceBetweenVehicles;
            _startPoint = startPoint;
            _finishPoint = finishPoint;
            
            _currentPosition = _finishPoint;
            _queueDirection = DistanceBetweenVehicles * (_startPoint - _finishPoint).normalized;
        }

        public async UniTask Enqueue(T t)
        {
            await BeforeEnqueue(t, _startPoint);
            _objects.Add(t);
            var nextPosition = _currentPosition;
            _currentPosition = nextPosition + _queueDirection;
            await AfterEnqueue(t, nextPosition);
        }

        public T Dequeue()
        {
            var obj = _objects[0];
            _objects.RemoveAt(0);
            return obj;
        }

        public abstract UniTask UpdateQueue();
        protected abstract UniTask BeforeEnqueue(T t, Vector3 position);
        protected abstract UniTask AfterEnqueue(T t, Vector3 position);
        public abstract void Dispose();
    }
}
