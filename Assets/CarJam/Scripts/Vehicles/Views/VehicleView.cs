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


        [Inject]
        private void Construct(VehicleModel model)
        {
            _model = model;
            _model.Material.Subscribe(material => _renderer.material = material).AddTo(_disposables);
        }

        public async UniTask<bool> MoveToPosition(Vector3 position, CancellationToken token)
        {
            transform.forward = position - transform.position;
            await transform.DOMove(position, Vector3.Distance(transform.position, position) / _model.MovementSpeed)
                           .SetEase(Ease.Linear)
                           .ToUniTask(cancellationToken: token);
            return !token.IsCancellationRequested;
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
