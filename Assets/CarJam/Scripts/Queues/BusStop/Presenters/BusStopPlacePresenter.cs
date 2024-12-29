using System;
using CarJam.Scripts.CarJam;
using CarJam.Scripts.Queues.BusStop.Models;
using CarJam.Scripts.Queues.BusStop.Views;
using CarJam.Scripts.Vehicles.Presenters;
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

        public bool IsEmpty => _model.Vehicle == Guid.Empty && !_model.Reserved;

        public Vector3 EnterPoint => _view.EnterPoint.position;

        public Vector3 Position => _view.transform.position;

        public Guid Id => _model.Id;

        [Inject]
        private void Construct()
        {
            _model = _modelFactory.Create();
            _model.Id = Guid.NewGuid();
            _view = _viewFactory.Create(_model);
        }

        public void PlaceToPosition(Vector3 position)
        {
            _view.transform.position = position;
        }

        public void SetParent(Transform parent)
        {
            _view.transform.SetParent(parent, true);
        }

        public void SetVehicle(Guid vehicle, GameColors color)
        {
            _model.Vehicle = vehicle;
            _model.Color = color;
        }

        public void Reserve()
        {
            _model.Reserved = true;
        }

        public void Unreserve()
        {
            _model.Reserved = false;
        }

        public class Factory : PlaceholderFactory<BusStopPlacePresenter>
        {
        }
    }
}
