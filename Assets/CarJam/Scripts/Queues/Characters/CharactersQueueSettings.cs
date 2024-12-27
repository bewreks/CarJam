using System;
using UnityEngine;
namespace CarJam.Scripts.Queues
{
    [Serializable]
    public class CharactersQueueSettings
    {
        [field: SerializeField] public float DistanceBetweenCharacters { get; private set; } 
    }
}
