using UnityEngine;
namespace CarJam.Scripts.Queues.Parking
{
    public class ParkingFacade
    {
        private readonly Vector3 _startPoint;
        private readonly Vector3 _finishPoint;
        private readonly Vector3 _spawnPoint;

        public ParkingFacade(Vector3 startPoint, Vector3 finishPoint, Vector3 spawnPoint)
        {
            _startPoint = startPoint;
            _finishPoint = finishPoint;
            _spawnPoint = spawnPoint;
        }
    }
}
