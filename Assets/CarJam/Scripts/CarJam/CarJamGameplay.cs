using System;
using CarJam.Scripts.Queues.BusStop;
using CarJam.Scripts.Queues.Characters;
using CarJam.Scripts.Queues.Parking;
using CarJam.Scripts.Signals;
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
        
        private CompositeDisposable _disposables = new CompositeDisposable();

        [Inject]
        public void Construct()
        {
            _signalBus.Subscribe<LevelLoadedSignal>(OnLevelLoaded);
            _parking.Value.LoadLevel(_level);
        }

        private void StartGameCountdown()
        {
            IDisposable disposable = null;
            disposable = Observable.Interval( TimeSpan.FromSeconds(1))
                                   .TakeWhile(timer => timer <= 2)
                                   .Select(l => 2 - l)
                                   .Subscribe(_ =>
                                   {
                                       if (_ == 0)
                                       {
                                           // ReSharper disable once AccessToModifiedClosure
                                           _disposables.Remove(disposable);
                                           // ReSharper disable once AccessToModifiedClosure
                                           disposable?.Dispose();
                                           disposable = null;
                                           FireStart();
                                       }
                                       else
                                       {
                                           Debug.Log($"Start game countdown: {_}");
                                       }
                                   }).AddTo(_disposables);
        }

        private void FireStart()
        {
            var gameModel = new GameModel
            {
                CharacterSpawnCooldown = _settings.CharacterSpawnCooldown,
                InGameColors = _settings.InGameColors
            };
            _signalBus.Fire(new StartGameSignal
            {
                GameModel = gameModel
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
