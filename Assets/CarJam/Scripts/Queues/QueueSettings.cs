using System;
using UnityEngine;
namespace CarJam.Scripts.Queues
{
    [Serializable]
    public class QueueSettings
    {
        [field: SerializeField] public float DistanceBetweenCharacters { get; private set; } 
    }
}
