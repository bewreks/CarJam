using System;
using System.Linq;
using CarJam.Scripts.Contexts.Installers;
using CarJam.Scripts.Data;
using CarJam.Scripts.Data.Settings;
using CarJam.Scripts.Parking;
using CarJam.Scripts.Queues.BusStop;
using CarJam.Scripts.Queues.Characters;
using CarJam.Scripts.Signals;
using CarJam.Scripts.Vehicles;
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
        
        private GameModel _gameModel;
        
        private IDisposable _levelFailedHandler;
        private IDisposable _onScoresChandelHandler;
        private IDisposable _countDownHandler;

        [Inject]
        public void Construct()
        {
            _signalBus.Subscribe<StartGameSignal>(OnStartGame);
            _signalBus.Subscribe<RestartGameSignal>(OnRestartGame);
            _signalBus.Subscribe<LevelLoadedSignal>(OnLevelLoaded);
            _signalBus.Subscribe<CharacterOnAboardSignal>(OnCharacterOnAboard);
            _signalBus.Subscribe<LevelClearedSignal>(OnLevelCleared);
        }

        private void OnLevelCleared()
        {
            _characters.Value.Dispose();
            _busStops.Value.Dispose();
            _parking.Value.Dispose();
            
            _signalBus.Fire(new GameEndedSignal
            {
                Score = _gameModel.Score.Value,
                IsWin = true
            });
        }

        private void OnRestartGame()
        {
            _gameModel.CurrentLevel = _level.CreateLevel(GetVehicleSettings());
            _gameModel.IsBusStopsQueueFull.Value = false;
            _gameModel.IsCharactersQueueWaiting.Value = false;
            
            _characters.Value.Restart();
            _busStops.Value.Restart();
            _parking.Value.Restart();
            
            _parking.Value.LoadLevel(_gameModel.CurrentLevel);
        }

        private void OnStartGame()
        {
            _gameModel = new GameModel
            {
                CharacterSpawnCooldown = _settings.CharacterSpawnCooldown,
                CharacterDespawnCooldown = _settings.CharacterDespawnCooldown,
                CurrentLevel = _level.CreateLevel(GetVehicleSettings()),
                Score = new IntReactiveProperty(),
                IsCharactersQueueWaiting = new BoolReactiveProperty(),
                IsBusStopsQueueFull = new BoolReactiveProperty()
            };

            _levelFailedHandler?.Dispose();
            _levelFailedHandler = new[]
            {
                _gameModel.IsCharactersQueueWaiting,
                _gameModel.IsBusStopsQueueFull
            }.CombineLatestValuesAreAllTrue().DistinctUntilChanged().Subscribe(OnLevelFailed);

            _onScoresChandelHandler = _gameModel.Score.Subscribe(OnScoreChanged);
            
            _characters.Value.Restart();
            _busStops.Value.Restart();
            _parking.Value.Restart();

            _parking.Value.LoadLevel(_gameModel.CurrentLevel);
        }

        private void OnLevelFailed(bool isFailed)
        {
            if (isFailed)
            {
                _characters.Value.Dispose();
                _busStops.Value.Dispose();
                _parking.Value.Dispose();
                
                _signalBus.Fire(new GameEndedSignal
                {
                    Score = _gameModel.Score.Value,
                    IsWin = false
                });
            }
        }

        private void OnScoreChanged(int score)
        {
            _signalBus.Fire(new ScoreUpdateSignal
            {
                Score = score
            });
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
            _gameModel.Score.Value += 1;
        }

        private void StartGameCountdown()
        {
            _signalBus.Fire(new CountDownSignal
            {
                Countdown = _settings.StartGameCountdown + 1
            });
            _countDownHandler = Observable.Interval(TimeSpan.FromSeconds(1))
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
                                                  _countDownHandler?.Dispose();
                                                  _countDownHandler = null;
                                                  FireStart();
                                              }
                                          });
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
            _signalBus.TryUnsubscribe<LevelClearedSignal>(OnLevelCleared);
            _signalBus.TryUnsubscribe<StartGameSignal>(OnStartGame);
            _signalBus.TryUnsubscribe<RestartGameSignal>(OnRestartGame);
            _signalBus.TryUnsubscribe<CharacterOnAboardSignal>(OnCharacterOnAboard);
            _signalBus.TryUnsubscribe<LevelLoadedSignal>(OnLevelLoaded);
            
            _levelFailedHandler?.Dispose();
            _onScoresChandelHandler?.Dispose();
            _countDownHandler?.Dispose();
        }

        private void OnLevelLoaded()
        {
            StartGameCountdown();
        }
    }
}
