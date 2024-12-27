using CarJam.Scripts.CarJam;
using CarJam.Scripts.Queues.Base;
using CarJam.Scripts.Queues.BusStop.Presenters;
using CarJam.Scripts.Signals;
using CarJam.Scripts.Vehicles.Presenters;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Zenject;
namespace CarJam.Scripts.Queues.BusStop
{
    public class BusStopFacade : BaseQueueFacade<BusStopPlacePresenter, BusStopQueue>
    {
        [Inject] private BusStopQueueSettings _settings;
        [Inject] private BusStopPlacePresenter.Factory _placeFactory;
        [Inject] private SignalBus _bus;
        
        private readonly Transform _parent;
        
        public BusStopFacade(Vector3 startPoint, Vector3 finishPoint, Transform parent) : base(startPoint, finishPoint, startPoint)
        {
            _parent = parent;
        }

        protected override void OnInitialize()
        {
            var placesCount = (int)(Vector3.Distance(_startPoint, _finishPoint) / DistanceBetweenObject);
            
            for (var i = 0; i < placesCount; i++)
            {
                Enqueue(GameColors.None).Forget();
            }
        }

        protected override float DistanceBetweenObject => _settings.DistanceBetweenVehicles;
        
        protected override BusStopPlacePresenter TFactory(GameColors color)
        {
            var presenter = _placeFactory.Create();
            presenter.SetParent(_parent);
            return presenter;
        }

        protected override void OnDequeue(BusStopPlacePresenter obj)
        {
            
        }

        public async UniTask TryToMoveVehicle(VehiclePresenter vehicle)
        {
            var place = _queue.GetEmptyPlace();
            
            if (place == null)
            {
                _bus.Fire<NoMorePlacesSignal>();
                return;
            }

            place.Reserve();
            await vehicle.MoveToPosition(place.EnterPoint);
            await vehicle.MoveToPosition(place.Position);
            place.SetVehicle(vehicle);
            place.Unreserve();
        }
    }
}
