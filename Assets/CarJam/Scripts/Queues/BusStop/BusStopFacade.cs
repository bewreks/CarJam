using System;
using CarJam.Scripts.CarJam;
using CarJam.Scripts.Queues.Base;
using CarJam.Scripts.Queues.BusStop.Presenters;
using CarJam.Scripts.Signals;
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

        [Inject]
        private void Construct()
        {
            _bus.Subscribe<UserSelectionSignal>(OnVehicleSelected);
            _bus.Subscribe<StartVehicleMovingToBusStopSignal>(OnStartVehicleMoving);
            _bus.Subscribe<FinishVehicleMovingToBusStopSignal>(OnFinishVehicleMoving);
        }

        private void OnFinishVehicleMoving(FinishVehicleMovingToBusStopSignal signal)
        {
            var place = _queue.GetPlace(signal.BusStopId);
            place.SetVehicle(signal.VehicleId, signal.Color);
            place.Unreserve();
        }

        private void OnStartVehicleMoving(StartVehicleMovingToBusStopSignal signal)
        {
            var place = _queue.GetPlace(signal.BusStopId);
            place.Reserve();
        }

        private void OnVehicleSelected(UserSelectionSignal signal)
        {
            var busStop = _queue.GetEmptyPlace();
            if (busStop == null)
            {
                _bus.Fire<NoMorePlacesSignal>();
                return;
            }
            
            _bus.Fire(new BusStopFoundSignal
            {
                BusStopId = busStop.Id,
                Position = busStop.Position,
                EnterPoint = busStop.EnterPoint,
                VehicleId = signal.VehicleId
            });
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

        protected override void OnDispose()
        {
            _bus.Unsubscribe<StartVehicleMovingToBusStopSignal>(OnStartVehicleMoving);
            _bus.Unsubscribe<FinishVehicleMovingToBusStopSignal>(OnFinishVehicleMoving);
            _bus.Unsubscribe<UserSelectionSignal>(OnVehicleSelected);
        }
    }
}
