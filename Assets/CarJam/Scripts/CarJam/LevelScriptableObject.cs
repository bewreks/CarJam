using System.Collections.Generic;
using CarJam.Scripts.Data;
using CarJam.Scripts.Data.Settings;
using CarJam.Scripts.Vehicles;
using UnityEngine;
using Zenject;
namespace CarJam.Scripts.CarJam
{
    [CreateAssetMenu(fileName = "Level", menuName = "CarJam/Level", order = 0)]
    public class LevelScriptableObject : ScriptableObjectInstaller
    {
        [SerializeField] private VehiclesData[] _vehicles;
        [SerializeField] private GameColors[] _usedColors;

        public override void InstallBindings()
        {
            Container.BindInstance(this).AsTransient();
        }

        public Level CreateLevel(params VehicleSettings[] vehicleSettings)
        {
            var vehicles = new VehiclesData[_vehicles.Length];
            for (var i = 0; i < _vehicles.Length; i++)
            {
                vehicles[i] = _vehicles[i].Clone();
            }
            
            var level = new Level(vehicles, _usedColors, vehicleSettings);
            return level;
        }
    }
}
