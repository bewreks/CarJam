using CarJam.Scripts.CarJam;
using CarJam.Scripts.Characters.Presenters;
using UnityEngine;
using Zenject;
namespace CarJam.Scripts.Queues
{
    public class CharactersQueueFacade : BaseQueueFacade<CharacterPresenter, CharactersQueue>
    {
        [Inject] private CharactersQueueSettings _settings;
        [Inject] private CharacterPresenter.Factory _characterFactory;

        public CharactersQueueFacade(Vector3 startPoint, Vector3 finishPoint, Vector3 characterSpawnPoint) : base(startPoint, finishPoint, characterSpawnPoint)
        {
        }

        protected override float DistanceBetweenObject => _settings.DistanceBetweenCharacters;

        protected override CharacterPresenter TFactory(GameColors color)
        {
            return _characterFactory.Create(color, _spawnPoint);
        }

        protected override void OnDequeue(CharacterPresenter obj)
        {
            obj.DestroySelf();
        }
    }

}
