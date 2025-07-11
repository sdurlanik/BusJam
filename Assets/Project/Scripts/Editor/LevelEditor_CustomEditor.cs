using System.IO;
using System.Linq;
using Sdurlanik.BusJam.Models;
using UnityEditor;
using UnityEngine;

namespace Sdurlanik.BusJam.LevelEditor
{

    [CustomEditor(typeof(LevelEditor))]
    public class LevelEditor_CustomEditor : Editor
    {
        private enum EditorTool
        {
            None,
            AddCharacter,
            AddObstacle,
            Erase
        }

        private EditorTool _currentTool = EditorTool.None;
        private CharacterColor _selectedColor = CharacterColor.Red;

        private SerializedObject levelSO_serialized;
        private SerializedObject gridConfig_serialized;

        private void OnEnable()
        {
            UpdateSerializedObjects();
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            LevelEditor editorScript = (LevelEditor)target;

            EditorGUI.BeginChangeCheck();
            DrawDefaultInspector();
            if (EditorGUI.EndChangeCheck())
            {
                UpdateSerializedObjects();
                if (editorScript.LevelToEdit != null) editorScript.DisplayLevel();
                else editorScript.ClearLevel();
            }

            if (editorScript.LevelToEdit == null || editorScript.GridConfig == null ||
                editorScript.PrefabConfig == null)
            {
                EditorGUILayout.HelpBox("Please assign all Configuration and Reference fields.", MessageType.Warning);
                return;
            }

            levelSO_serialized.Update();
            if (gridConfig_serialized != null) gridConfig_serialized.Update();

            DrawToolsGUI();
            DrawLevelPropertiesGUI(editorScript);
            DrawBusSettingsGUI();
            DrawGridConfigSettingsGUI(editorScript);
            DrawActionButtonsGUI(editorScript);

            levelSO_serialized.ApplyModifiedProperties();
            if (gridConfig_serialized != null) gridConfig_serialized.ApplyModifiedProperties();

            serializedObject.ApplyModifiedProperties();
        }

        private void DrawToolsGUI()
        {
            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Tools", EditorStyles.boldLabel);
            _currentTool = (EditorTool)GUILayout.Toolbar((int)_currentTool,
                new string[] { "None", "Add Character", "Add Obstacle", "Erase" });

            if (_currentTool == EditorTool.AddCharacter)
            {
                _selectedColor = (CharacterColor)EditorGUILayout.EnumPopup("Character Color", _selectedColor);
            }

            EditorGUILayout.Space();
        }

        private void OnSceneGUI()
        {
            LevelEditor editorScript = (LevelEditor)target;
            if (editorScript.LevelToEdit == null || _currentTool == EditorTool.None) return;

            Event currentEvent = Event.current;
            if (currentEvent.type == EventType.MouseDown && currentEvent.button == 0)
            {
                Ray ray = HandleUtility.GUIPointToWorldRay(currentEvent.mousePosition);
                if (Physics.Raycast(ray, out RaycastHit hit, 200f))
                {
                    Vector2Int gridPos = editorScript.WorldToGridPosition(hit.point);
                    ProcessClick(editorScript, gridPos);
                    currentEvent.Use();
                }
            }
        }

        private void ProcessClick(LevelEditor editorScript, Vector2Int gridPos)
        {
            LevelSO level = editorScript.LevelToEdit;
            bool changed = false;

            level.Characters.RemoveAll(c => c.GridPosition == gridPos);
            level.Obstacles.RemoveAll(o => o.GridPosition == gridPos);

            switch (_currentTool)
            {
                case EditorTool.AddCharacter:
                    level.Characters.Add(new CharacterPlacementData { Color = _selectedColor, GridPosition = gridPos });
                    changed = true;
                    break;
                case EditorTool.AddObstacle:
                    level.Obstacles.Add(new ObstaclePlacementData { GridPosition = gridPos });
                    changed = true;
                    break;
                case EditorTool.Erase:
                    changed = true;
                    break;
            }

            if (changed)
            {
                EditorUtility.SetDirty(level);
                editorScript.DisplayLevel();
                Repaint();
            }
        }

