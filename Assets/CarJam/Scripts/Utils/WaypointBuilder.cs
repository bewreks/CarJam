using System;
using CarJam.Scripts.Parking.Models;
using CarJam.Scripts.Parking.Presenters;
using CarJam.Scripts.Vehicles.Presenters;
using UnityEngine;
namespace CarJam.Scripts.Utils
{
    public static class WaypointBuilder
    {
        public static Waypoint[] BuildWaypoints(VehiclePresenter vehicle, ParkingPresenter parking, Vector3 busStopPosition, Vector3 busStopEnterPoint)
        {
            return BuildWaypoints(vehicle.Position, vehicle.Direction, parking, busStopPosition, busStopEnterPoint);
        }
        
        public static Waypoint[] BuildWaypoints(Vector3 position, Vector3 direction, ParkingPresenter parking, Vector3 busStopPosition, Vector3 busStopEnterPoint)
        {
            var way = GetPointsOnParking(position, direction, parking.Model);
            
            way[^1] = new Waypoint(busStopPosition);
            way[^2] = new Waypoint(busStopEnterPoint);
            
            return way;
        }

        private static Waypoint[] GetPointsOnParking(Vector3 position, Vector3 direction, ParkingModel parkingModel)
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

            Waypoint[] way; 

            if (hasHit)
            {
                if (normal == Vector3.back)
                {
                    way = new Waypoint[3];
                } else if (normal == Vector3.forward)
                {
                    way = new Waypoint[5];

                    if (direction.x > 0)
                    {
                        way[1] = new Waypoint(parkingModel.RbPoint);
                        way[2] = new Waypoint(parkingModel.RtPoint);
                    }
                    else
                    {
                        way[1] = new Waypoint(parkingModel.LbPoint);
                        way[2] = new Waypoint(parkingModel.LtPoint);
                    }
                }
                else
                {
                    way = new Waypoint[4];
                    if (direction.x > 0)
                    {
                        way[1] = new Waypoint(parkingModel.RtPoint);
                    }
                    else
                    {
                        way[1] = new Waypoint(parkingModel.LtPoint);
                    }
                }
                way[0] = new Waypoint(ray.GetPoint(minDistance));
            }
            else
            {
                way = Array.Empty<Waypoint>();
            }
            
            return way;
        }
    }
}
