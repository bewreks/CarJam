using CarJam.Scripts.Queues.BusStop.Models;
using CarJam.Scripts.Queues.BusStop.Views;
using UnityEngine;
using Zenject;
namespace CarJam.Scripts.Queues.BusStop.Presenters
{
    public class BusStopPlacePresenter
    {
        [Inject] private BusStopPlaceModel.Factory _modelFactory;
        [Inject] private BusStopPlaceView.Factory _viewFactory;
        
        private BusStopPlaceModel _model;
        private BusStopPlaceView _view;

        [Inject]
        private void Construct()
        {
            _model = _modelFactory.Create();
            _view = _viewFactory.Create(_model);
        }

        public class Factory : PlaceholderFactory<BusStopPlacePresenter>
        {
        }

        public void PlaceToPosition(Vector3 position)
        {
            _view.transform.position = position;
        }

        public void SetParent(Transform parent)
        {
            _view.transform.SetParent(parent, true);
        }
    }
}
