using System;
using CarJam.Scripts.Queues;
using CarJam.Scripts.Queues.Characters;
using Cysharp.Threading.Tasks;
using UniRx;
using Zenject;
using Random = UnityEngine.Random;
namespace CarJam.Scripts.CarJam
{
    public class CarJamGameplay : IInitializable, IDisposable
    {
        [Inject] private CarJamSettings _settings;
        [Inject] private LazyInject<CharactersQueueFacade> _charactersQueue;
        
        
        private CompositeDisposable _disposables = new CompositeDisposable();

        public void Initialize()
        {
            Observable.Timer(TimeSpan.FromSeconds(_settings.CharacterSpawnCooldown)).Repeat().Subscribe(OnCharacterSpawn).AddTo(_disposables);
        }
        
        private void OnCharacterSpawn(long _)
        {
            _charactersQueue.Value.Enqueue(_settings.InGameColors[Random.Range(0, _settings.InGameColors.Length)]).Forget();
        }

        public void Dispose()
        {
            _disposables.Dispose();
        }
    }
}
