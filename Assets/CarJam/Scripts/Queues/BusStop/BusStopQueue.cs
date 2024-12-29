using System;
using System.Linq;
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
        
        public override UniTask UpdateQueue()
        {
            return UniTask.CompletedTask; 
        }

        protected override UniTask BeforeEnqueue(BusStopPlacePresenter t, Vector3 position)
        {
            return UniTask.CompletedTask; 
        }

        protected override UniTask AfterEnqueue(BusStopPlacePresenter t, Vector3 position)
        {
            t.PlaceToPosition(position);
            return UniTask.CompletedTask;
        }

        public override void Dispose()
        {
            
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
