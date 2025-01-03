using CarJam.Scripts.CarJam;
using CarJam.Scripts.Data;
using UniRx;
using UnityEngine;
using Zenject;
namespace CarJam.Scripts.Characters.Models
{
    public class CharacterModel
    {
        public GameColors Color;
        public float MovementSpeed;
        public ReactiveProperty<Material> Material = new ReactiveProperty<Material>();
        public BoolReactiveProperty IsMoving = new BoolReactiveProperty();

        public class Factory : PlaceholderFactory<CharacterModel>
        {
            
        }
    }
}
