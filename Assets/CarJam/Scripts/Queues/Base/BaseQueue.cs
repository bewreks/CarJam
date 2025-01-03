﻿using System;
using System.Collections.Generic;
using System.Threading;
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
        
        public T First => _objects.Count > 0 ? _objects[0] : default;
        
        public bool IsHaveEnoughSpace => Vector3.Distance(_currentPosition, _startPoint) > DistanceBetweenVehicles;

        protected BaseQueue(Vector3 startPoint, Vector3 finishPoint, float distanceBetweenVehicles)
        {
            DistanceBetweenVehicles = distanceBetweenVehicles;
            _startPoint = startPoint;
            _finishPoint = finishPoint;
            
            _currentPosition = _finishPoint;
            _queueDirection = DistanceBetweenVehicles * (_startPoint - _finishPoint).normalized;
        }

        public async UniTask Enqueue(T t, CancellationToken token)
        {
            var nextPosition = _currentPosition;
            _currentPosition = nextPosition + _queueDirection;
            await BeforeEnqueue(t, _startPoint, token);
            if (token.IsCancellationRequested) return;
            _objects.Add(t);
            await AfterEnqueue(t, nextPosition, token);
        }

        public T Dequeue()
        {
            var obj = _objects[0];
            _objects.RemoveAt(0);
            return obj;
        }

        public void Clear()
        {
            _objects.Clear();
            _currentPosition = _finishPoint;
        }

        public abstract UniTask UpdateQueue(CancellationToken token);
        protected abstract UniTask BeforeEnqueue(T t, Vector3 position, CancellationToken token);
        protected abstract UniTask AfterEnqueue(T t, Vector3 position, CancellationToken token);
        public abstract void Dispose();
    }
}