        private void DrawLevelPropertiesGUI(LevelEditor editorScript)
        {
            EditorGUILayout.LabelField("Level Properties", EditorStyles.boldLabel);

            EditorGUILayout.PropertyField(levelSO_serialized.FindProperty("LevelNumber"));
            EditorGUILayout.PropertyField(levelSO_serialized.FindProperty("TimeLimitInSeconds"));

            var gridSizeProp = levelSO_serialized.FindProperty("MainGridSize");
            EditorGUILayout.PropertyField(gridSizeProp);

            Vector2Int minSize = GetMinimumRequiredGridSize(editorScript.LevelToEdit);
            if (gridSizeProp.vector2IntValue.x < minSize.x || gridSizeProp.vector2IntValue.y < minSize.y)
            {
                EditorGUILayout.HelpBox($"Grid size cannot be smaller than {minSize} due to placed objects. Clamped.",
                    MessageType.Warning);
                gridSizeProp.vector2IntValue = new Vector2Int(
                    Mathf.Max(gridSizeProp.vector2IntValue.x, minSize.x),
                    Mathf.Max(gridSizeProp.vector2IntValue.y, minSize.y)
                );
            }
        }

        private void DrawBusSettingsGUI()
        {
            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Bus Properties", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(levelSO_serialized.FindProperty("BusColorSequence"), true);
            EditorGUILayout.PropertyField(levelSO_serialized.FindProperty("BusStopPosition"));
            EditorGUILayout.PropertyField(levelSO_serialized.FindProperty("NextBusOffset"));
        }

        private void DrawGridConfigSettingsGUI(LevelEditor editorScript)
        {
            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Grid Layout Properties (from GridConfiguration)", EditorStyles.boldLabel);

            EditorGUI.BeginChangeCheck();
            EditorGUILayout.PropertyField(gridConfig_serialized.FindProperty("SpacingBetweenGrids"));
            if (EditorGUI.EndChangeCheck())
            {
                editorScript.DisplayLevel();
            }
        }

        private Vector2Int GetMinimumRequiredGridSize(LevelSO level)
        {
            int maxX = 0;
            int maxY = 0;

            if (level.Characters != null && level.Characters.Any())
            {
                maxX = level.Characters.Max(c => c.GridPosition.x);
                maxY = level.Characters.Max(c => c.GridPosition.y);
            }

            if (level.Obstacles != null && level.Obstacles.Any())
            {
                maxX = Mathf.Max(maxX, level.Obstacles.Max(o => o.GridPosition.x));
                maxY = Mathf.Max(maxY, level.Obstacles.Max(o => o.GridPosition.y));
            }

            return new Vector2Int(maxX + 1, maxY + 1);
        }

        private void CreateNewLevelSO(LevelEditor currentEditor)
        {
            string path = EditorUtility.SaveFilePanelInProject(
                "Create New Level",
                "so_level_New.asset",
                "asset",
                "Please enter a file name to save the level to.");

            if (string.IsNullOrEmpty(path)) return; 

            LevelSO newLevel = ScriptableObject.CreateInstance<LevelSO>();
            AssetDatabase.CreateAsset(newLevel, path);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            currentEditor.LevelToEdit = newLevel;
            Selection.activeObject = newLevel;
            Debug.Log($"New level created at: {path}");
        }

        private void DrawActionButtonsGUI(LevelEditor editorScript)
        {
            EditorGUILayout.Space(20);

            GUI.backgroundColor = new Color(0.6f, 1f, 0.6f);
            if (GUILayout.Button("Save Changes & Rename Asset", GUILayout.Height(40)))
            {
                LevelSO currentLevel = editorScript.LevelToEdit;
                string assetPath = AssetDatabase.GetAssetPath(currentLevel);
                string newAssetName = $"so_level_{currentLevel.LevelNumber}";

                if (Path.GetFileNameWithoutExtension(assetPath) != newAssetName)
                {
                    AssetDatabase.RenameAsset(assetPath, newAssetName);
                }

                EditorUtility.SetDirty(currentLevel);
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
                Debug.Log($"'{currentLevel.name}' has been saved!");
            }

            GUI.backgroundColor = new Color(1f, 0.8f, 0.4f);
            if (GUILayout.Button("Create New Level Asset"))
            {
                CreateNewLevelSO(editorScript);
            }

            GUI.backgroundColor = Color.white;
        }

        private void UpdateSerializedObjects()
        {
            LevelEditor editorScript = (LevelEditor)target;
            if (editorScript.LevelToEdit != null)
                levelSO_serialized = new SerializedObject(editorScript.LevelToEdit);
            else
                levelSO_serialized = null;

            if (editorScript.GridConfig != null)
                gridConfig_serialized = new SerializedObject(editorScript.GridConfig);
            else
                gridConfig_serialized = null;
        }
    }
}