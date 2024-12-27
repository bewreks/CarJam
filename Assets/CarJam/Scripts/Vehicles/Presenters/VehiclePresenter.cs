using System;
using System.Threading;
using CarJam.Scripts.CarJam;
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
        [Inject] private VehicleSettings _settings;
        [Inject] private VehicleModel.Factory _modelFactory;
        [Inject] private VehicleView.Factory _viewFactory;
        
        private VehicleModel _model;
        private VehicleView _view;
        
        private CancellationTokenSource _movementCts;

        [Inject]
        private void Construct(GameColors color, Vector3 spawnPoint)
        {
            _model = _modelFactory.Create();
            _model.Color = color;
            _model.Material.Value = _settings.Materials[color];
            _model.MovementSpeed = _settings.MovementSpeed;

            _view = _viewFactory.Create(_model);
            _view.transform.position = spawnPoint;
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

        public async UniTask MoveToPosition(Vector3 position)
        {
            _movementCts?.Cancel();
            _movementCts = new CancellationTokenSource();
            _model.IsMoving.Value = true;
            var result = await _view.MoveToPosition(position, _movementCts.Token);
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
