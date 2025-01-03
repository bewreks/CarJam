using System.Collections.Generic;
using System.Threading;
using CarJam.Scripts.Characters.Presenters;
using CarJam.Scripts.Queues.Base;
using Cysharp.Threading.Tasks;
using UnityEngine;
namespace CarJam.Scripts.Queues.Characters
{
    public class CharactersQueue : BaseQueue<CharacterPresenter>
    {
        private List<CharacterPresenter> _precachedObjects = new List<CharacterPresenter>();
        
        public CharactersQueue(Vector3 startPoint, Vector3 finishPoint, float distanceBetweenVehicles) : base(startPoint, finishPoint, distanceBetweenVehicles)
        {
        }

        public override bool IsCanDequeue => _objects.Count > 0 && !_objects[0].IsMoving;

        public override bool UpdateInProgress { get; protected set; }

        public int Count => _objects.Count;

        public override void Dispose()
        {
            _precachedObjects.ForEach(presenter => presenter.DestroySelf());
            _precachedObjects.Clear();
            
            _objects.ForEach(presenter => presenter.DestroySelf());
            Clear();
        }

        public override UniTask UpdateQueue(CancellationToken token)
        {
            UpdateInProgress = true;
            _currentPosition = _finishPoint;
            foreach (var character in _objects)
            {
                character.MoveToPosition(_currentPosition, token).Forget();
                _currentPosition += _queueDirection;
            }
            
            UpdateInProgress = false;
            return UniTask.CompletedTask; 
        }

        async protected override UniTask BeforeEnqueue(CharacterPresenter t, Vector3 position, CancellationToken token)
        {
            _precachedObjects.Add(t);
            await t.MoveToPosition(_startPoint, token);
            _precachedObjects.Remove(t);
        }

        async protected override UniTask AfterEnqueue(CharacterPresenter t, Vector3 position, CancellationToken token)
        {
            await t.MoveToPosition(position, token);
        }
    }
}
