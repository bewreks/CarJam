﻿using System;
using System.Collections.Generic;
using System.Threading;
using CarJam.Scripts.Data;
using CarJam.Scripts.Parking.Presenters;
using CarJam.Scripts.Parking.Waypoints;
using CarJam.Scripts.Signals;
using CarJam.Scripts.Vehicles.Presenters;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Zenject;
namespace CarJam.Scripts.Parking
{
    public class ParkingFacade : IDisposable
    {
        [Inject] private VehiclePresenter.Factory _vehiclePresenterFactory;
        [Inject] private ParkingPresenter.Factory _presenterFactory;
        [Inject] private SignalBus _bus;
        
        private ParkingPresenter _presenter;

        private Vector3 _outPoint;
        private int _vehiclesLayerMask;
        private Dictionary<Guid, VehiclePresenter> _vehicles = new Dictionary<Guid, VehiclePresenter>();
        
        private CancellationTokenSource _cancellationTokenSource;

        [Inject]
        private void Construct(Vector3 rbPoint, Vector3 ltPoint, Vector3 outPoint)
        {
            _outPoint = outPoint;
            _presenter = _presenterFactory.Create(rbPoint, ltPoint);
            _vehiclesLayerMask = 1 << 6;
        }
        
        private void InitializeCancellationToken()
        {
            _cancellationTokenSource?.Cancel();
            _cancellationTokenSource?.Dispose();
            _cancellationTokenSource = new CancellationTokenSource();
        }
        
        private void SubscribeToSignals()
        {
            _bus.Subscribe<BusStopFoundSignal>(OnVehicleSelected);
            _bus.Subscribe<CharacterOnAboardSignal>(OnCharacterOnAboard);
        }
        
        public void UnsubscribeFromSignals()
        {
            _bus.TryUnsubscribe<BusStopFoundSignal>(OnVehicleSelected);
            _bus.TryUnsubscribe<CharacterOnAboardSignal>(OnCharacterOnAboard);
        }

        private void OnCharacterOnAboard(CharacterOnAboardSignal signal)
        {
            var vehicle = _vehicles[signal.VehicleId];
            vehicle.OnAboard();
            if (vehicle.IsFull)
            {
                _bus.Fire(new VehicleMoveOutBusStopSignal
                {
                    BusStopId = vehicle.BusStopId,
                    VehicleId = vehicle.Id,
                    Color = vehicle.Color
                });
                var ray = new Ray(vehicle.Position, -vehicle.Direction);
                if (_presenter.Model.TopPlane.Raycast(ray, out var distance))
                {
                    var waypoints = new Waypoint[2];
                    waypoints[0] = new Waypoint(ray.GetPoint(distance), true);
                    waypoints[1] = new Waypoint(_outPoint);
                    vehicle.MoveByWaypoints(waypoints, _cancellationTokenSource.Token).Forget(); 
                }
                else
                {
                    vehicle.DestroySelf();
                }
            }
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

            MoveToBusStop(signal, vehicle, _cancellationTokenSource.Token).Forget();
        }

        private async UniTask MoveToBusStop(BusStopFoundSignal signal, VehiclePresenter vehicle, CancellationToken token)
        {
            _bus.Fire(new StartVehicleMovingToBusStopSignal
            {
                BusStopId = signal.BusStopId,
            });
            vehicle.SetBusStopId(signal.BusStopId);
            var waypoints = WaypointBuilder.BuildWaypoints(vehicle, _presenter, signal.Position, signal.EnterPoint);
            await vehicle.MoveByWaypoints(waypoints, token);
            
            if (token.IsCancellationRequested) return;
            
            _bus.Fire(new FinishVehicleMovingToBusStopSignal
            {
                VehicleId = signal.VehicleId, 
                BusStopId = signal.BusStopId,
                Color = vehicle.Color
            });
        }

        public void LoadLevel(Level level)
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
            _cancellationTokenSource?.Cancel();
            _cancellationTokenSource?.Dispose();
            _cancellationTokenSource = null;
            
            UnsubscribeFromSignals();
            foreach (var (key, value) in _vehicles)
            {
                value.DestroySelf();
            }
            _vehicles.Clear();
        }

        public void Restart()
        {
            InitializeCancellationToken();
            SubscribeToSignals();
        }
    }
}
