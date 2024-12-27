using CarJam.Scripts.Queues.BusStop.Models;
using UnityEngine;
using Zenject;
namespace CarJam.Scripts.Queues.BusStop.Views
{
    public class BusStopPlaceView : MonoBehaviour
    {
        [field: SerializeField] public Transform EnterPoint { get; private set; }
        
        private BusStopPlaceModel _model;

        [Inject]
        private void Construct(BusStopPlaceModel model)
        {
            _model = model;

        }

        public class Factory : PlaceholderFactory<BusStopPlaceModel, BusStopPlaceView>
        {
        }
    }
}
