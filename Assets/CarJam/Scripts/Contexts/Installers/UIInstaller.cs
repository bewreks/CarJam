using CarJam.Scripts.UI;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;
namespace CarJam.Scripts.Contexts.Installers
{
    public class UIInstaller : MonoInstaller
    {
        
        [field: Header("UI Score")]
        [field: SerializeField] public Canvas ScoreCanvas { get; private set; }
        [field: SerializeField] public TMP_Text ScoreText { get; private set; }
        
        [field: Header("UI Main Menu")]
        [field: SerializeField] public Canvas MainMenuCanvas { get; private set; }
        [field: SerializeField] public Button StartButton { get; private set; }
        [field: SerializeField] public Button RestartButton { get; private set; }
        [field: SerializeField] public TMP_Text MessageText { get; private set; }
        
        [field: Header("UI Countdown")]
        [field: SerializeField] public Canvas CountdownCanvas { get; private set; }
        [field: SerializeField] public TMP_Text CountdownText { get; private set; }
        
        [Inject] private SignalBus _signalBus;

        public override void InstallBindings()
        {
            
        }

        [Inject]
        private void Construct()
        {
            Container.BindInterfacesAndSelfTo<UIScoreController>().AsSingle()
                     .WithArguments(ScoreCanvas, ScoreText);
            
            Container.BindInterfacesAndSelfTo<UIMainMenuController>().AsSingle()
                     .WithArguments(MainMenuCanvas, StartButton, RestartButton, MessageText);

            Container.BindInterfacesAndSelfTo<UICountdownController>().AsSingle()
                     .WithArguments(CountdownCanvas, CountdownText);
        }
    }
}
