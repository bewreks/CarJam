using UniRx;
namespace CarJam.Scripts.CarJam
{
    public class GameModel
    {
        public float CharacterSpawnCooldown;
        public float CharacterDespawnCooldown;
        public Level CurrentLevel;
        public IntReactiveProperty Score;
        public BoolReactiveProperty IsCharactersQueueWaiting;
        public BoolReactiveProperty IsBusStopsQueueFull;
    }
}
