using System;
using System.Collections.Generic;
using CarJam.Scripts.Characters.Presenters;
using CarJam.Scripts.Characters.Views;
using Cysharp.Threading.Tasks;
using UnityEngine;
namespace CarJam.Scripts.Queues
{
    public class CharactersQueue : IDisposable
    {
        private readonly Vector3 _startPoint;
        private readonly Vector3 _finishPoint;
        
        private Vector3 _currentPosition;
        private Vector3 _queueDirection;
        private float _distanceBetweenCharacters;
        
        private List<CharacterPresenter> _characters = new List<CharacterPresenter>();

        public CharactersQueue(Vector3 startPoint, Vector3 finishPoint, float distanceBetweenCharacters)
        {
            _distanceBetweenCharacters = distanceBetweenCharacters;
            _startPoint = startPoint;
            _finishPoint = finishPoint;
            
            _currentPosition = _finishPoint;
            _queueDirection = _distanceBetweenCharacters * (_startPoint - _finishPoint).normalized;
        }

        public bool IsHaveEnoughSpace => Vector3.Distance(_currentPosition, _startPoint) > _distanceBetweenCharacters;

        public bool IsCanDequeue => _characters.Count > 0 && !_characters[0].IsMoving;

        public bool UpdateInProgress { get; private set; }

        public void Dispose()
        {
            
        }

        public async UniTask Enqueue(CharacterPresenter character)
        {
            _characters.Add(character);
            var nextPosition = _currentPosition + _queueDirection;
            _currentPosition = nextPosition;
            await character.MoveToPosition(_startPoint);
            await character.MoveToPosition(nextPosition);
        }

        public CharacterPresenter Dequeue()
        {
            var character = _characters[0];
            _characters.RemoveAt(0);
            return character;
        }

        public async UniTask UpdateQueue()
        {
            UpdateInProgress = true;
            _currentPosition = _finishPoint;
            var tasks = _characters.Select(view =>
            {
                var task = view.MoveToPosition(_currentPosition);
                _currentPosition += _queueDirection;
                return task;
            });

            await UniTask.WhenAll(tasks);
            UpdateInProgress = false;
        }
    }
}
