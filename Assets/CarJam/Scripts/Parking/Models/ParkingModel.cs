using UnityEngine;
using Zenject;
namespace CarJam.Scripts.Parking.Models
{
    public class ParkingModel
    {
        public Vector3 LtPoint;
        public Vector3 RtPoint;
        public Vector3 RbPoint;
        public Vector3 LbPoint;

        public Plane LeftPlane;
        public Plane RightPlane;
        public Plane TopPlane;
        public Plane BottomPlane;

        public class Factory : PlaceholderFactory<ParkingModel>
        {
        }
    }
}
