using CarJam.Scripts.Input;
using CarJam.Scripts.Queues;
using UnityEngine;
using Zenject;
namespace CarJam.Scripts.Contexts.Installers
{
    public class CarJamGameContext : MonoInstaller
    {
        [SerializeField] private Transform _characterSpawnPoint;
        [SerializeField] private Transform _startPoint;
        [SerializeField] private Transform _finishPoint;
        
        public override void InstallBindings()
        {
            Container.BindInterfacesAndSelfTo<CharactersQueueFacade>().AsSingle()
                     .WithArguments(_startPoint.position, _finishPoint.position, _characterSpawnPoint.position).NonLazy();
            
            Container.BindInterfacesAndSelfTo<UserInputService>().AsSingle();
        }
    }

}
