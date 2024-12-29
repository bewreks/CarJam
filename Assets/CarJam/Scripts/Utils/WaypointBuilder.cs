using System;
using CarJam.Scripts.Queues.BusStop.Presenters;
using CarJam.Scripts.Queues.Parking.Models;
using CarJam.Scripts.Queues.Parking.Presenters;
using CarJam.Scripts.Vehicles.Presenters;
using UnityEngine;
namespace CarJam.Scripts.Utils
{
    public static class WaypointBuilder
    {
        public static Vector3[] BuildWaypoints(VehiclePresenter vehicle, ParkingPresenter parking, BusStopPlacePresenter busStop)
        {
            return BuildWaypoints(vehicle.Position, vehicle.Direction, parking, busStop);
        }
        
        public static Vector3[] BuildWaypoints(Vector3 position, Vector3 direction, ParkingPresenter parking, BusStopPlacePresenter busStop)
        {
            if (busStop == null) return Array.Empty<Vector3>();
            
            var way = GetPointsOnParking(position, direction, parking.Model);
            
            way[^1] = busStop.Position;
            way[^2] = busStop.EnterPoint;
            
            return way;
        }

        private static Vector3[] GetPointsOnParking(Vector3 position, Vector3 direction, ParkingModel parkingModel)
        {
            var ltNormalized = parkingModel.LtPoint - position;
            var rbNormalized = parkingModel.RbPoint - position;
            var rtNormalized = parkingModel.RtPoint - position;
            var lbNormalized = parkingModel.LbPoint - position;
            
            var ltDot = Vector3.Dot(direction, ltNormalized.normalized);
            var rtDot = Vector3.Dot(direction, rtNormalized.normalized);
            var rbDot = Vector3.Dot(direction, rbNormalized.normalized);
            var lbDot = Vector3.Dot(direction, lbNormalized.normalized);

            var planes = new Plane[2];

            if (Vector3.Dot(direction, Vector3.forward) >= 0)
            {
                planes[0] = ltDot > rtDot ? parkingModel.LeftPlane : parkingModel.RightPlane;
                planes[1] = parkingModel.TopPlane;
            }
            else
            {
                planes[0] = lbDot > rbDot ? parkingModel.LeftPlane : parkingModel.RightPlane;
                planes[1] = parkingModel.BottomPlane;
            }

            var ray = new Ray(position, direction);

            var minDistance = float.MaxValue;
            var normal = Vector3.zero;
            var hasHit = false;
            foreach (var plane in planes)
            {
                if (plane.Raycast(ray, out var distance) && distance < minDistance)
                {
                    normal = plane.normal;
                    minDistance = distance;
                    hasHit = true;
                }
            }

            Vector3[] way; 

            if (hasHit)
            {
                if (normal == Vector3.back)
                {
                    way = new Vector3[3];
                } else if (normal == Vector3.forward)
                {
                    way = new Vector3[5];

                    if (direction.x > 0)
                    {
                        way[1] = parkingModel.RbPoint;
                        way[2] = parkingModel.RtPoint;
                    }
                    else
                    {
                        way[1] = parkingModel.LbPoint;
                        way[2] = parkingModel.LtPoint;
                    }
                }
                else
                {
                    way = new Vector3[4];
                    if (direction.x > 0)
                    {
                        way[1] = parkingModel.RtPoint;
                    }
                    else
                    {
                        way[1] = parkingModel.LtPoint;
                    }
                }
                way[0] = ray.GetPoint(minDistance);
            }
            else
            {
                way = Array.Empty<Vector3>();
            }
            
            return way;
        }
    }
}
