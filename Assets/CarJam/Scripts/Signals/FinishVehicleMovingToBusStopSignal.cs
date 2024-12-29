using System;
using CarJam.Scripts.CarJam;
namespace CarJam.Scripts.Signals
{
    public struct FinishVehicleMovingToBusStopSignal
    {
        public Guid VehicleId;
        public Guid BusStopId;
        public GameColors Color;
    }
}
