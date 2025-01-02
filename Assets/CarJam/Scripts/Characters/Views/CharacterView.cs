using System;
using System.Threading;
using CarJam.Scripts.CarJam;
using CarJam.Scripts.Characters.Models;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UniRx;
using UnityEngine;
using Zenject;
namespace CarJam.Scripts.Characters.Views
{
    public class CharacterView : MonoBehaviour, IDisposable
    {
        private static readonly int IsRunning = Animator.StringToHash("IsRunning");

        [SerializeField] private Animator _animator;
        [SerializeField] private SkinnedMeshRenderer _renderer;

        private readonly CompositeDisposable _disposables = new CompositeDisposable();
        
        private CharacterModel _model;


        [Inject]
        private void Construct(CharacterModel model)
        {
            _model = model;
            _model.Material.Subscribe(material => _renderer.material = material).AddTo(_disposables);
            _model.IsMoving.SkipLatestValueOnSubscribe().Subscribe(isMoving => _animator.SetBool(IsRunning, isMoving)).AddTo(_disposables);
        }

        public async UniTask<bool> MoveToPosition(Vector3 position, CancellationToken token)
        {
            if (transform.position == position) return true; 
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

        public class Factory : PlaceholderFactory<CharacterModel, CharacterView>
        {

        }
    }
}
