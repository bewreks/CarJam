using System;
using UnityEngine;
namespace CarJam.Scripts.Queues.Parking
{
    [Serializable]
    public class ParkingQueueSettings
    {
        [field: SerializeField] public float DistanceBetweenVehicles { get; private set; }
    }
}
