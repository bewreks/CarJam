﻿using AYellowpaper.SerializedCollections;
using UnityEngine;
namespace CarJam.Scripts.Data.Settings
{
    [CreateAssetMenu(fileName = "VehicleSettings", menuName = "CarJam/Settings/Vehicle", order = 0)]
    public class VehicleSettings : ScriptableObject
    {
        [field: SerializeField] public VehicleType Type { get; private set; }
        [field: SerializeField] public float RotationSpeed { get; private set; }
        [field: SerializeField] public float MovementSpeed { get; private set; }
        [field: SerializeField] public int Capacity { get; private set; }
        [field: SerializeField] public SerializedDictionary<GameColors, Material> Materials { get; private set; }
    }
}
