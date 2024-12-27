using CarJam.Scripts.Vehicles.Models;
using UnityEngine;
using Zenject;
namespace CarJam.Scripts.Vehicles.Views
{
    public class VehicleView : MonoBehaviour
    {
        private VehicleModel _model;

        [Inject]
        private void Construct(VehicleModel model)
        {
            _model = model;
        }

        public class Factory : PlaceholderFactory<VehicleModel, VehicleView>
        {
        }
    }
}
