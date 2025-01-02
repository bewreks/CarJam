using System;
using CarJam.Scripts.CarJam;
using CarJam.Scripts.Queues;
using CarJam.Scripts.Queues.BusStop;
using CarJam.Scripts.Queues.Characters;
using CarJam.Scripts.Signals;
using CarJam.Scripts.Vehicles.Views;
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
        private Camera _camera;
        
        private int _vehiclesMask;

        [Inject]
        private void Construct(Camera camera)
        {
            _camera = camera;
            _vehiclesMask = 1 << 6;
            _signalBus.Subscribe<GameStartedSignal>(OnStartGame);
            _signalBus.Subscribe<LevelClearedSignal>(OnLevelCleared);
        }

        private void OnLevelCleared()
        {
            _inputActions.Disable();
        }

        private void OnStartGame()
        {
            _inputActions.Enable();
        }

        public void Initialize()
        {
            _inputActions.GamePlay.Select.performed += OnSelect;
            _inputActions.GamePlay.DebugSpawn.performed += OnDebugSpawn;
            _inputActions.GamePlay.DebugDespawn.performed += OnDebugDespawn;
            _inputActions.GamePlay.DebugSpawnBus.performed += OnDebugSpawnBus;
            _inputActions.GamePlay.DebugDespawnBus.performed += OnDebugDespawnBus;
        }

        private void OnSelect(InputAction.CallbackContext obj)
        {
            var screenPoint = _inputActions.GamePlay.SelectPosition.ReadValue<Vector2>();
            var ray = _camera.ScreenPointToRay(screenPoint);
            if (Physics.Raycast(ray, out var hit, _camera.farClipPlane, _vehiclesMask))
            {
                var vehicleView = hit.transform.GetComponent<VehicleView>();
                _signalBus.Fire(new UserSelectionSignal
                {
                    VehicleId = vehicleView.Id
                });
            }
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
            _inputActions.Dispose();
            _signalBus.Unsubscribe<GameStartedSignal>(OnStartGame);
            _inputActions.GamePlay.Select.performed -= OnSelect;
            _inputActions.GamePlay.DebugSpawn.performed -= OnDebugSpawn;
            _inputActions.GamePlay.DebugDespawn.performed -= OnDebugDespawn;
            _inputActions.GamePlay.DebugSpawnBus.performed -= OnDebugSpawnBus;
            _inputActions.GamePlay.DebugDespawnBus.performed -= OnDebugDespawnBus;
        }
    }
}
