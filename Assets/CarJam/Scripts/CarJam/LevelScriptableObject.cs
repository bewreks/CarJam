using System.Collections.Generic;
using CarJam.Scripts.Vehicles.Data;
using UnityEngine;
using Zenject;
namespace CarJam.Scripts.CarJam
{
    [CreateAssetMenu(fileName = "Level", menuName = "CarJam/Level", order = 0)]
    public class LevelScriptableObject : ScriptableObjectInstaller
    {
        public IReadOnlyList<VehiclesData> Vehicles => _vehicles;
        [SerializeField] private List<VehiclesData> _vehicles = new List<VehiclesData>();

        public override void InstallBindings()
        {
            Container.BindInstance(this).AsTransient();
        }
    }
}
