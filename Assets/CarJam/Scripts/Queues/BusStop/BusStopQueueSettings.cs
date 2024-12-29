using System;
using UnityEngine;
namespace CarJam.Scripts.Queues.BusStop
{
    [CreateAssetMenu(fileName = "BusStopQueueSettings", menuName = "CarJam/Settings/BusStopQueue")]
    public class BusStopQueueSettings : ScriptableObject
    {
        [field: SerializeField] public float DistanceBetweenVehicles { get; private set; }
    }
}
