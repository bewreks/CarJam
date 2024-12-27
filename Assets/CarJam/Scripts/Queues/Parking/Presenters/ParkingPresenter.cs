using CarJam.Scripts.Queues.Parking.Models;
using Zenject;
namespace CarJam.Scripts.Queues.Parking.Presenters
{
    public class ParkingPresenter
    {
        [Inject] private ParkingModel.Factory _modelFactory;
        private ParkingModel _model;

        [Inject]
        private void Construct()
        {
            _model = _modelFactory.Create();
        }

        public class Factory : PlaceholderFactory<ParkingPresenter>
        {
        }
    }
}
