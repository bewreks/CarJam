using System;
using CarJam.Scripts.CarJam;
using CarJam.Scripts.Queues;
using Cysharp.Threading.Tasks;
using UnityEngine.InputSystem;
using Zenject;
namespace CarJam.Scripts.Input
{
    public class UserInputService : IInitializable, IDisposable
    {
        [Inject] private LazyInject<CharactersQueueFacade> _charactersQueue;
        
        private readonly InputActions _inputActions = new InputActions();

        public void Initialize()
        {
            _inputActions.Enable();
            _inputActions.GamePlay.Select.performed += OnSelect;
            _inputActions.GamePlay.DebugSpawn.canceled += OnDebugSpawn;
            _inputActions.GamePlay.DebugDespawn.canceled += OnDebugDespawn;
        }

        private void OnSelect(InputAction.CallbackContext obj)
        {
            
        }

        private void OnDebugSpawn(InputAction.CallbackContext obj)
        {
            _charactersQueue.Value.Enqueue(GameColors.Blue).Forget();
        }

        private void OnDebugDespawn(InputAction.CallbackContext obj)
        {
            _charactersQueue.Value.DequeueCharacter();
        }

        public void Dispose()
        {
            _inputActions.GamePlay.Select.performed -= OnSelect;
            _inputActions.GamePlay.DebugSpawn.canceled -= OnDebugSpawn;
            _inputActions.GamePlay.DebugDespawn.canceled -= OnDebugDespawn;
            _inputActions.Dispose();
        }
    }
}
