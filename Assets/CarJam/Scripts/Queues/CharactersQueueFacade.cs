using System;
using System.Collections.Generic;
using CarJam.Scripts.CarJam;
using CarJam.Scripts.Characters.Factories;
using CarJam.Scripts.Characters.Views;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Zenject;
namespace CarJam.Scripts.Queues
{
    public class CharactersQueueFacade : IInitializable, IDisposable
    {
        [Inject] private QueueSettings _settings;
        [Inject] private CharacterFactory _characterFactory;

        private readonly Vector3 _startPoint;
        private readonly Vector3 _finishPoint;
        private readonly Vector3 _characterSpawnPoint;
        
        private CharactersQueue _charactersQueue;

        public CharactersQueueFacade(Vector3 startPoint, Vector3 finishPoint, Vector3 characterSpawnPoint)
        {
            _startPoint = startPoint;
            _finishPoint = finishPoint;
            _characterSpawnPoint = characterSpawnPoint;
        }

        public void Initialize()
        {
            _charactersQueue = new CharactersQueue(_startPoint, _finishPoint, _settings.DistanceBetweenCharacters);
        }

        public void Dispose()
        {
            
        }

        public async UniTask EnqueueCharacter()
        {
            if (!_charactersQueue.IsHaveEnoughSpace ||
                _charactersQueue.UpdateInProgress) return;

            var characterView = _characterFactory.Create(GameColors.Blue);
            characterView.transform.position = _characterSpawnPoint;
            await _charactersQueue.Enqueue(characterView);
        }
        
        public void DequeueCharacter()
        {
            if (!_charactersQueue.IsCanDequeue ||
                _charactersQueue.UpdateInProgress) return;
            
            var character = _charactersQueue.Dequeue();
            character.DestroySelf();
            _charactersQueue.UpdateQueue().Forget();
        }
    }
}
