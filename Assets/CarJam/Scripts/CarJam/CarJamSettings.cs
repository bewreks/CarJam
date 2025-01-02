using System;
using UnityEngine;
namespace CarJam.Scripts.CarJam
{
    [CreateAssetMenu(fileName = "CarJamSettings", menuName = "CarJam/Settings/CarJam", order = 0)]
    public class CarJamSettings : ScriptableObject
    {
        [field: SerializeField] public float CharacterSpawnCooldown { get; private set; }
        [field: SerializeField] public float CharacterDespawnCooldown { get; private set; }
    }
}
