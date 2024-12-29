using System;
using AYellowpaper.SerializedCollections;
using CarJam.Scripts.CarJam;
using CarJam.Scripts.Contexts.Installers;
using UnityEngine;
namespace CarJam.Scripts.Characters
{
    [CreateAssetMenu(fileName = "CharacterSettings", menuName = "CarJam/Settings/Character", order = 0)]
    public class CharacterSettings : ScriptableObject
    {
        [field: SerializeField] public float MovementSpeed { get; private set; }
        [field: SerializeField] public SerializedDictionary<GameColors, Material> Materials { get; private set; }
    }
}
