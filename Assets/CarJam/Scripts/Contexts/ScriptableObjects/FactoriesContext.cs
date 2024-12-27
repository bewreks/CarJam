using CarJam.Scripts.CarJam;
using CarJam.Scripts.Characters.Models;
using CarJam.Scripts.Characters.Presenters;
using CarJam.Scripts.Characters.Views;
using CarJam.Scripts.Vehicles.Views;
using UnityEngine;
using Zenject;
namespace CarJam.Scripts.Contexts.ScriptableObjects
{
    
    [CreateAssetMenu(fileName = "FactoriesContext", menuName = "CarJam/FactoriesContext", order = 0)]
    public class FactoriesContext : ScriptableObjectInstaller
    {
        [field: SerializeField] public CharacterView CharacterPrefab { get; private set; }
        [field: SerializeField] public GameObject CarPrefab { get; private set; }
        [field: SerializeField] public GameObject BusPrefab { get; private set; }

        public override void InstallBindings()
        {
            Container.BindFactory<CharacterModel, CharacterView, CharacterView.Factory>().FromComponentInNewPrefab(CharacterPrefab).AsSingle();
            Container.BindFactory<CharacterModel, CharacterModel.Factory>().AsSingle();
            Container.BindFactory<GameColors, Vector3, CharacterPresenter, CharacterPresenter.Factory>().AsSingle();

            
            Container.BindFactory<CarView, PlaceholderFactory<CarView>>().FromComponentInNewPrefab(CarPrefab).AsSingle();
            Container.BindFactory<BusVehicle, PlaceholderFactory<BusVehicle>>().FromComponentInNewPrefab(BusPrefab).AsSingle();
        }
    }
}
