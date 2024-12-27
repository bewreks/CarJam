using System;
using CarJam.Scripts.CarJam;
using CarJam.Scripts.Queues;
using CarJam.Scripts.Queues.BusStop;
using CarJam.Scripts.Queues.Characters;
using CarJam.Scripts.Signals;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.InputSystem;
using Zenject;
namespace CarJam.Scripts.Input
{
    public class UserInputService : IInitializable, IDisposable
    {
        [Inject] private LazyInject<CharactersQueueFacade> _charactersQueue;
        [Inject] private LazyInject<BusStopFacade> _parkingQueue;
        [Inject] private SignalBus _signalBus;
        
        private readonly InputActions _inputActions = new InputActions();

        public void Initialize()
        {
            _inputActions.Enable();
            _inputActions.GamePlay.Select.performed += OnSelect;
            _inputActions.GamePlay.DebugSpawn.performed += OnDebugSpawn;
            _inputActions.GamePlay.DebugDespawn.performed += OnDebugDespawn;
            _inputActions.GamePlay.DebugSpawnBus.performed += OnDebugSpawnBus;
            _inputActions.GamePlay.DebugDespawnBus.performed += OnDebugDespawnBus;
        }

        private void OnSelect(InputAction.CallbackContext obj)
        {
            _signalBus.Fire(new DebugPlaceBusSignal//UserSelectionSignal
            {
                SelectionPosition = _inputActions.GamePlay.SelectPosition.ReadValue<Vector2>()
            });
        }

        private void OnDebugSpawn(InputAction.CallbackContext obj)
        {
            _charactersQueue.Value.Enqueue(GameColors.Blue).Forget();
        }

        private void OnDebugDespawn(InputAction.CallbackContext obj)
        {
            _charactersQueue.Value.Dequeue();
        }

        private void OnDebugSpawnBus(InputAction.CallbackContext obj)
        {
            _parkingQueue.Value.Enqueue(GameColors.Blue).Forget();
        }

        private void OnDebugDespawnBus(InputAction.CallbackContext obj)
        {
            _parkingQueue.Value.Dequeue();
        }

        public void Dispose()
        {
            _inputActions.GamePlay.Select.performed -= OnSelect;
            _inputActions.GamePlay.DebugSpawn.performed -= OnDebugSpawn;
            _inputActions.GamePlay.DebugDespawn.performed -= OnDebugDespawn;
            _inputActions.GamePlay.DebugSpawnBus.performed -= OnDebugSpawnBus;
            _inputActions.GamePlay.DebugDespawnBus.performed -= OnDebugDespawnBus;
            _inputActions.Dispose();
        }
    }
}
