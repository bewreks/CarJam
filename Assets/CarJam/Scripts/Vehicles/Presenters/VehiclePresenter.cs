using System.Threading;
using CarJam.Scripts.CarJam;
using CarJam.Scripts.Vehicles.Models;
using CarJam.Scripts.Vehicles.Views;
using Zenject;
namespace CarJam.Scripts.Vehicles.Presenters
{
    public class VehiclePresenter
    {
        [Inject] private VehicleSettings _settings;
        [Inject] private VehicleModel.Factory _modelFactory;
        [Inject] private VehicleView.Factory _viewFactory;
        
        private VehicleModel _model;
        private VehicleView _view;
        
        private CancellationTokenSource _movementCts;

        [Inject]
        private void Construct(GameColors color)
        {
            _model = _modelFactory.Create();
            _model.Color = color;
            _model.Material.Value = _settings.Materials[color];
            _model.MovementSpeed = _settings.MovementSpeed;

            _view = _viewFactory.Create(_model);
        }
        
        public class Factory : PlaceholderFactory<GameColors, VehiclePresenter>
        {
            
        }
    }
}
