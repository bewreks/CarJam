using CarJam.Scripts.Data;
using CarJam.Scripts.Data.Settings;
using UnityEngine;
namespace CarJam.Scripts.Vehicles.Dummies
{
    [SelectionBase]
    [ExecuteInEditMode]
    [RequireComponent(typeof(BoxCollider))]
    public class DummyVehicleView : MonoBehaviour
    {
        [field: SerializeField] public LayerMask Mask { get; private set; }
        [field: SerializeField] public GameColors Color { get; private set; }
        [field: SerializeField] public BoxCollider Collider { get; private set; }
        [field: SerializeField] public Renderer Renderer { get; private set; }
        [field: SerializeField] public VehicleSettings Settings { get; private set; }
        [field: SerializeField] public Material ErrorMaterial { get; private set; }
        [field: SerializeField] public VehicleType Type { get; private set; }
        
        public bool IsOverlapping { get; private set; } 

        private Collider[] _overlaps = new Collider[2];
        
        public void SetColor(GameColors color)
        {
            Color = color;
            if (color == GameColors.None) return;
            
            Renderer.material = Settings.Materials[color];
        }

        private void Update()
        {
            var position = transform.position;
            position.y = 0;
            transform.position = position;
            IsOverlapping = Physics.OverlapBoxNonAlloc(position, Collider.size / 2f, _overlaps, transform.rotation, Mask) > 1;
            Renderer.material = IsOverlapping ? ErrorMaterial : Settings.Materials[Color];
        }
    }
}
