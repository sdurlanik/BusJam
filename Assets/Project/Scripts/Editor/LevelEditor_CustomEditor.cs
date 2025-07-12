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
        private enum EditorTool { None, AddCharacter, AddObstacle, Erase }

        private EditorTool _currentTool = EditorTool.None;
        private CharacterColor _selectedColor = CharacterColor.Red;

        private SerializedObject levelSO;
        private SerializedObject gridConfigSO;

        private void OnEnable()
        {
            UpdateSerializedObjects();
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
                editor.DisplayLevel();

            DrawActionButtons(editor);

            levelSO.ApplyModifiedProperties();
            gridConfigSO.ApplyModifiedProperties();
            serializedObject.ApplyModifiedProperties();
        }

        #region Section Drawers

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

            var gridSizeProp = levelSO.FindProperty("MainGridSize");
            EditorGUILayout.PropertyField(gridSizeProp);

            Vector2Int minGrid = GetMinimumGridSize(editor.LevelToEdit);
            if (gridSizeProp.vector2IntValue.x < minGrid.x || gridSizeProp.vector2IntValue.y < minGrid.y)
            {
                EditorGUILayout.HelpBox($"Grid size adjusted to {minGrid} to fit placed objects.", MessageType.Warning);
                gridSizeProp.vector2IntValue = new Vector2Int(
                    Mathf.Max(minGrid.x, gridSizeProp.vector2IntValue.x),
                    Mathf.Max(minGrid.y, gridSizeProp.vector2IntValue.y)
                );
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

        private void DrawActionButtons(LevelEditor editor)
        {
            EditorGUILayout.Space(20);
            EditorGUILayout.HelpBox("Saving will rename the asset based on LevelIndex.", MessageType.Info);

            GUI.backgroundColor = new Color(0.6f, 1f, 0.6f);
            if (GUILayout.Button("Save Changes & Rename Asset", GUILayout.Height(40)))
                SaveLevelAsset(editor.LevelToEdit);

            GUI.backgroundColor = new Color(1f, 0.8f, 0.4f);
            if (GUILayout.Button("Create New Level Asset"))
                CreateNewLevelAsset(editor);

            GUI.backgroundColor = Color.white;
        }

        #endregion

        #region Utility Methods

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

            Debug.Log($"🆕 New level created at: {path}");
        }


        #endregion

        #region Scene Interaction

        private void OnSceneGUI()
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
