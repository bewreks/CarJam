using CarJam.Scripts.CarJam;
using CarJam.Scripts.Characters.Models;
using CarJam.Scripts.Characters.Presenters;
using CarJam.Scripts.Characters.Views;
using CarJam.Scripts.Queues.BusStop.Models;
using CarJam.Scripts.Queues.BusStop.Presenters;
using CarJam.Scripts.Queues.BusStop.Views;
using CarJam.Scripts.Queues.Parking.Models;
using CarJam.Scripts.Queues.Parking.Presenters;
using CarJam.Scripts.Vehicles.Models;
using CarJam.Scripts.Vehicles.Presenters;
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
        [field: SerializeField] public BusStopPlaceView BusStopPlacePrefab { get; private set; }

        public override void InstallBindings()
        {
            Container.BindFactory<CharacterModel, CharacterView, CharacterView.Factory>().FromComponentInNewPrefab(CharacterPrefab).AsSingle();
            Container.BindFactory<CharacterModel, CharacterModel.Factory>().AsSingle();
            Container.BindFactory<GameColors, Vector3, CharacterPresenter, CharacterPresenter.Factory>().AsSingle();

            Container.BindFactory<BusStopPlaceModel, BusStopPlaceView, BusStopPlaceView.Factory>().FromComponentInNewPrefab(BusStopPlacePrefab).AsSingle();
            Container.BindFactory<BusStopPlaceModel, BusStopPlaceModel.Factory>().AsSingle();
            Container.BindFactory<BusStopPlacePresenter, BusStopPlacePresenter.Factory>().AsSingle();

            Container.BindFactory<VehicleModel, VehicleView, VehicleView.Factory>().FromComponentInNewPrefab(BusPrefab).AsSingle();
            Container.BindFactory<VehicleModel, VehicleModel.Factory>().AsSingle();
            Container.BindFactory<GameColors, Vector3, VehiclePresenter, VehiclePresenter.Factory>().AsSingle();

            Container.BindFactory<ParkingModel, ParkingModel.Factory>().AsSingle();
            Container.BindFactory<ParkingPresenter, ParkingPresenter.Factory>().AsSingle();
        }
    }
}
