using System;
using CarJam.Scripts.Signals;
using TMPro;
using UnityEngine;
using Zenject;
namespace CarJam.Scripts.UI
{
    public class UIScoreController : IDisposable
    {

        [Inject] private SignalBus _signalBus;
        private Canvas _canvas;
        private TMP_Text _scoreText;

        [Inject]
        private void Construct(Canvas canvas, TMP_Text scoreText)
        {
            _scoreText = scoreText;
            _canvas = canvas;
            
            _canvas.gameObject.SetActive(false);
            
            
            _signalBus.Subscribe<ScoreUpdateSignal>(OnScoreUpdated);
            _signalBus.Subscribe<GameStartedSignal>(OnStartGame);
        }

        private void OnStartGame()
        {
            _canvas.gameObject.SetActive(true);
        }

        private void OnScoreUpdated(ScoreUpdateSignal signal)
        {
            _scoreText.text = $"Score: {signal.Score}";
        }
        
        public void Dispose()
        {
            _signalBus.Unsubscribe<GameStartedSignal>(OnStartGame);
            _signalBus.Unsubscribe<ScoreUpdateSignal>(OnScoreUpdated);
        }
    }
}
