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
        [Inject] private LazyInject<CharactersQueueFacade> _charactersQueue;
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
            var settings = new VehicleSettings[Enum.GetNames(typeof(VehicleType)).Length];
            
            foreach (var type in Enum.GetValues(typeof(VehicleType)).Cast<VehicleType>())
            {
                settings[(int) type] = _container.ResolveId<VehicleSettings>(type);
            }
            
            _gameModel = new GameModel
            {
                CharacterSpawnCooldown = _settings.CharacterSpawnCooldown,
                CharacterDespawnCooldown = _settings.CharacterDespawnCooldown,
                CurrentLevel = _level.CreateLevel(settings)
            };
            _signalBus.Subscribe<LevelLoadedSignal>(OnLevelLoaded);
            _parking.Value.LoadLevel(_gameModel.CurrentLevel);
        }

        private void StartGameCountdown()
        {
            IDisposable disposable = null;
            disposable = Observable.Interval( TimeSpan.FromSeconds(1))
                                   .TakeWhile(timer => timer <= 2)
                                   .Select(l => 2 - l)
                                   .Subscribe(counter =>
                                   {
                                       Debug.Log($"Start game countdown: {counter}");
                                       if (counter == 0)
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
            _signalBus.Fire(new StartGameSignal
            {
                GameModel = _gameModel
            });
        }

        public void Dispose()
        {
            _signalBus.Unsubscribe<LevelLoadedSignal>(OnLevelLoaded);
            _disposables.Dispose();
        }

        private void OnLevelLoaded()
        {
            StartGameCountdown();
        }
    }
}
