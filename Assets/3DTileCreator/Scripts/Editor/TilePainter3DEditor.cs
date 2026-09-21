using UnityEngine;
using UnityEditor;
using TilePainter3D;
using TilePainter3D.Blocks; // Added to access BlockDataSO
#if UNITY_EDITOR

namespace TilePainter3D.EDITOR
{
    [CustomEditor(typeof(TilePainter3D))]
    public class TilePainter3DEditor : Editor
    {
        private TilePainter3D painter;
        private bool isPaintMode = false;
        private bool isEraseMode = false;

        // Size of the grid to draw (e.g., 20x20 cells around the origin)
        private int gridExtent = 15;

        private Tool lastTool = Tool.Move;

        private void OnEnable()
        {
            if (target == null) return;
            painter = (TilePainter3D)target;
        }

        public override void OnInspectorGUI()
        {

            if (painter == null || target == null) return;
            DrawDefaultInspector();


            EditorGUILayout.Space(10);

            // --- UPDATED LOGIC: VISUAL BLOCK SELECTOR ---
            EditorGUILayout.LabelField("Block Library", EditorStyles.boldLabel);

            string[] guids = AssetDatabase.FindAssets("t:BlockDataSO");
            if (guids.Length == 0)
            {
                EditorGUILayout.HelpBox("No BlockDataSO assets found in project!", MessageType.Warning);
            }
            else
            {
                int columns = 3;
                int index = 0;

                EditorGUILayout.BeginVertical(EditorStyles.helpBox);

                while (index < guids.Length)
                {
                    EditorGUILayout.BeginHorizontal();
                    for (int c = 0; c < columns; c++)
                    {
                        if (index < guids.Length)
                        {
                            string path = AssetDatabase.GUIDToAssetPath(guids[index]);
                            BlockDataSO blockData = AssetDatabase.LoadAssetAtPath<BlockDataSO>(path);

                            if (blockData != null)
                            {
                                bool isSelected = (painter.selectedBlock == blockData);

                                EditorGUILayout.BeginVertical(GUILayout.Width(80), GUILayout.Height(80));

                                Texture2D previewTexture = null;
                                GameObject blockPreview = blockData.blockData.block0Conn.prefab;
                                if (blockPreview != null && blockPreview != null)
                                {
                                    previewTexture = AssetPreview.GetAssetPreview(blockPreview);
                                    if (previewTexture == null)
                                    {
                                        Repaint();
                                    }
                                }

                                // We use an empty string so no text is forced inside the button layout, 
                                // and pass the texture directly as the image.
                                GUIContent content = new GUIContent(previewTexture, blockData.name);

                                // Style to highlight the selected block with a background tint or border
                                GUIStyle iconButtonStyle = new GUIStyle(GUI.skin.button);
                                if (isSelected)
                                {
                                    iconButtonStyle.normal.background = Texture2D.grayTexture;
                                }

                                if (GUILayout.Button(content, iconButtonStyle, GUILayout.Width(75), GUILayout.Height(75)))
                                {
                                    Undo.RecordObject(painter, "Change Selected Block");
                                    painter.selectedBlock = blockData;
                                    EditorUtility.SetDirty(painter);
                                }

                                EditorGUILayout.EndVertical();
                            }
                            index++;
                        }
                        else
                        {
                            GUILayout.Label("", GUILayout.Width(80), GUILayout.Height(80));
                        }
                    }
                    EditorGUILayout.EndHorizontal();
                }

                EditorGUILayout.EndVertical();
            }
            // --- END OF VISUAL BLOCK SELECTOR ---

            EditorGUILayout.Space(10);

            // Clean style and info (no duplicates)
            GUIStyle headerStyle = new GUIStyle(EditorStyles.boldLabel) { fontSize = 13 };
            EditorGUILayout.LabelField("3D Paint Tools", headerStyle);

            GUILayout.BeginHorizontal();

            // Mode Buttons
            GUI.backgroundColor = isPaintMode ? Color.green : Color.white;
            if (GUILayout.Button("Paint", GUILayout.Height(30)))
            {
                SetPaintMode(!isPaintMode);
            }

            GUI.backgroundColor = isEraseMode ? Color.red : Color.white;
            if (GUILayout.Button("Erase", GUILayout.Height(30)))
            {
                SetEraseMode(!isEraseMode);
            }

            GUI.backgroundColor = Color.white;
            GUILayout.EndHorizontal();

            EditorGUILayout.Space(5);

            // Undo and Redo Buttons
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Undo"))
            {
                painter.UndoAction();
                SceneView.RepaintAll();
            }
            if (GUILayout.Button("Redo"))
            {
                painter.RedoAction();
                SceneView.RepaintAll();
            }
            GUILayout.EndHorizontal();
            EditorGUILayout.Space(5);

            // Clear Grid Button (Red Color)
            GUI.backgroundColor = new Color(1f, 0.3f, 0.3f); // Soft red so it doesn't clash with the UI
            if (GUILayout.Button("Clear Grid", GUILayout.Height(25)))
            {
                // Safety confirmation to prevent accidental deletions
                if (EditorUtility.DisplayDialog("Clear Grid",
                    "¿Are you sure?", "Yes, clean", "Cancel"))
                {
                    TilePainter3D painter = (TilePainter3D)target;

                    // If the ClearGrid function is 'private', remember to change it to 'public' in TilePainter3D.cs
                    painter.ClearGrid();
                    SceneView.RepaintAll();
                }
            }
            GUI.backgroundColor = Color.white; // Reset color
        }

