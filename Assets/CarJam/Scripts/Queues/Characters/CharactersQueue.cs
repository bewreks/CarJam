using CarJam.Scripts.Characters.Presenters;
using Cysharp.Threading.Tasks;
using UnityEngine;
namespace CarJam.Scripts.Queues
{
    public class CharactersQueue : BaseQueue<CharacterPresenter>
    {
        public CharactersQueue(Vector3 startPoint, Vector3 finishPoint, float distanceBetweenCharacters) : base(startPoint, finishPoint, distanceBetweenCharacters)
        {
        }

        public override bool IsHaveEnoughSpace => Vector3.Distance(_currentPosition, _startPoint) > _distanceBetweenCharacters;

        public override bool IsCanDequeue => _characters.Count > 0 && !_characters[0].IsMoving;

        public override bool UpdateInProgress { get; protected set; }

        public override void Dispose()
        {
            _characters.ForEach(presenter => presenter.Dispose());
        }

        public override UniTask UpdateQueue()
        {
            UpdateInProgress = true;
            _currentPosition = _finishPoint;
            foreach (var character in _characters)
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
