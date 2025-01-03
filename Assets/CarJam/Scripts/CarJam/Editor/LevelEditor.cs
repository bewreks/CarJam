using System;
using System.Linq;
using CarJam.Scripts.Data;
using CarJam.Scripts.Vehicles.Dummies;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace CarJam.Scripts.CarJam.Editor
{
    [CustomEditor(typeof(LevelScriptableObject))]
    public class LevelEditor : UnityEditor.Editor
    {
        [field: SerializeField] public GameObject BusPrefab { get; set; }
        [field: SerializeField] public GameObject CarPrefab { get; set; }
        
        private GameColors _selectedColor = GameColors.Unknown;
        private Transform _dummyContainer;
        
        private SerializedProperty VehiclesProperty => serializedObject.FindProperty("_vehicles");
        
        public override void OnInspectorGUI()
        {
            DrawButtons();
            DrawProperties();
        }

        private void DrawButtons()
        {
            CreationPart();
            SavePart();
        }

        private void SavePart()
        {
            if (GUILayout.Button("Clear level"))
            {
                ClearDummies();
            }
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Load level"))
            {
                ClearDummies();
                var list = VehiclesProperty;
                for (var i = 0; i < list.arraySize; i++)
                {
                    DummyVehicleView view;
                    var data = list.GetArrayElementAtIndex(i);

                    var id = data.FindPropertyRelative("Id").stringValue;
                    switch (data.FindPropertyRelative("Type").intValue)
                    {
                        case (int)VehicleType.Bus:
                            view = CreateDummyVehicle(BusPrefab, id);
                            break;
                        case (int)VehicleType.Car:
                            view = CreateDummyVehicle(CarPrefab, id);
                            break;
                        default:
                            continue;
                    }
                    if (view == null)
                    {
                        continue;
                    }
                    view.transform.position = data.FindPropertyRelative("Position").vector3Value;
                    view.transform.forward = data.FindPropertyRelative("Direction").vector3Value;
                    view.SetColor((GameColors)data.FindPropertyRelative("Color").intValue);
                }
            }
            if (GUILayout.Button("Bake level"))
            {
                var dummies = FindObjectsByType<DummyVehicleView>(FindObjectsSortMode.None);
                if (dummies.Any(view => view.IsOverlapping))
                {
                    Debug.LogError("Dummies are overlapping");
                    EditorGUILayout.HelpBox("Dummies are overlapping", MessageType.Error, true);
                    return;
                }
                var list = VehiclesProperty;
                list.arraySize = dummies.Length;
                for (var i = 0; i < dummies.Length; i++)
                {
                    var data = list.GetArrayElementAtIndex(i);
                    data.FindPropertyRelative("Color").intValue = (int)dummies[i].Color;
                    data.FindPropertyRelative("Type").intValue = (int)dummies[i].Type;
                    data.FindPropertyRelative("Position").vector3Value = dummies[i].transform.position;
                    data.FindPropertyRelative("Direction").vector3Value = dummies[i].transform.forward;
                    data.FindPropertyRelative("Id").stringValue = dummies[i].name;
                }
                serializedObject.ApplyModifiedProperties();
                Save();
            }
            EditorGUILayout.EndHorizontal();
        }

        private void CreationPart()
        {
            _selectedColor = (GameColors)EditorGUILayout.EnumPopup(new GUIContent("Vehicle color"), _selectedColor);
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Create bus"))
            {
                CreateDummyVehicle(BusPrefab, Guid.NewGuid().ToString());
            }
            if (GUILayout.Button("Create car"))
            {
                CreateDummyVehicle(CarPrefab, Guid.NewGuid().ToString());
            }
            EditorGUILayout.EndHorizontal();
        }

        private DummyVehicleView CreateDummyVehicle(GameObject prefab, string id)
        {
            var sceneView = SceneView.lastActiveSceneView;
            var ray = sceneView.camera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 1));
            var spawnPoint = Vector3.zero;
            var plane = new Plane(Vector3.up, Vector3.zero);
            if (plane.Raycast(ray, out var distance))
            {
                spawnPoint = ray.GetPoint(distance);
            }
            var vehicleGo = Instantiate(prefab, spawnPoint, Quaternion.identity, CreateDummyContainer());
            vehicleGo.name = id;
            var view = vehicleGo.GetComponent<DummyVehicleView>();
            view.SetColor(_selectedColor);
            return view;
        }

        private void ClearDummies()
        {
            var gameObject = GameObject.FindWithTag("DummiesContainer");
            if (gameObject != null)
            {
                DestroyImmediate(gameObject);
                _dummyContainer = null;
            }
            EditorSceneManager.MarkSceneDirty(SceneManager.GetActiveScene());
        }

        private Transform CreateDummyContainer()
        {
            if (_dummyContainer == null)
            {
                var gameObject = GameObject.FindWithTag("DummiesContainer");
                if (gameObject != null)
                {
                    _dummyContainer = gameObject.transform;
                }
            }
            
            if (_dummyContainer == null)
            {
                _dummyContainer = new GameObject("DummiesContainer").transform;
                _dummyContainer.tag = "DummiesContainer";
            }

            return _dummyContainer;

        }

        private void DrawProperties()
        {
            GUI.enabled = false;
            EditorGUILayout.PropertyField(VehiclesProperty, new GUIContent("Vehicles"), true);
            GUI.enabled = true;
            EditorGUILayout.PropertyField(serializedObject.FindProperty("_usedColors"), new GUIContent("Colors for level"), true);
            serializedObject.ApplyModifiedProperties();
        }

        private void Save()
        {
            EditorUtility.SetDirty(target);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }
    }
}
