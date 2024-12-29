using System;
using System.Threading;
using System.Threading.Tasks;
using CarJam.Scripts.CarJam;
using CarJam.Scripts.Vehicles.Models;
using CarJam.Scripts.Vehicles.Views;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Zenject;
using Object = UnityEngine.Object;
using Random = UnityEngine.Random;
namespace CarJam.Scripts.Vehicles.Presenters
{
    public class VehiclePresenter : IDisposable
    {
        [Inject] private VehicleSettings _settings;
        [Inject] private VehicleModel.Factory _modelFactory;
        [Inject] private VehicleView.Factory _viewFactory;
        
        private VehicleModel _model;
        private VehicleView _view;
        
        private CancellationTokenSource _movementCts;

        public GameColors Color => _model.Color;

        public Vector3 Position => _view.transform.position;
        public Vector3 Direction => _view.transform.forward;

        [Inject]
        private void Construct(GameColors color, Vector3 spawnPoint)
        {
            _model = _modelFactory.Create();
            _model.Color = color;
            _model.Material.Value = _settings.Materials[color];
            _model.MovementSpeed = _settings.MovementSpeed;

            _view = _viewFactory.Create(_model);
            _view.transform.position = spawnPoint;
            _view.transform.rotation = Quaternion.Euler(0, Random.Range(0, 360), 0);
        }

        public void DestroySelf()
        {
            Dispose();
            Object.Destroy(_view.gameObject);
        }

        public void Dispose()
        {
            _movementCts?.Dispose();
            _view.Dispose();
        }

        public async Task MoveByWaypoints(Vector3[] waypoints)
        {
            _movementCts?.Cancel();
            _movementCts = new CancellationTokenSource();
            _model.IsMoving.Value = true;
            var result = await _view.MoveByWaypoints(waypoints, _movementCts.Token);
            if (result)
            {
                _model.IsMoving.Value = false;
            }
        }
        
        public class Factory : PlaceholderFactory<GameColors, Vector3, VehiclePresenter>
        {
            
        }
    }
}
