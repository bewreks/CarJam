using System;
using UnityEngine;
namespace CarJam.Scripts.CarJam
{
    [Serializable]
    public class CarJamSettings
    {
        [field: SerializeField] public GameColors[] InGameColors { get; private set; }
        [field: SerializeField] public float CharacterSpawnCooldown { get; private set; } //TODO fix low spawn speed
    }
}
