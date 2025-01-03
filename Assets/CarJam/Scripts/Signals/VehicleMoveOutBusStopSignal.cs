using System;
using CarJam.Scripts.Data;
namespace CarJam.Scripts.Signals
{
    public struct VehicleMoveOutBusStopSignal
    {
        public Guid VehicleId;
        public Guid BusStopId;
        public GameColors Color;
    }
}
