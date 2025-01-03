﻿using CarJam.Scripts.CarJam;
using CarJam.Scripts.Input;
using CarJam.Scripts.Parking;
using CarJam.Scripts.Queues;
using CarJam.Scripts.Queues.BusStop;
using CarJam.Scripts.Queues.Characters;
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
        [SerializeField] private Transform _rbPoint, _ltPoint, _outPoint;
        
        public override void InstallBindings()
        {
            Container.BindInstance(_camera).AsSingle();
            
            Container.BindInterfacesAndSelfTo<CharactersQueueFacade>().AsSingle()
                     .WithArguments(_charactersQueue.Start.position, _charactersQueue.End.position, _characterSpawnPoint.position).NonLazy();
            
            Container.BindInterfacesAndSelfTo<BusStopFacade>().AsSingle()
                     .WithArguments(_parkingQueue.Start.position, _parkingQueue.End.position, _parkingQueue.transform).NonLazy();

            Container.BindInterfacesAndSelfTo<ParkingFacade>().AsSingle()
                     .WithArguments(_rbPoint.position, _ltPoint.position, _outPoint.position).NonLazy();
            
            Container.BindInterfacesAndSelfTo<UserInputService>().AsSingle().NonLazy();
            Container.BindInterfacesAndSelfTo<CarJamGameplay>().AsSingle();
        }
    }

}
