using System;
using System.Threading.Tasks;
using CarJam.Scripts.CarJam;
using CarJam.Scripts.Queues.BusStop;
using CarJam.Scripts.Queues.Parking.Presenters;
using CarJam.Scripts.Signals;
using CarJam.Scripts.Vehicles.Presenters;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Zenject;
namespace CarJam.Scripts.Queues.Parking
{
    public class ParkingFacade : IDisposable
    {
        [Inject] private BusStopFacade _busStopFacade;
        [Inject] private VehiclePresenter.Factory _vehiclePresenterFactory;
        [Inject] private ParkingPresenter.Factory _presenterFactory;
        [Inject] private SignalBus _bus;
        
        private ParkingPresenter _presenter;
        private Camera _camera;
        private Plane _plane;

        [Inject]
        private void Construct(Camera camera)
        {
            _plane = new Plane(Vector3.up, Vector3.zero);

            _camera = camera;
            _presenter = _presenterFactory.Create();

            _bus.Subscribe<DebugPlaceBusSignal>(OnPlaceBus);
        }

        private void OnPlaceBus(DebugPlaceBusSignal signal)
        {
            var ray = _camera.ScreenPointToRay(signal.SelectionPosition);
            if (_plane.Raycast(ray, out var distance))
            {
                var vehicle = _vehiclePresenterFactory.Create(GameColors.Blue, ray.GetPoint(distance));
                _busStopFacade.TryToMoveVehicle(vehicle).Forget();
            }
        }

        public void Dispose()
        {
            _bus.Unsubscribe<DebugPlaceBusSignal>(OnPlaceBus);
        }
    }
}
