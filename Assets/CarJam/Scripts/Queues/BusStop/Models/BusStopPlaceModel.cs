using CarJam.Scripts.CarJam;
using CarJam.Scripts.Vehicles.Presenters;
using Zenject;
namespace CarJam.Scripts.Queues.BusStop.Models
{
    public class BusStopPlaceModel
    {
        public VehiclePresenter Vehicle;
        public GameColors Color = GameColors.None;
        public bool Reserved;

        public class Factory : PlaceholderFactory<BusStopPlaceModel>
        {
        }
    }
}
