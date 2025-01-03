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
            
            _signalBus.Subscribe<GameEndedSignal>(OnGameEndedCleared);
        }

        private void OnGameEndedCleared(GameEndedSignal signal)
        {
            _startButton.gameObject.SetActive(!signal.IsWin);
            _restartButton.gameObject.SetActive(signal.IsWin);
            var result = signal.IsWin ? "won" : "Lost";
            _message.text = $"Game {result}! \r\n Score: {signal.Score}";
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
            _signalBus.TryUnsubscribe<GameEndedSignal>(OnGameEndedCleared);
            _restartButton.onClick.RemoveAllListeners();
            _startButton.onClick.RemoveAllListeners();
        }
    }
}
