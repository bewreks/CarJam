using AYellowpaper.SerializedCollections;
using UnityEngine;
namespace CarJam.Scripts.Data.Settings
{
    [CreateAssetMenu(fileName = "CharacterSettings", menuName = "CarJam/Settings/Character", order = 0)]
    public class CharacterSettings : ScriptableObject
    {
        [field: SerializeField] public float MovementSpeed { get; private set; }
        [field: SerializeField] public SerializedDictionary<GameColors, Material> Materials { get; private set; }
    }
}
