﻿using System;
using CarJam.Scripts.Data;
using UniRx;
using UnityEngine;
using Zenject;
namespace CarJam.Scripts.Vehicles.Models
{
    public class VehicleModel
    {
        public Guid Id;
        public GameColors Color;
        public float RotationSpeed;
        public float MovementSpeed;
        public int MaxCapacity;
        public IntReactiveProperty CurrentCapacity = new IntReactiveProperty();
        public ReactiveProperty<Material> Material = new ReactiveProperty<Material>();
        public BoolReactiveProperty IsMoving = new BoolReactiveProperty();
        public Guid BusStopId;

        public class Factory : PlaceholderFactory<VehicleModel>
        {
            
        }
    }
}
