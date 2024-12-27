using System.Threading;
using CarJam.Scripts.CarJam;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;
using Zenject;
namespace CarJam.Scripts.Characters.Views
{
    public class CharacterView : MonoBehaviour
    {
        private static readonly int IsRunning = Animator.StringToHash("IsRunning");

        [SerializeField] private Animator _animator;
        [SerializeField] private SkinnedMeshRenderer _renderer;
        
        [Inject] private CharacterSettings _settings;

        private CancellationTokenSource _cancellationTokenSource;

        public bool IsMoving => _animator.GetBool(IsRunning);

        [Inject]
        private void Construct(GameColors color)
        {
            _renderer.material = _settings.Materials[color];
        }

        public async UniTask MoveToPosition(Vector3 position)
        {
            _cancellationTokenSource?.Cancel();
            _cancellationTokenSource = new CancellationTokenSource();
            transform.forward = position - transform.position;
            _animator.SetBool(IsRunning, true);
            if (await AnimateMoving(position, _cancellationTokenSource.Token))
            {
                _animator.SetBool(IsRunning, false);
            }
        }

        private async UniTask<bool> AnimateMoving(Vector3 position, CancellationToken token)
        {
            await transform.DOMove(position, Vector3.Distance(transform.position, position) / _settings.MovementSpeed)
                           .SetEase(Ease.Linear)
                           .ToUniTask(cancellationToken: token);
            return !token.IsCancellationRequested;
        }

        public void DestroySelf()
        {
            Destroy(gameObject);
        }
    }
}
