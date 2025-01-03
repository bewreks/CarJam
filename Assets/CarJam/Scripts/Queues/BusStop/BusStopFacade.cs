using System;
using System.Threading;
using CarJam.Scripts.CarJam;
using CarJam.Scripts.Data;
using CarJam.Scripts.Data.Settings;
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
        
        private CancellationTokenSource _cancellationToken;
        private GameModel _gameModel;

        public BusStopFacade(Vector3 startPoint, Vector3 finishPoint, Transform parent) : base(startPoint, finishPoint, startPoint)
        {
            _parent = parent;
        }
        
        private void SubscribeToSignals()
        {
            _bus.Subscribe<GameStartedSignal>(OnGameStarted);
            _bus.Subscribe<UserSelectionSignal>(OnVehicleSelected);
            _bus.Subscribe<StartVehicleMovingToBusStopSignal>(OnStartVehicleMoving);
            _bus.Subscribe<FinishVehicleMovingToBusStopSignal>(OnFinishVehicleMoving);
            _bus.Subscribe<VehicleMoveOutBusStopSignal>(OnVehicleMoveOutBusStop);
        }
        
        private void UnsubscribeFromSignals()
        {
            _bus.TryUnsubscribe<GameStartedSignal>(OnGameStarted);
            _bus.TryUnsubscribe<UserSelectionSignal>(OnVehicleSelected);
            _bus.TryUnsubscribe<StartVehicleMovingToBusStopSignal>(OnStartVehicleMoving);
            _bus.TryUnsubscribe<FinishVehicleMovingToBusStopSignal>(OnFinishVehicleMoving);
            _bus.TryUnsubscribe<VehicleMoveOutBusStopSignal>(OnVehicleMoveOutBusStop);
        }
        
        private void OnGameStarted(GameStartedSignal signal)
        {
            _gameModel = signal.GameModel;
        }

        private void InitializeCancellationToken()
        {
            _cancellationToken?.Cancel();
            _cancellationToken?.Dispose();
            _cancellationToken = new CancellationTokenSource();
        }

        private void OnVehicleMoveOutBusStop(VehicleMoveOutBusStopSignal signal)
        {
            var place = _queue.GetPlace(signal.BusStopId);
            place.SetVehicle(Guid.Empty, GameColors.None);
            _gameModel.IsBusStopsQueueFull.Value = false;
        }

        private void OnFinishVehicleMoving(FinishVehicleMovingToBusStopSignal signal)
        {
            var place = _queue.GetPlace(signal.BusStopId);
            place.SetVehicle(signal.VehicleId, signal.Color);
            place.Unreserve();
            if (_queue.Count == 0)
            {
                _gameModel.IsBusStopsQueueFull.Value = true;
            }
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
            InitializeCancellationToken();
            InitializePlaces();
        }
        
        private void InitializePlaces()
        {
            var placesCount = (int)(Vector3.Distance(_startPoint, _finishPoint) / DistanceBetweenObject);

            for (var i = 0; i < placesCount; i++)
            {
                Enqueue(GameColors.None, _cancellationToken.Token).Forget();
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
            _cancellationToken?.Cancel();
            _cancellationToken?.Dispose();
            _cancellationToken = null;
            
            UnsubscribeFromSignals();
        }

        public void Restart()
        {
            InitializeCancellationToken();
            SubscribeToSignals();
        }
    }
}
