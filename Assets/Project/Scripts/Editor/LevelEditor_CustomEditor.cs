using System.IO;
using System.Linq;
using Sdurlanik.BusJam.Core.Grid;
using Sdurlanik.BusJam.Models;
using UnityEditor;
using UnityEngine;

namespace Sdurlanik.BusJam.LevelEditor
{
    [CustomEditor(typeof(LevelEditor))]
    public class LevelEditor_CustomEditor : Editor
    {
        private enum EditorTool { None, AddCharacter, AddObstacle, Erase }

        private EditorTool _currentTool = EditorTool.None;
        private CharacterColor _selectedColor = CharacterColor.Red;

        private SerializedObject levelSO;
        private SerializedObject gridConfigSO;

        private void OnEnable()
        {
            UpdateSerializedObjects();
            SceneView.duringSceneGui += OnSceneGUI;
        }

        private void OnDisable()
        {
            SceneView.duringSceneGui -= OnSceneGUI;
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            LevelEditor editor = (LevelEditor)target;

            DrawConfigurationSection(editor);
            if (!IsEditorReady(editor)) return;

            levelSO.Update();
            gridConfigSO.Update();

            EditorGUI.BeginChangeCheck();
            DrawToolsSection();
            DrawLevelPropertiesSection(editor);
            DrawBusSettingsSection(editor);
            DrawGridSettingsSection(editor);

            if (EditorGUI.EndChangeCheck())
            {
                editor.DisplayLevel();
                FocusCameraOnGrid(editor);
            }

            DrawActionButtons(editor);

            levelSO.ApplyModifiedProperties();
            gridConfigSO.ApplyModifiedProperties();
            serializedObject.ApplyModifiedProperties();
        }

        #region Section Drawers

        private void DrawActionButtons(LevelEditor editor)
        {
            EditorGUILayout.Space(20);

            if (GUILayout.Button("Focus Camera on Grid", GUILayout.Height(30)))
            {
                FocusCameraOnGrid(editor);
            }
            
            EditorGUILayout.Space(10);
            
            GUI.backgroundColor = new Color(0.6f, 1f, 0.6f);
            if (GUILayout.Button("Save Changes & Rename Asset", GUILayout.Height(40)))
                SaveLevelAsset(editor.LevelToEdit);

            GUI.backgroundColor = new Color(1f, 0.8f, 0.4f);
            if (GUILayout.Button("Create New Level Asset"))
                CreateNewLevelAsset(editor);

            GUI.backgroundColor = Color.white;
        }
        
        private void DrawConfigurationSection(LevelEditor editor)
        {
            EditorGUILayout.BeginVertical("box");
            EditorGUILayout.LabelField("Configuration & Scene Assets", EditorStyles.boldLabel);
            EditorGUILayout.HelpBox("Assign the level to edit and its dependencies.", MessageType.Info);

            EditorGUI.BeginChangeCheck();
            DrawDefaultInspector();
            if (EditorGUI.EndChangeCheck())
            {
                UpdateSerializedObjects();
                if (editor.LevelToEdit != null)
                    editor.DisplayLevel();
                else
                    editor.ClearLevel();
            }

            EditorGUILayout.EndVertical();
        }
        private void DrawToolsSection()
        {
            EditorGUILayout.Space(15);
            EditorGUILayout.BeginVertical("box");
            EditorGUILayout.HelpBox("Use these tools to place characters, obstacles, or erase existing objects.", MessageType.None);
            EditorGUILayout.Space(15);
            EditorGUILayout.LabelField("Editing Tools", EditorStyles.boldLabel);


            _currentTool = (EditorTool)GUILayout.Toolbar((int)_currentTool, new[] { "None", "Add Character", "Add Obstacle", "Erase" });

            if (_currentTool == EditorTool.AddCharacter)
                _selectedColor = (CharacterColor)EditorGUILayout.EnumPopup("Character Color", _selectedColor);

            EditorGUILayout.EndVertical();
        }
        private void DrawLevelPropertiesSection(LevelEditor editor)
        {
            EditorGUILayout.Space(15);
            EditorGUILayout.BeginVertical("box");
            EditorGUILayout.HelpBox("Edit level number, time limit and grid dimensions.", MessageType.None);

            EditorGUILayout.PropertyField(levelSO.FindProperty("LevelIndex"));
            EditorGUILayout.PropertyField(levelSO.FindProperty("TimeLimitInSeconds"));

            SerializedProperty gridSizeProp = levelSO.FindProperty("MainGridSize");
            Vector2Int gridSize = gridSizeProp.vector2IntValue;

            Vector2Int minGrid = GetMinimumGridSize(editor.LevelToEdit);
            Vector2Int newGridSize = EditorGUILayout.Vector2IntField("Main Grid Size", gridSize);

            newGridSize.x = Mathf.Max(newGridSize.x, minGrid.x);
            newGridSize.y = Mathf.Max(newGridSize.y, minGrid.y);

            if (newGridSize != gridSize)
            {
                gridSizeProp.vector2IntValue = newGridSize;
                levelSO.ApplyModifiedProperties();
                editor.DisplayLevel();
                GUI.FocusControl(null);
            }

            EditorGUILayout.EndVertical();
        }


