using System;
using System.Threading;
using CarJam.Scripts.Data;
using CarJam.Scripts.Data.Settings;
using CarJam.Scripts.Vehicles.Models;
using CarJam.Scripts.Vehicles.Views;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Zenject;
using Object = UnityEngine.Object;
namespace CarJam.Scripts.Vehicles.Presenters
{
    public class VehiclePresenter : IDisposable
    {
        [Inject] private VehicleModel.Factory _modelFactory;
        [Inject] private DiContainer _container;
        
        private VehicleModel _model;
        private VehicleView _view;
        
        private CancellationTokenSource _movementCts;

        public GameColors Color => _model.Color;

        public Vector3 Position => _view.transform.position;
        public Vector3 Direction => _view.transform.forward;
        public Guid Id => _model.Id;

        public bool IsFull => _model.CurrentCapacity.Value >= _model.MaxCapacity;

        public Guid BusStopId => _model.BusStopId;

        [Inject]
        private void Construct(VehiclesData data)
        {
            var settings = _container.ResolveId<VehicleSettings>(data.Type);
            var viewFactory = _container.ResolveId<VehicleView.Factory>(data.Type);

            _model = _modelFactory.Create();
            _model.Id = Guid.Parse(data.Id);
            _model.Color = data.Color;
            _model.MaxCapacity = settings.Capacity;
            _model.Material.Value = settings.Materials[data.Color];
            _model.MovementSpeed = settings.MovementSpeed;
            _model.RotationSpeed = settings.RotationSpeed;

            _view = viewFactory.Create(_model);
            _view.name = data.Id;
            _view.transform.position = data.Position;
            _view.transform.forward = data.Direction;
        }

        public void DestroySelf()
        {
            Dispose();
            if (_view)
            {
                Object.Destroy(_view.gameObject);
            }
        }

        public void Dispose()
        {
            _movementCts?.Dispose();
            _view.Dispose();
        }

        public async UniTask MoveByWaypoints(Waypoint[] waypoints, CancellationToken token)
        {
            _movementCts?.Cancel();
            _movementCts = CancellationTokenSource.CreateLinkedTokenSource(token);
            _model.IsMoving.Value = true;
            var result = await _view.MoveByWaypoints(waypoints, _movementCts.Token);
            if (result)
            {
                _model.IsMoving.Value = false;
            }
        }

        public void OnAboard()
        {
            _model.CurrentCapacity.Value += 1;
        }

        public void SetBusStopId(Guid id)
        {
            _model.BusStopId = id;
        }
        
        public class Factory : PlaceholderFactory<VehiclesData, VehiclePresenter>
        {
            
        }
    }
}
