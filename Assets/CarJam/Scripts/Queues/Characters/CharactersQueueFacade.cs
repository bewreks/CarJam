using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
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
        
        private GameModel _gameModel;
        
        private Dictionary<GameColors, List<Guid>> _vehiclesOnBusStop;

        private IDisposable _spawnHandler;
        private IDisposable _despawnHandler;
        private Dictionary<GameColors, int> _counter;

        private CancellationTokenSource _cancellationToken;

        public CharactersQueueFacade(Vector3 startPoint, Vector3 finishPoint, Vector3 characterSpawnPoint) : base(startPoint, finishPoint, characterSpawnPoint)
        {
        }

        protected override void OnInitialize()
        {
            _vehiclesOnBusStop = new Dictionary<GameColors, List<Guid>>();
        }
        
        private void SubscribeToSignals()
        {
            _signalBus.Subscribe<GameStartedSignal>(OnStartGame);
            _signalBus.Subscribe<FinishVehicleMovingToBusStopSignal>(OnVehicleOnBusStop);
            _signalBus.Subscribe<VehicleMoveOutBusStopSignal>(OnVehicleMoveOutBusStop);
        }
        
        private void UnsubscribeFromSignals()
        {
            _signalBus.TryUnsubscribe<VehicleMoveOutBusStopSignal>(OnVehicleMoveOutBusStop);
            _signalBus.TryUnsubscribe<FinishVehicleMovingToBusStopSignal>(OnVehicleOnBusStop);
            _signalBus.TryUnsubscribe<GameStartedSignal>(OnStartGame);
        }

        private void InitializeCancellationToken()
        {
            _cancellationToken?.Cancel();
            _cancellationToken?.Dispose();
            _cancellationToken = new CancellationTokenSource();
        }

        private void InitializeVehicleDictionaries()
        {
            _vehiclesOnBusStop.Clear();
            foreach (GameColors colors in Enum.GetValues(typeof(GameColors)))
            {
                _vehiclesOnBusStop[colors] = new List<Guid>();
            }
        }

        private void OnVehicleMoveOutBusStop(VehicleMoveOutBusStopSignal signal)
        {
            _vehiclesOnBusStop[signal.Color].Remove(signal.VehicleId);

            if (_vehiclesOnBusStop.Values.Sum(list => list.Count) == 0)
            {
                UnsubscribeToDespawn();
            }
        }

        private void OnVehicleOnBusStop(FinishVehicleMovingToBusStopSignal signal)
        {
            _vehiclesOnBusStop[signal.Color].Add(signal.VehicleId);

            _gameModel.IsCharactersQueueWaiting.Value = false;

            SubscribeToDespawn();
        }

        private void SubscribeToDespawn()
        {
            _despawnHandler ??= Observable.Timer(TimeSpan.FromSeconds(_gameModel.CharacterDespawnCooldown)).Repeat().Subscribe(OnCharacterDespawn);
        }
        
        private void SubscribeToSpawn()
        {
            _spawnHandler ??= Observable.Timer(TimeSpan.FromSeconds(_gameModel.CharacterSpawnCooldown)).Repeat().Subscribe(OnCharacterSpawn);
        }

        private void UnsubscribeToDespawn()
        {
            _despawnHandler?.Dispose();
            _despawnHandler = null;
        }

        private void OnStartGame(GameStartedSignal startedSignal)
        {
            _gameModel = startedSignal.GameModel;
            _counter = _gameModel.CurrentLevel.CharactersCounter;
            SubscribeToSpawn();
        }

        private void OnCharacterSpawn(long _)
        {
            if (_counter.Count == 0)
            {
                _signalBus.Fire<NoMoreCharactersToSpawnSignal>();
                _spawnHandler.Dispose();
                _spawnHandler = null;
                return;
            }
            var (key, value) = _counter.ElementAt(Random.Range(0, _counter.Count));
            Enqueue(key, _cancellationToken.Token).Forget();
        }

        protected override void BeforeEnqueue(GameColors color)
        {
            _counter[color]--;
            if (_counter[color] == 0) _counter.Remove(color);
        }

        private void OnCharacterDespawn(long _)
        {
            if (_vehiclesOnBusStop[_queue.First.Color].Count == 0)
            {
                _gameModel.IsCharactersQueueWaiting.Value = true;
                UnsubscribeToDespawn();
                return;
            }
            
            Dequeue(_cancellationToken.Token);
        }

        protected override CharacterPresenter TFactory(GameColors color)
        {
            return _characterFactory.Create(color, _spawnPoint);
        }

        protected override void OnDequeue(CharacterPresenter obj)
        {
            obj.DestroySelf();
            _signalBus.Fire(new CharacterOnAboardSignal
            {
                VehicleId = _vehiclesOnBusStop[obj.Color][0]
            });
            if (_queue.Count == 0)
            {
                _signalBus.Fire<LevelClearedSignal>();
            }
        }

        protected override void OnDispose()
        {
            _spawnHandler?.Dispose();
            _spawnHandler = null;
            _despawnHandler?.Dispose();
            _despawnHandler = null;
            _cancellationToken?.Cancel();
            _cancellationToken?.Dispose();
            _cancellationToken = null;
            UnsubscribeFromSignals();
            _counter?.Clear();
        }

        public void Restart()
        {
            InitializeCancellationToken();
            SubscribeToSignals();
            InitializeVehicleDictionaries();
        }
    }

}
