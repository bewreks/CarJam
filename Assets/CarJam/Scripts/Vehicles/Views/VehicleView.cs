using System;
using System.Threading;
using CarJam.Scripts.Data;
using CarJam.Scripts.Vehicles.Models;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UniRx;
using UnityEngine;
using Zenject;
namespace CarJam.Scripts.Vehicles.Views
{
    [SelectionBase]
    public class VehicleView : MonoBehaviour, IDisposable
    {
        [SerializeField] private Renderer _renderer;
        [SerializeField] private Canvas _canvas;
        [SerializeField] private SlicedFilledImage _loader;

        private readonly CompositeDisposable _disposables = new CompositeDisposable();
        
        private VehicleModel _model;

        public Guid Id => _model.Id;

        private void Awake()
        {
            _canvas.gameObject.SetActive(false);
        }

        [Inject]
        private void Construct(VehicleModel model, Camera gameCamera)
        {
            _model = model;
            _model.Material.Subscribe(material => _renderer.material = material).AddTo(_disposables);
            _model.CurrentCapacity.Subscribe(current => _loader.fillAmount = current/(float)_model.MaxCapacity).AddTo(_disposables);
            _canvas.worldCamera = gameCamera;
        }

        public async UniTask<bool> MoveByWaypoints(Waypoint[] waypoints, CancellationToken token)
        {
            await GetMovingSequence(waypoints).WithCancellation(token);
            return !token.IsCancellationRequested;
        }

        private Sequence GetMovingSequence(Waypoint[] waypoints)
        {
            var moving = DOTween.Sequence();
            moving.SetLink(gameObject);
            var cachedPosition = transform.position;
            var cachedDirection = transform.forward;
            foreach (var waypoint in waypoints)
            {
                var toDirection = (waypoint.Position - cachedPosition).normalized;

                if (waypoint.Reverse)
                {
                    toDirection = -toDirection;
                }
                
                var rotationDistance = 1 - (Vector3.Dot(cachedDirection, toDirection) + 1) / 2;
                var movingDistance = Vector3.Distance(transform.position, waypoint.Position);

                var rotationDuration = 0f;
                var movingDuration = 0f;
                if (_model.RotationSpeed != 0)
                {
                    rotationDuration = rotationDistance / _model.RotationSpeed;
                }
                if (_model.MovementSpeed != 0)
                {
                    movingDuration = movingDistance / _model.MovementSpeed;
                }
                moving.Append(DOTween.To(() => transform.forward, x =>
                {
                    transform.forward = x;
                }, toDirection, rotationDuration).SetEase(Ease.Linear));
                moving.Append(transform.DOMove(waypoint.Position, movingDuration).SetEase(Ease.Linear));

                cachedPosition = waypoint.Position;
                cachedDirection = toDirection;
            }
            moving.OnComplete(() =>
            {
                _canvas.gameObject.SetActive(true);
            });
            return moving;
        }

        public void Dispose()
        {
            _disposables.Dispose();
        }

        public class Factory : PlaceholderFactory<VehicleModel, VehicleView>
        {
        }
    }
}
