using System;
using System.Threading;
using CarJam.Scripts.Vehicles.Models;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UniRx;
using UnityEngine;
using Zenject;
namespace CarJam.Scripts.Vehicles.Views
{
    public class VehicleView : MonoBehaviour, IDisposable
    {
        [SerializeField] private Renderer _renderer;

        private readonly CompositeDisposable _disposables = new CompositeDisposable();
        
        private VehicleModel _model;

        public Guid Id => _model.Id;


        [Inject]
        private void Construct(VehicleModel model)
        {
            _model = model;
            _model.Material.Subscribe(material => _renderer.material = material).AddTo(_disposables);
        }

        public async UniTask<bool> MoveByWaypoints(Vector3[] waypoints, CancellationToken token)
        {
            await GetMovingSequence(waypoints).ToUniTask(cancellationToken: token);
            return !token.IsCancellationRequested;
        }

        private Sequence GetMovingSequence(Vector3[] waypoints)
        {
            var moving = DOTween.Sequence();
            foreach (var waypoint in waypoints)
            {
                moving.Append(transform.DOLookAt(waypoint, 0));
                moving.Join(transform.DOMove(waypoint, Vector3.Distance(transform.position, waypoint) / _model.MovementSpeed));
            }
            moving.SetEase(Ease.Linear);
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
