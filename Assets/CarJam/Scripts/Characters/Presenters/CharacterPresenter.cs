using System;
using System.Threading;
using CarJam.Scripts.CarJam;
using CarJam.Scripts.Characters.Models;
using CarJam.Scripts.Characters.Views;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Zenject;
using Object = UnityEngine.Object;
namespace CarJam.Scripts.Characters.Presenters
{
    public class CharacterPresenter : IDisposable
    {
        [Inject] private CharacterSettings _settings;
        [Inject] private CharacterModel.Factory _modelFactory;
        [Inject] private CharacterView.Factory _viewFactory;
        
        private CharacterModel _model;
        private CharacterView _view;
        
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

        public bool IsMoving => _model.IsMoving.Value;

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

        public class Factory : PlaceholderFactory<GameColors, Vector3, CharacterPresenter>
        {
            
        }
    }
}
