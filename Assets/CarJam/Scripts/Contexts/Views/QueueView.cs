using UnityEngine;
namespace CarJam.Scripts.Queues
{
    public class QueueView : MonoBehaviour
    {
        [field: SerializeField] public Transform Start { get; private set; }
        [field: SerializeField] public Transform End { get; private set; }
    }
}
