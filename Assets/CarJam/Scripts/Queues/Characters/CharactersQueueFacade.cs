using System;
using CarJam.Scripts.CarJam;
using CarJam.Scripts.Characters.Presenters;
using CarJam.Scripts.Queues.Base;
using CarJam.Scripts.Signals;
using Cysharp.Threading.Tasks;
using UniRx;
using UnityEngine;
using Zenject;
using Random = UnityEngine.Random;
namespace CarJam.Scripts.Queues.Characters
{
    public class CharactersQueueFacade : BaseQueueFacade<CharacterPresenter, CharactersQueue>
    {
        [Inject] private CharactersQueueSettings _settings;
        [Inject] private CharacterPresenter.Factory _characterFactory;
        [Inject] private SignalBus _signalBus;
        
        protected override float DistanceBetweenObject => _settings.DistanceBetweenCharacters;
        
        private CompositeDisposable _disposables = new CompositeDisposable();
        private GameModel _gameModel;

        public CharactersQueueFacade(Vector3 startPoint, Vector3 finishPoint, Vector3 characterSpawnPoint) : base(startPoint, finishPoint, characterSpawnPoint)
        {
        }

        protected override void OnInitialize()
        {
            _signalBus.Subscribe<StartGameSignal>(OnStartGame);
        }

        private void OnStartGame(StartGameSignal signal)
        {
            _gameModel = signal.GameModel;
            Observable.Timer(TimeSpan.FromSeconds(_gameModel.CharacterSpawnCooldown)).Repeat().Subscribe(OnCharacterSpawn).AddTo(_disposables);
        }

        private void OnCharacterSpawn(long _)
        {
            Enqueue(_gameModel.InGameColors[Random.Range(0, _gameModel.InGameColors.Length)]).Forget();
        }

        protected override CharacterPresenter TFactory(GameColors color)
        {
            return _characterFactory.Create(color, _spawnPoint);
        }

        protected override void OnDequeue(CharacterPresenter obj)
        {
            obj.DestroySelf();
        }

        protected override void OnDispose()
        {
            _signalBus.Unsubscribe<StartGameSignal>(OnStartGame);
            _disposables.Dispose();
        }
    }

}
