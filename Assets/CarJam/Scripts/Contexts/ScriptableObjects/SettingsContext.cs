using CarJam.Scripts.CarJam;
using CarJam.Scripts.Characters;
using CarJam.Scripts.Queues;
using CarJam.Scripts.Queues.Parking;
using CarJam.Scripts.Vehicles;
using UnityEngine;
using Zenject;
namespace CarJam.Scripts.Contexts.ScriptableObjects
{
    [CreateAssetMenu(fileName = "SettingsContext", menuName = "CarJam/SettingsContext")]
    public class SettingsContext : ScriptableObjectInstaller
    {
        [field: SerializeField] public CharacterSettings CharacterSettings { get; set; }
        [field: SerializeField] public CharactersQueueSettings CharactersQueueSettings { get; set; }
        [field: SerializeField] public ParkingQueueSettings ParkingQueueSettings { get; set; }
        [field: SerializeField] public VehicleSettings VehicleSettings { get; set; }
        [field: SerializeField] public CarJamSettings CarJamSettings { get; set; }

        public override void InstallBindings()
        {
            Container.BindInstance(CharacterSettings).AsSingle();
            Container.BindInstance(CharactersQueueSettings).AsSingle();
            Container.BindInstance(ParkingQueueSettings).AsSingle();
            Container.BindInstance(VehicleSettings).AsSingle();
            Container.BindInstance(CarJamSettings).AsSingle();
        }
    }
}
