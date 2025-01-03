using CarJam.Scripts.Signals;
using Zenject;
namespace CarJam.Scripts.Contexts.Installers
{
    public class SignalsInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            SignalBusInstaller.Install(Container);
            
            Container.DeclareSignal<UserSelectionSignal>();
            Container.DeclareSignal<NoMorePlacesSignal>();
            Container.DeclareSignal<LevelLoadedSignal>();
            Container.DeclareSignal<GameStartedSignal>();
            Container.DeclareSignal<BusStopFoundSignal>();
            Container.DeclareSignal<StartVehicleMovingToBusStopSignal>();
            Container.DeclareSignal<FinishVehicleMovingToBusStopSignal>();
            Container.DeclareSignal<VehicleMoveOutBusStopSignal>();
            Container.DeclareSignal<CharacterOnAboardSignal>();
            Container.DeclareSignal<LevelClearedSignal>();
            Container.DeclareSignal<NoMoreCharactersToSpawnSignal>();
            
            Container.DeclareSignal<GameEndedSignal>();
            Container.DeclareSignal<StartGameSignal>();
            Container.DeclareSignal<RestartGameSignal>();
            Container.DeclareSignal<ScoreUpdateSignal>();
            Container.DeclareSignal<CountDownSignal>();
        }
    }
}
