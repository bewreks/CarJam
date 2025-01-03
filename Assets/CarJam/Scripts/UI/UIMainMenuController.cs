using System;
using CarJam.Scripts.CarJam;
using CarJam.Scripts.Signals;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;
namespace CarJam.Scripts.UI
{
    public class UIMainMenuController : IDisposable
    {
        [Inject] private SignalBus _signalBus;
        
        private Canvas _canvas;
        private Button _startButton;
        private Button _restartButton;
        private TMP_Text _message;
        private GameModel _gameModel;


        [Inject]
        private void Construct(Canvas canvas, Button startButton, Button restartButton, TMP_Text message)
        {
            _message = message;
            _restartButton = restartButton;
            _startButton = startButton;
            _canvas = canvas;
            
            _startButton.onClick.AddListener(OnStartClicked);
            _restartButton.onClick.AddListener(OnRestartClicked);
            
            _startButton.gameObject.SetActive(true);
            _restartButton.gameObject.SetActive(false);
            _message.gameObject.SetActive(false);
            
            _canvas.gameObject.SetActive(true);
            
            _signalBus.Subscribe<GameStartedSignal>(OnGameStarted);
            _signalBus.Subscribe<LevelClearedSignal>(OnLevelCleared);
        }

        private void OnGameStarted(GameStartedSignal signal)
        {
            _gameModel = signal.GameModel;
        }

        private void OnLevelCleared()
        {
            _startButton.gameObject.SetActive(false);
            _restartButton.gameObject.SetActive(true);
            _message.text = $"Level cleared! \r\n Score: {_gameModel.Score}";
            _message.gameObject.SetActive(true);
            
            _canvas.gameObject.SetActive(true);
        }

        private void OnRestartClicked()
        {
            _signalBus.Fire<RestartGameSignal>();
            
            _canvas.gameObject.SetActive(false);
        }

        private void OnStartClicked()
        {
            _signalBus.Fire<StartGameSignal>();
            
            _canvas.gameObject.SetActive(false);
        }

        public void Dispose()
        {
            _restartButton.onClick.RemoveAllListeners();
            _startButton.onClick.RemoveAllListeners();
        }
    }
}
