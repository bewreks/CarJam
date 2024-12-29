using System;
using CarJam.Scripts.CarJam;
namespace CarJam.Scripts.Signals
{
    public struct VehicleMoveOutBusStopSignal
    {
        public Guid VehicleId;
        public Guid BusStopId;
        public GameColors Color;
    }
}
