using System;
using System.Threading.Tasks;
using CarJam.Scripts.CarJam;
using CarJam.Scripts.Queues.BusStop;
using CarJam.Scripts.Queues.Parking.Presenters;
using CarJam.Scripts.Signals;
using CarJam.Scripts.Utils;
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
        private void Construct(Vector3 rbPoint, Vector3 ltPoint, Camera camera)
        {
            _plane = new Plane(Vector3.up, Vector3.zero);

            _camera = camera;
            _presenter = _presenterFactory.Create(rbPoint, ltPoint);

            _bus.Subscribe<DebugPlaceBusSignal>(OnPlaceBus);
        }

        private async void OnPlaceBus(DebugPlaceBusSignal signal)
        {
            var ray = _camera.ScreenPointToRay(signal.SelectionPosition);
            if (_plane.Raycast(ray, out var distance))
            {
                var busStop = _busStopFacade.GetBusStopPresenter();
                if (busStop == null)
                {
                    _bus.Fire<NoMorePlacesSignal>();
                    return;
                }
                var vehicle = _vehiclePresenterFactory.Create(GameColors.Blue, ray.GetPoint(distance));
                busStop.Reserve();
                await vehicle.MoveByWaypoints(WaypointBuilder.BuildWaypoints(vehicle, _presenter, busStop));
                busStop.SetVehicle(vehicle);
                busStop.Reserve();
            }
        }

        public void Dispose()
        {
            _bus.Unsubscribe<DebugPlaceBusSignal>(OnPlaceBus);
        }

        public ParkingPresenter GetPresenter()
        {
            return _presenter;
        }
    }
}
