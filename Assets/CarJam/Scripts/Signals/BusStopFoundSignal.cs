using System;
using UnityEngine;
namespace CarJam.Scripts.Signals
{
    public struct BusStopFoundSignal
    {
        public Guid BusStopId;
        public Guid VehicleId;
        public Vector3 Position;
        public Vector3 EnterPoint;
    }
}