        private void DrawBusSettingsSection(LevelEditor editor)
        {
            EditorGUILayout.Space(15);
            EditorGUILayout.BeginVertical("box");
            EditorGUILayout.HelpBox("Control bus spawn position, next bus offset and color sequence.", MessageType.None);

            EditorGUI.BeginChangeCheck();

            EditorGUILayout.PropertyField(levelSO.FindProperty("BusColorSequence"), true);
            EditorGUILayout.PropertyField(levelSO.FindProperty("BusStopPosition"));
            EditorGUILayout.PropertyField(levelSO.FindProperty("NextBusOffset"));

            if (EditorGUI.EndChangeCheck())
            {
                levelSO.ApplyModifiedProperties();
                editor.DisplayLevel();
            }

            EditorGUILayout.EndVertical();
        }
        private void DrawGridSettingsSection(LevelEditor editor)
        {
            EditorGUILayout.Space(15);
            EditorGUILayout.BeginVertical("box");
            EditorGUILayout.HelpBox("Spacing between main grid and waiting area.", MessageType.None);
            EditorGUILayout.Space(15);
            EditorGUILayout.LabelField("Grid Layout Settings", EditorStyles.boldLabel);

            EditorGUI.BeginChangeCheck();
            EditorGUILayout.PropertyField(gridConfigSO.FindProperty("SpacingBetweenGrids"));
            if (EditorGUI.EndChangeCheck())
                editor.DisplayLevel();

            EditorGUILayout.EndVertical();
        }
        

        #endregion

        #region Utility & Camera Methods

        private void FocusCameraOnGrid(LevelEditor editor)
        {
            SceneView sceneView = SceneView.lastActiveSceneView;
            if (sceneView == null || sceneView.camera == null) return;
            
            var camera = sceneView.camera;
            var levelData = editor.LevelToEdit;

            camera.orthographic = false;
            float fieldOfView = 60f;
            float padding = 1.15f;
            
            Bounds gameplayBounds = CalculateGameplayBounds(levelData, editor.GridConfig);

            float worldWidth = gameplayBounds.size.x;
            float worldHeight = gameplayBounds.size.z;
            
            float verticalFovRad = fieldOfView * Mathf.Deg2Rad;
            float horizontalFovRad = 2f * Mathf.Atan(Mathf.Tan(verticalFovRad / 2f) * camera.aspect);

            float distanceForHeight = (worldHeight / 2f) / Mathf.Tan(verticalFovRad / 2f);
            float distanceForWidth = (worldWidth / 2f) / Mathf.Tan(horizontalFovRad / 2f);
            
            float requiredDistance = Mathf.Max(distanceForWidth, distanceForHeight);
            
            Vector3 targetPoint = gameplayBounds.center;
            targetPoint.x = (levelData.MainGridSize.x - 1) / 2f;
            
            var rotation = Quaternion.Euler(60f, 0, 0);
            
            sceneView.LookAt(targetPoint, rotation, requiredDistance * padding, false);
        }
        
