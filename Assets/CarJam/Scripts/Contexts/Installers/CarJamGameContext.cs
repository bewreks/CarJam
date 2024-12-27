using CarJam.Scripts.CarJam;
using CarJam.Scripts.Input;
using CarJam.Scripts.Queues;
using UnityEngine;
using Zenject;
namespace CarJam.Scripts.Contexts.Installers
{
    public class CarJamGameContext : MonoInstaller
    {
        [SerializeField] private Transform _characterSpawnPoint;
        [SerializeField] private QueueView _charactersQueue;
        [SerializeField] private QueueView _parkingQueue;
        
        public override void InstallBindings()
        {
            Container.BindInterfacesAndSelfTo<CharactersQueueFacade>().AsSingle()
                     .WithArguments(_charactersQueue.Start.position, _charactersQueue.End.position, _characterSpawnPoint.position).NonLazy();
            
            
            Container.BindInterfacesAndSelfTo<UserInputService>().AsSingle();
            Container.BindInterfacesAndSelfTo<CarJamGameplay>().AsSingle();
        }
    }

}
