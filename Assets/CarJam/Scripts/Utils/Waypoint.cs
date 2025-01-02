using UnityEngine;
namespace CarJam.Scripts.Utils
{
    public struct Waypoint
    {
        public Vector3 Position;
        public bool Reverse;
        
        public Waypoint(Vector3 position)
        {
            Position = position;
            Reverse = false;
        }
        
        public Waypoint(Vector3 position, bool reverse = false)
        {
            Position = position;
            Reverse = reverse;
        }
    }
}