        private Bounds CalculateGameplayBounds(LevelSO levelData, GridConfiguration gridConfig)
        {
            var bounds = new Bounds();
            bounds.Encapsulate(Vector3.zero);
            bounds.Encapsulate(new Vector3(levelData.MainGridSize.x, 0, levelData.MainGridSize.y));

            float waitingAreaXOffset = (levelData.MainGridSize.x - gridConfig.WaitingGridSize.x) / 2f;
            float waitingAreaZPos = levelData.MainGridSize.y + gridConfig.SpacingBetweenGrids;
            
            var waitingAreaStart = new Vector3(waitingAreaXOffset, 0, waitingAreaZPos);
            var waitingAreaEnd = waitingAreaStart + new Vector3(gridConfig.WaitingGridSize.x, 0, gridConfig.WaitingGridSize.y);
            
            bounds.Encapsulate(waitingAreaStart);
            bounds.Encapsulate(waitingAreaEnd);
            
            bounds.Encapsulate(levelData.BusStopPosition);
            bounds.Encapsulate(levelData.BusStopPosition + levelData.NextBusOffset);

            return bounds;
        }

        private void UpdateSerializedObjects()
        {
            var editor = (LevelEditor)target;
            levelSO = editor.LevelToEdit != null ? new SerializedObject(editor.LevelToEdit) : null;
            gridConfigSO = editor.GridConfig != null ? new SerializedObject(editor.GridConfig) : null;
        }
        private bool IsEditorReady(LevelEditor editor)
        {
            if (editor.LevelToEdit == null || editor.GridConfig == null || editor.PrefabConfig == null)
            {
                EditorGUILayout.HelpBox("Please complete all required configuration fields.", MessageType.Warning);
                return false;
            }

            return true;
        }
        private Vector2Int GetMinimumGridSize(LevelSO level)
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
        private void SaveLevelAsset(LevelSO level)
        {
            string path = AssetDatabase.GetAssetPath(level);
            string newName = $"so_level_{level.LevelIndex + 1}";

            if (Path.GetFileNameWithoutExtension(path) != newName)
                AssetDatabase.RenameAsset(path, newName);

            EditorUtility.SetDirty(level);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            Debug.Log($"Saved Level: {newName}");
        }
        private void CreateNewLevelAsset(LevelEditor editor)
        {
            string path = EditorUtility.SaveFilePanelInProject(
                "Create New Level",
                "so_level_New.asset",
                "asset",
                "Enter a file name to save the level to.");

            if (string.IsNullOrEmpty(path)) return;

            LevelSO newLevel = ScriptableObject.CreateInstance<LevelSO>();
            AssetDatabase.CreateAsset(newLevel, path);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            Undo.RecordObject(editor, "Assign New LevelSO");
            editor.LevelToEdit = newLevel;

            serializedObject.Update();
            UpdateSerializedObjects();

            editor.DisplayLevel();
            EditorUtility.SetDirty(editor);
            Repaint();

            Selection.activeGameObject = editor.gameObject;
        }


        #endregion

        #region Scene Interaction

        private void OnSceneGUI(SceneView sceneView)
        {
            LevelEditor editor = (LevelEditor)target;
            if (editor.LevelToEdit == null || _currentTool == EditorTool.None) return;

            Event e = Event.current;
            if (e.type == EventType.MouseDown && e.button == 0)
            {
                Ray ray = HandleUtility.GUIPointToWorldRay(e.mousePosition);
                if (Physics.Raycast(ray, out RaycastHit hit, 100f))
                {
                    Vector2Int pos = editor.WorldToGridPosition(hit.point);
                    HandleSceneClick(editor, pos);
                    e.Use();
                }
            }
        }

        private void HandleSceneClick(LevelEditor editor, Vector2Int gridPos)
        {
            var level = editor.LevelToEdit;

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
                editor.DisplayLevel();
                Repaint();
            }
        }
        #endregion
    }
}