        private void OnSceneGUI()
        {

            // Always draw the base with its visual mesh/grid at Y = 0
            DrawGridBase();

            if (!isPaintMode && !isEraseMode) return;


            // Disable accidental selection in the scene while painting
            HandleUtility.AddDefaultControl(GUIUtility.GetControlID(FocusType.Passive));

            Ray ray = HandleUtility.GUIPointToWorldRay(Event.current.mousePosition);
            Plane groundPlane = new Plane(Vector3.up, Vector3.zero);

            if (groundPlane.Raycast(ray, out float enter))
            {
                Vector3 hitPoint = ray.GetPoint(enter);
                Vector3Int centerGridPos = painter.WorldToGrid(hitPoint);
                int size = painter.brushSize;

                // Calculamos el punto de inicio para centrar el pincel según el tamaño seleccionado
                int radius = size / 2;
                Vector3Int startPos = centerGridPos - new Vector3Int(radius, 0, radius);

                // --- 1. DIBUJAR EL WIREFRAME DE TODO EL ÁREA DEL PINCEL ---
                for (int x = 0; x < size; x++)
                {
                    for (int z = 0; z < size; z++)
                    {
                        Vector3Int currentCell = startPos + new Vector3Int(x, 0, z);
                        Vector3 cellCenter = painter.GridToWorld(currentCell);

                        if (isPaintMode)
                        {
                            Handles.color = painter.IsCellOccupied(currentCell) ? Color.yellow : Color.cyan;
                        }
                        else
                        {
                            Handles.color = Color.red;
                        }

                        Handles.DrawWireCube(cellCenter, painter.cellSize);
                    }
                }

                // --- 2. APLICAR ACCIÓN (PAINT / ERASE) EN ÁREA ---
                Event e = Event.current;
                if ((e.type == EventType.MouseDown || e.type == EventType.MouseDrag) && e.button == 0)
                {
                    for (int x = 0; x < size; x++)
                    {
                        for (int z = 0; z < size; z++)
                        {
                            Vector3Int targetCell = startPos + new Vector3Int(x, 0, z);

                            if (isPaintMode)
                            {
                                painter.PaintTile(targetCell);
                            }
                            else if (isEraseMode)
                            {
                                painter.EraseTile(targetCell);
                            }
                        }
                    }
                    e.Use();
                }
            }

            SceneView.RepaintAll();
        }

        private void DrawGridBase()
        {
            Vector3 cellSize = painter.cellSize;

            if (cellSize.x <= 0 || cellSize.z <= 0) return;

            float width = gridExtent * cellSize.x;
            float depth = gridExtent * cellSize.z;

            // 1. Draw gray plane (Y = 0)
            Handles.zTest = UnityEngine.Rendering.CompareFunction.LessEqual;

            Vector3[] planeVertices = new Vector3[]
            {
            new Vector3(-width, 0, -depth),
            new Vector3(width, 0, -depth),
            new Vector3(width, 0, depth),
            new Vector3(-width, 0, depth)
            };

            Handles.DrawSolidRectangleWithOutline(
                planeVertices,
                new Color(0.18f, 0.18f, 0.18f, 0.85f),
                new Color(0.3f, 0.3f, 0.3f, 1f)
            );

            // 2. Draw grid lines
            Handles.color = new Color(0.5f, 0.5f, 0.5f, 0.4f);

            // Parallel lines to Z (X axis)
            for (int i = -gridExtent; i <= gridExtent; i++)
            {
                float xPos = i * cellSize.x;
                Handles.DrawLine(new Vector3(xPos, 0.001f, -depth), new Vector3(xPos, 0.001f, depth));
            }

            // Parallel lines to X (Z axis)
            for (int j = -gridExtent; j <= gridExtent; j++)
            {
                float zPos = j * cellSize.z;
                Handles.DrawLine(new Vector3(-width, 0.001f, zPos), new Vector3(width, 0.001f, zPos));
            }
        }
        private void SetPaintMode(bool active)
        {
            isPaintMode = active;
            isEraseMode = false;
            HandleToolState();
        }

        private void SetEraseMode(bool active)
        {
            isEraseMode = active;
            isPaintMode = false;
            HandleToolState();
        }
        private void HandleToolState()
        {
            if (isPaintMode || isEraseMode)
            {
                // Si entramos en modo pintura/borrado y las herramientas aún están visibles, guardamos la actual
                if (Tools.current != Tool.None)
                {
                    lastTool = Tools.current;
                }
                Tools.current = Tool.None; // Ocultamos el gizmo
            }
            else
            {
                // Al salir, restauramos exactamente la herramienta que el usuario tenía antes
                Tools.current = lastTool;
            }
        }
    }

}
#endif