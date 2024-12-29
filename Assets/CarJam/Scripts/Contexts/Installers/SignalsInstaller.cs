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
            Container.DeclareSignal<StartGameSignal>();
            Container.DeclareSignal<BusStopFoundSignal>();
            Container.DeclareSignal<StartVehicleMovingToBusStopSignal>();
            Container.DeclareSignal<FinishVehicleMovingToBusStopSignal>();
        }
    }
}
