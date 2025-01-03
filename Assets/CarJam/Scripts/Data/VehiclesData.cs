using System;
using UnityEngine;
namespace CarJam.Scripts.Data
{
    [Serializable]
    public class VehiclesData
    {
        public string Id;
        public Vector3 Position;
        public Vector3 Direction;
        public VehicleType Type;
        public GameColors Color;

        public VehiclesData Clone()
        {
            return (VehiclesData) MemberwiseClone();
        }
    }
    
    public enum VehicleType : byte
    {
        Car = 0,
        Bus
    } 
}
