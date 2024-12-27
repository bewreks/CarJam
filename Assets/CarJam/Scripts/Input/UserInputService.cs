using System;
using CarJam.Scripts.Queues;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Zenject;
namespace CarJam.Scripts.Input
{
    public class UserInputService : IInitializable, ITickable, IDisposable
    {
        [Inject] private LazyInject<CharactersQueueFacade> _charactersQueue;
        
        private readonly InputActions _inputActions = new InputActions();

        public void Initialize()
        {
            _inputActions.Enable();
        }

        public void Dispose()
        {
            _inputActions.Dispose();
        }

        public void Tick()
        {
            if (_inputActions.GamePlay.DebugSpawn.WasPerformedThisFrame())
            {
                _charactersQueue.Value.EnqueueCharacter().Forget();
            }
            if (_inputActions.GamePlay.DebugDespawn.WasPerformedThisFrame())
            {
                _charactersQueue.Value.DequeueCharacter();
            }
        }
    }
}
