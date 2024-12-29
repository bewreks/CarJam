using System.Collections.Generic;
using CarJam.Scripts.Vehicles.Data;
using UnityEngine;
namespace CarJam.Scripts.CarJam
{
    [CreateAssetMenu(fileName = "Level", menuName = "CarJam/Level", order = 0)]
    public class LevelScriptableObject : ScriptableObject
    {
        public List<VehiclesData> Vehicles = new List<VehiclesData>();
    }
}
