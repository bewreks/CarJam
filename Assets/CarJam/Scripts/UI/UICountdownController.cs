using System;
using CarJam.Scripts.Signals;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using TMPro;
using UnityEngine;
using Zenject;
namespace CarJam.Scripts.UI
{
    public class UICountdownController : IDisposable
    {
        [Inject] private SignalBus _signalBus;
        
        private Canvas _canvas;
        private TMP_Text _countdownText;

        [Inject]
        private void Construct(Canvas canvas, TMP_Text countdownText)
        {
            _countdownText = countdownText;
            _canvas = canvas;

            _canvas.gameObject.SetActive(false);
            _countdownText.alpha = 0;

            _signalBus.Subscribe<StartGameSignal>(OnStarGame);
            _signalBus.Subscribe<RestartGameSignal>(OnStarGame);
            _signalBus.Subscribe<CountDownSignal>(OnCountDown);
        }

        private void OnStarGame()
        {
            _canvas.gameObject.SetActive(true);
        }

        private async void OnCountDown(CountDownSignal signal)
        {
            _countdownText.alpha = 1;
            _countdownText.text = $"Game starts in {signal.Countdown} seconds!";
            await UniTask.Delay(300);
            await _countdownText.DOFade(0, 0.5f);
            if (signal.Countdown == 0)
            {
                _canvas.gameObject.SetActive(false);
            }
        }

        public void Dispose()
        {
            _signalBus.TryUnsubscribe<StartGameSignal>(OnStarGame);
            _signalBus.TryUnsubscribe<RestartGameSignal>(OnStarGame);
            _signalBus.TryUnsubscribe<CountDownSignal>(OnCountDown);
        }
    }
}
