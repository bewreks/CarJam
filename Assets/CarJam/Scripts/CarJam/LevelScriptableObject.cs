using System.Collections.Generic;
using CarJam.Scripts.Vehicles;
using CarJam.Scripts.Vehicles.Data;
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
            var level = new Level(_vehicles, _usedColors, vehicleSettings);
            return level;
        }
    }
}
