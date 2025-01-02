using System.Collections.Generic;
using System.Linq;
using CarJam.Scripts.Vehicles;
using CarJam.Scripts.Vehicles.Data;
using UnityEngine;
namespace CarJam.Scripts.CarJam
{
    public class Level
    {
        public readonly VehiclesData[] Vehicles;
        public Dictionary<GameColors, int> CharactersCounter;

        public Level(VehiclesData[] vehicles, GameColors[] usedColors, params VehicleSettings[] vehicleSettings)
        {
            Vehicles = vehicles;
            
            foreach (var data in Vehicles.Where(data => data.Color == GameColors.Unknown))
            {
                data.Color = usedColors[Random.Range(0, usedColors.Length)];
            }

            CharactersCounter = Vehicles.GroupBy(data => data.Color)
                                        .ToDictionary(datas => datas.Key,
                                            datas => datas.Sum(data => vehicleSettings.First(settings => settings.Type == data.Type).Capacity));
        }

    }
}
