using System;
using CarJam.Scripts.Signals;
using CarJam.Scripts.Vehicles.Views;
using UnityEngine;
using UnityEngine.InputSystem;
using Zenject;
namespace CarJam.Scripts.Input
{
    public class UserInputService : IInitializable, IDisposable
    {
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

        public void Dispose()
        {
            _inputActions.Dispose();
            _signalBus.Unsubscribe<GameStartedSignal>(OnStartGame);
            _inputActions.GamePlay.Select.performed -= OnSelect;
        }
    }
}
