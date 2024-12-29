using CarJam.Scripts.CarJam;
using CarJam.Scripts.Input;
using CarJam.Scripts.Queues;
using CarJam.Scripts.Queues.BusStop;
using CarJam.Scripts.Queues.Characters;
using CarJam.Scripts.Queues.Parking;
using UnityEngine;
using Zenject;
namespace CarJam.Scripts.Contexts.Installers
{
    public class CarJamGameContext : MonoInstaller
    {
        [SerializeField] private Camera _camera;
        [SerializeField] private Transform _characterSpawnPoint;
        [SerializeField] private QueueView _charactersQueue;
        [SerializeField] private QueueView _parkingQueue;
        [SerializeField] private Transform _rbPoint, _ltPoint;
        
        public override void InstallBindings()
        {
            Container.BindInterfacesAndSelfTo<CharactersQueueFacade>().AsSingle()
                     .WithArguments(_charactersQueue.Start.position, _charactersQueue.End.position, _characterSpawnPoint.position).NonLazy();
            
            Container.BindInterfacesAndSelfTo<BusStopFacade>().AsSingle()
                     .WithArguments(_parkingQueue.Start.position, _parkingQueue.End.position, _parkingQueue.transform).NonLazy();

            Container.BindInterfacesAndSelfTo<ParkingFacade>().AsSingle()
                     .WithArguments(_rbPoint.position, _ltPoint.position).NonLazy();
            
            Container.BindInterfacesAndSelfTo<UserInputService>().AsSingle()
                     .WithArguments(_camera).NonLazy();
            Container.BindInterfacesAndSelfTo<CarJamGameplay>().AsSingle();
        }
    }

}
