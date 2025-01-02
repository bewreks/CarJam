using System;
using System.Collections.Generic;
using System.Linq;
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

        public CharactersQueueFacade(Vector3 startPoint, Vector3 finishPoint, Vector3 characterSpawnPoint) : base(startPoint, finishPoint, characterSpawnPoint)
        {
        }

        protected override void OnInitialize()
        {
            _vehiclesOnBusStop = new Dictionary<GameColors, List<Guid>>();
            foreach (GameColors colors in Enum.GetValues(typeof(GameColors)))
            {
                _vehiclesOnBusStop[colors] = new List<Guid>();
            }
            _signalBus.Subscribe<StartGameSignal>(OnStartGame);
            _signalBus.Subscribe<FinishVehicleMovingToBusStopSignal>(OnVehicleOnBusStop);
            _signalBus.Subscribe<VehicleMoveOutBusStopSignal>(OnVehicleMoveOutBusStop);
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

            SubscribeToDespawn();
        }

        private void SubscribeToDespawn()
        {
            if (_despawnHandler == null)
            {
                _despawnHandler = Observable.Timer(TimeSpan.FromSeconds(_gameModel.CharacterDespawnCooldown)).Repeat().Subscribe(OnCharacterDespawn);
            }
        }

        private void UnsubscribeToDespawn()
        {
            _despawnHandler?.Dispose();
            _despawnHandler = null;
        }

        private void OnStartGame(StartGameSignal signal)
        {
            _gameModel = signal.GameModel;
            _counter = _gameModel.CurrentLevel.CharactersCounter;
            _spawnHandler = Observable.Timer(TimeSpan.FromSeconds(_gameModel.CharacterSpawnCooldown)).Repeat().Subscribe(OnCharacterSpawn);
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
            Enqueue(key).Forget();
        }

        protected override void BeforeEnqueue(GameColors color)
        {
            _counter[color]--;
            if (_counter[color] == 0) _counter.Remove(color);
        }

        private void OnCharacterDespawn(long _)
        {
            if (_vehiclesOnBusStop[_queue.First.Color].Count == 0) return;
            
            Dequeue();
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
            _signalBus.Unsubscribe<VehicleMoveOutBusStopSignal>(OnVehicleMoveOutBusStop);
            _signalBus.Unsubscribe<FinishVehicleMovingToBusStopSignal>(OnVehicleOnBusStop);
            _signalBus.Unsubscribe<StartGameSignal>(OnStartGame);
            _spawnHandler?.Dispose();
            _despawnHandler?.Dispose();
        }
    }

}
