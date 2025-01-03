﻿using System;
using System.Linq;
using System.Threading;
using CarJam.Scripts.Queues.Base;
using CarJam.Scripts.Queues.BusStop.Presenters;
using Cysharp.Threading.Tasks;
using UnityEngine;
namespace CarJam.Scripts.Queues.BusStop
{
    public class BusStopQueue : BaseQueue<BusStopPlacePresenter>
    {

        public BusStopQueue(Vector3 startPoint, Vector3 finishPoint, float distanceBetweenVehicles) : base(startPoint, finishPoint, distanceBetweenVehicles)
        {
        }

        public override bool IsCanDequeue => false;
        public override bool UpdateInProgress
        {
            get;
            protected set;
        }
        
        public int Count => _objects.Count(presenter => presenter.IsEmpty);

        public override UniTask UpdateQueue(CancellationToken token)
        {
            return UniTask.CompletedTask; 
        }

        protected override UniTask BeforeEnqueue(BusStopPlacePresenter t, Vector3 position, CancellationToken token)
        {
            return UniTask.CompletedTask; 
        }

        protected override UniTask AfterEnqueue(BusStopPlacePresenter t, Vector3 position, CancellationToken token)
        {
            t.PlaceToPosition(position);
            return UniTask.CompletedTask;
        }

        public override void Dispose()
        {
            foreach (var presenter in _objects)
            {
                presenter.Dispose();
            }
        }

        public BusStopPlacePresenter GetEmptyPlace()
        {
            return _objects.FirstOrDefault(presenter => presenter.IsEmpty);
        }

        public BusStopPlacePresenter GetPlace(Guid id)
        {
            return _objects.First(presenter => presenter.Id == id);
        }
    }
}
