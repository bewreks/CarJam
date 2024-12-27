using CarJam.Scripts.CarJam;
using CarJam.Scripts.Characters.Factories;
using CarJam.Scripts.Characters.Views;
using CarJam.Scripts.Vehicles.Views;
using UnityEngine;
using Zenject;
namespace CarJam.Scripts.Contexts.ScriptableObjects
{
    
    [CreateAssetMenu(fileName = "PrefabsContext", menuName = "CarJam/PrefabsContext", order = 0)]
    public class PrefabsContext : ScriptableObjectInstaller
    {
        [field: SerializeField] public CharacterView CharacterPrefab { get; private set; }
        [field: SerializeField] public GameObject CarPrefab { get; private set; }
        [field: SerializeField] public GameObject BusPrefab { get; private set; }

        public override void InstallBindings()
        {
            Container.BindFactory<GameColors, CharacterView, CharacterFactory>().FromComponentInNewPrefab(CharacterPrefab).AsSingle();
            Container.BindFactory<CarView, PlaceholderFactory<CarView>>().FromComponentInNewPrefab(CarPrefab).AsSingle();
            Container.BindFactory<BusVehicle, PlaceholderFactory<BusVehicle>>().FromComponentInNewPrefab(BusPrefab).AsSingle();
        }
    }
}
