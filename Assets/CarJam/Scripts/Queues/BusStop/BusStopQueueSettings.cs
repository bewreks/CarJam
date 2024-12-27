using System;
using UnityEngine;
namespace CarJam.Scripts.Queues.BusStop
{
    [Serializable]
    public class BusStopQueueSettings
    {
        [field: SerializeField] public float DistanceBetweenVehicles { get; private set; }
    }
}
