using System;
using AYellowpaper.SerializedCollections;
using CarJam.Scripts.CarJam;
using UnityEngine;
namespace CarJam.Scripts.Vehicles
{
    [Serializable]
    public class VehicleSettings
    {
        [field: SerializeField] public float MovementSpeed { get; private set; }
        [field: SerializeField] public SerializedDictionary<GameColors, Material> Materials { get; private set; }
    }
}
