using System;
using UnityEngine;
namespace CarJam.Scripts.Queues.Characters
{
    [CreateAssetMenu(fileName = "CharacterQueueSettings", menuName = "CarJam/Settings/CharacterQueue", order = 0)]
    public class CharactersQueueSettings : ScriptableObject
    {
        [field: SerializeField] public float DistanceBetweenCharacters { get; private set; } 
    }
}
