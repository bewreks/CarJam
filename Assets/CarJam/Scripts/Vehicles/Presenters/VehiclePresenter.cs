﻿using System;
using System.Threading;
using System.Threading.Tasks;
using CarJam.Scripts.CarJam;
using CarJam.Scripts.Vehicles.Data;
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
        [Inject] private VehicleModel.Factory _modelFactory;
        [Inject] private DiContainer _container;
        
        private VehicleModel _model;
        private VehicleView _view;
        
        private CancellationTokenSource _movementCts;

        public GameColors Color => _model.Color;

        public Vector3 Position => _view.transform.position;
        public Vector3 Direction => _view.transform.forward;

        [Inject]
        private void Construct(VehiclesData data)
        {
            var settings = _container.ResolveId<VehicleSettings>(data.Type);
            var viewFactory = _container.ResolveId<VehicleView.Factory>(data.Type);

            _model = _modelFactory.Create();
            _model.Color = data.Color;
            _model.Material.Value = settings.Materials[data.Color];
            _model.MovementSpeed = settings.MovementSpeed;

            _view = viewFactory.Create(_model);
            _view.transform.position = data.Position;
            _view.transform.forward = data.Direction;
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
        
        public class Factory : PlaceholderFactory<VehiclesData, VehiclePresenter>
        {
            
        }
    }
}