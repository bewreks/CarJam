using System;
using System.Linq;
using CarJam.Scripts.Parking;
using CarJam.Scripts.Queues.BusStop;
using CarJam.Scripts.Queues.Characters;
using CarJam.Scripts.Signals;
using CarJam.Scripts.Vehicles;
using CarJam.Scripts.Vehicles.Data;
using UniRx;
using UnityEngine;
using Zenject;
namespace CarJam.Scripts.CarJam
{
    public class CarJamGameplay : IDisposable
    {
        [Inject] private CarJamSettings _settings;
        [Inject] private LazyInject<CharactersQueueFacade> _characters;
        [Inject] private LazyInject<BusStopFacade> _busStops;
        [Inject] private LazyInject<ParkingFacade> _parking;
        [Inject] private LevelScriptableObject _level;
        [Inject] private SignalBus _signalBus;
        [Inject] private DiContainer _container;
        
        private CompositeDisposable _disposables = new CompositeDisposable();
        private GameModel _gameModel;

        [Inject]
        public void Construct()
        {
            _signalBus.Subscribe<StartGameSignal>(OnStartGame);
            _signalBus.Subscribe<RestartGameSignal>(OnRestartGame);
            _signalBus.Subscribe<LevelLoadedSignal>(OnLevelLoaded);
            _signalBus.Subscribe<CharacterOnAboardSignal>(OnCharacterOnAboard);
        }

        private void OnRestartGame()
        {
            _gameModel.CurrentLevel = _level.CreateLevel(GetVehicleSettings());
            
            _characters.Value.Clear();
            _busStops.Value.Clear();
            _parking.Value.Clear();
            
            _parking.Value.LoadLevel(_gameModel.CurrentLevel);
        }

        private void OnStartGame()
        {
            _gameModel = new GameModel
            {
                CharacterSpawnCooldown = _settings.CharacterSpawnCooldown,
                CharacterDespawnCooldown = _settings.CharacterDespawnCooldown,
                CurrentLevel = _level.CreateLevel(GetVehicleSettings())
            };
            
            _parking.Value.LoadLevel(_gameModel.CurrentLevel);
        }

        private VehicleSettings[] GetVehicleSettings()
        {

            var settings = new VehicleSettings[Enum.GetNames(typeof(VehicleType)).Length];

            foreach (var type in Enum.GetValues(typeof(VehicleType)).Cast<VehicleType>())
            {
                settings[(int) type] = _container.ResolveId<VehicleSettings>(type);
            }
            return settings;
        }

        private void OnCharacterOnAboard()
        {
            _gameModel.Score += 1;
            _signalBus.Fire(new ScoreUpdateSignal
            {
                Score = _gameModel.Score
            });
        }

        private void StartGameCountdown()
        {
            _signalBus.Fire(new CountDownSignal
            {
                Countdown = _settings.StartGameCountdown + 1
            });
            IDisposable disposable = null;
            disposable = Observable.Interval( TimeSpan.FromSeconds(1))
                                   .TakeWhile(timer => timer <= _settings.StartGameCountdown)
                                   .Select(l => _settings.StartGameCountdown - l)
                                   .Subscribe(counter =>
                                   {
                                       _signalBus.Fire(new CountDownSignal
                                       {
                                           Countdown = (int)counter
                                       });
                                       if (counter <= 0)
                                       {
                                           // ReSharper disable once AccessToModifiedClosure
                                           _disposables.Remove(disposable);
                                           // ReSharper disable once AccessToModifiedClosure
                                           disposable?.Dispose();
                                           disposable = null;
                                           FireStart();
                                       }
                                   }).AddTo(_disposables);
        }

        private void FireStart()
        {
            _signalBus.Fire(new GameStartedSignal
            {
                GameModel = _gameModel
            });
        }

        public void Dispose()
        {
            _signalBus.Unsubscribe<StartGameSignal>(OnStartGame);
            _signalBus.Unsubscribe<RestartGameSignal>(OnRestartGame);
            _signalBus.Unsubscribe<CharacterOnAboardSignal>(OnCharacterOnAboard);
            _signalBus.Unsubscribe<LevelLoadedSignal>(OnLevelLoaded);
            _disposables.Dispose();
        }

        private void OnLevelLoaded()
        {
            StartGameCountdown();
        }
    }
}
