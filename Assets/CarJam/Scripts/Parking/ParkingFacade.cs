using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CarJam.Scripts.CarJam;
using CarJam.Scripts.Queues.BusStop.Presenters;
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
        [Inject] private VehiclePresenter.Factory _vehiclePresenterFactory;
        [Inject] private ParkingPresenter.Factory _presenterFactory;
        [Inject] private SignalBus _bus;
        
        private ParkingPresenter _presenter;

        private Dictionary<Guid, VehiclePresenter> _vehicles = new Dictionary<Guid, VehiclePresenter>();
        private int _vehiclesLayerMask;

        [Inject]
        private void Construct(Vector3 rbPoint, Vector3 ltPoint)
        {
            _presenter = _presenterFactory.Create(rbPoint, ltPoint);
            _bus.Subscribe<BusStopFoundSignal>(OnVehicleSelected);
            _vehiclesLayerMask = 1 << 6;
        }

        private void OnVehicleSelected(BusStopFoundSignal signal)
        {
            if (!_vehicles.TryGetValue(signal.VehicleId, out var vehicle))
            {
                throw new Exception($"Vehicle with id {signal.VehicleId} not found");
            }

            var ray = new Ray(vehicle.Position, vehicle.Direction);
            if (Physics.Raycast(ray, 100, _vehiclesLayerMask))
            {
                return;
            }

            MoveToBusStop(signal, vehicle).Forget();
        }

        private async UniTask MoveToBusStop(BusStopFoundSignal signal, VehiclePresenter vehicle)
        {

            _bus.Fire(new StartVehicleMovingToBusStopSignal
            {
                BusStopId = signal.BusStopId,
            });
            await vehicle.MoveByWaypoints(WaypointBuilder.BuildWaypoints(vehicle, _presenter, signal.Position, signal.EnterPoint));
            _bus.Fire(new FinishVehicleMovingToBusStopSignal{
                VehicleId = signal.VehicleId, 
                BusStopId = signal.BusStopId,
                Color = vehicle.Color
            });
        }

        public void LoadLevel(LevelScriptableObject level)
        {
            _vehicles.Clear();
            foreach (var vehicleData in level.Vehicles)
            {
                _vehicles.Add(new Guid(vehicleData.Id), _vehiclePresenterFactory.Create(vehicleData));
            }

            _bus.Fire<LevelLoadedSignal>();
        }

        public void Dispose()
        {
            _bus.Unsubscribe<BusStopFoundSignal>(OnVehicleSelected);
            _vehicles.Clear();
        }
    }
}
