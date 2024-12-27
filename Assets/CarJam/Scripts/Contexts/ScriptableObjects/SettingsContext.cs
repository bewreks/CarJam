using CarJam.Scripts.Characters;
using CarJam.Scripts.Queues;
using UnityEngine;
using Zenject;
namespace CarJam.Scripts.Contexts.ScriptableObjects
{
    [CreateAssetMenu(fileName = "SettingsContext", menuName = "CarJam/SettingsContext")]
    public class SettingsContext : ScriptableObjectInstaller
    {
        [field: SerializeField] public CharacterSettings CharacterSettings { get; set; }
        [field: SerializeField] public QueueSettings QueueSettings { get; set; }

        public override void InstallBindings()
        {
            Container.BindInstance(CharacterSettings).AsSingle();
            Container.BindInstance(QueueSettings).AsSingle();
        }
    }
}
