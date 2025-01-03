using UnityEngine;
namespace CarJam.Scripts.Data.Settings
{
    [CreateAssetMenu(fileName = "CharacterQueueSettings", menuName = "CarJam/Settings/CharacterQueue", order = 0)]
    public class CharactersQueueSettings : ScriptableObject
    {
        [field: SerializeField] public float DistanceBetweenCharacters { get; private set; } 
    }
}
