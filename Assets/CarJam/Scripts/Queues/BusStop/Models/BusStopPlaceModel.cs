using CarJam.Scripts.Vehicles.Presenters;
using Zenject;
namespace CarJam.Scripts.Queues.BusStop.Models
{
    public class BusStopPlaceModel
    {
        public VehiclePresenter Vehicle;

        public class Factory : PlaceholderFactory<BusStopPlaceModel>
        {
        }
    }
}
