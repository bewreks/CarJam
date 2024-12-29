using System;
using System.Collections.Generic;
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
        [Inject] private VehiclePresenter.Factory _vehiclePresenterFactory;
        [Inject] private ParkingPresenter.Factory _presenterFactory;
        [Inject] private SignalBus _bus;
        
        private ParkingPresenter _presenter;

        public ParkingPresenter Presenter => _presenter;
        
        private List<VehiclePresenter> _vehicles = new List<VehiclePresenter>();

        [Inject]
        private void Construct(Vector3 rbPoint, Vector3 ltPoint)
        {
            _presenter = _presenterFactory.Create(rbPoint, ltPoint);
        }

        public void LoadLevel(LevelScriptableObject level)
        {
            _vehicles.Clear();
            foreach (var vehicleData in level.Vehicles)
            {
                _vehicles.Add(_vehiclePresenterFactory.Create(vehicleData));
            }

            _bus.Fire<LevelLoadedSignal>();
        }

        public void Dispose()
        {
            _vehicles.Clear();
        }
    }
}
