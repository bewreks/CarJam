using CarJam.Scripts.CarJam;
using CarJam.Scripts.Characters;
using CarJam.Scripts.Data;
using CarJam.Scripts.Data.Settings;
using CarJam.Scripts.Queues.BusStop;
using CarJam.Scripts.Queues.Characters;
using UnityEngine;
using Zenject;
namespace CarJam.Scripts.Contexts.ScriptableObjects
{
    [CreateAssetMenu(fileName = "SettingsContext", menuName = "CarJam/SettingsContext")]
    public class SettingsContext : ScriptableObjectInstaller
    {
        [field: SerializeField] public CharacterSettings CharacterSettings { get; set; }
        [field: SerializeField] public CharactersQueueSettings CharactersQueueSettings { get; set; }
        [field: SerializeField] public BusStopQueueSettings BusStopQueueSettings { get; set; }
        [field: SerializeField] public VehicleSettings BusVehicleSettings { get; set; }
        [field: SerializeField] public VehicleSettings CarVehicleSettings { get; set; }
        [field: SerializeField] public CarJamSettings CarJamSettings { get; set; }

        public override void InstallBindings()
        {
            Container.BindInstance(CharacterSettings).AsSingle();
            Container.BindInstance(CharactersQueueSettings).AsSingle();
            Container.BindInstance(BusStopQueueSettings).AsSingle();
            Container.BindInstance(BusVehicleSettings).WithId(VehicleType.Bus).AsCached();
            Container.BindInstance(CarVehicleSettings).WithId(VehicleType.Car).AsCached();
            Container.BindInstance(CarJamSettings).AsSingle();
        }
    }
}
