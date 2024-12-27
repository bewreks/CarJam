using CarJam.Scripts.Characters.Presenters;
using CarJam.Scripts.Queues.Base;
using Cysharp.Threading.Tasks;
using UnityEngine;
namespace CarJam.Scripts.Queues.Characters
{
    public class CharactersQueue : BaseQueue<CharacterPresenter>
    {
        public CharactersQueue(Vector3 startPoint, Vector3 finishPoint, float distanceBetweenVehicles) : base(startPoint, finishPoint, distanceBetweenVehicles)
        {
        }

        public override bool IsCanDequeue => _objects.Count > 0 && !_objects[0].IsMoving;

        public override bool UpdateInProgress { get; protected set; }

        public override void Dispose()
        {
            _objects.ForEach(presenter => presenter.Dispose());
        }

        public override UniTask UpdateQueue()
        {
            UpdateInProgress = true;
            _currentPosition = _finishPoint;
            foreach (var character in _objects)
            {
                character.MoveToPosition(_currentPosition).Forget();
                _currentPosition += _queueDirection;
            }
            
            UpdateInProgress = false;
            return UniTask.CompletedTask; 
        }

        async protected override UniTask BeforeEnqueue(CharacterPresenter t, Vector3 position)
        {
            await t.MoveToPosition(_startPoint);
        }

        async protected override UniTask AfterEnqueue(CharacterPresenter t, Vector3 position)
        {
            await t.MoveToPosition(position);
        }
    }
}
