﻿using System;
using CarJam.Scripts.CarJam;
using UnityEngine;
namespace CarJam.Scripts.Vehicles.Data
{
    [Serializable]
    public class VehiclesData
    {
        public Vector3 Position;
        public Vector3 Direction;
        public VehicleType Type;
        public GameColors Color;
    }
    
    public enum VehicleType
    {
        Car,
        Bus
    } 
}