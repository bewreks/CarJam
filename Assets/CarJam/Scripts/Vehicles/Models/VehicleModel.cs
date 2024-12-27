using CarJam.Scripts.CarJam;
using UniRx;
using UnityEngine;
using Zenject;
namespace CarJam.Scripts.Vehicles.Models
{
    public class VehicleModel
    {
        public GameColors Color;
        public float MovementSpeed;
        public ReactiveProperty<Material> Material = new ReactiveProperty<Material>();

        public class Factory : PlaceholderFactory<VehicleModel>
        {
            
        }
    }
}
