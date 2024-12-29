using System;
using AYellowpaper.SerializedCollections;
using CarJam.Scripts.CarJam;
using UnityEngine;
namespace CarJam.Scripts.Vehicles
{
    [CreateAssetMenu(fileName = "VehicleSettings", menuName = "CarJam/Settings/Vehicle", order = 0)]
    public class VehicleSettings : ScriptableObject
    {
        [field: SerializeField] public float MovementSpeed { get; private set; }
        [field: SerializeField] public int Capacity { get; private set; }
        [field: SerializeField] public SerializedDictionary<GameColors, Material> Materials { get; private set; }
    }
}
