using System;
using CarJam.Scripts.CarJam;
using Zenject;
namespace CarJam.Scripts.Queues.BusStop.Models
{
    public class BusStopPlaceModel
    {
        public Guid Vehicle = Guid.Empty;
        public GameColors Color = GameColors.None;
        public bool Reserved;
        public Guid Id;

        public class Factory : PlaceholderFactory<BusStopPlaceModel>
        {
        }

    }
}
