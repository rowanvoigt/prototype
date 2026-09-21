using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TilePainter3D.Blocks;
using Unity.VisualScripting;


#if UNITY_EDITOR
using UnityEditor;
#endif

namespace TilePainter3D
{

    [ExecuteInEditMode]
    public class TilePainter3D : MonoBehaviour
    {
        //TODO: when doing height with blocks, make inactive fully surrounded blocks(3x3x3 must be full),
        //add bake button to erase interior blocks from memory and optimize

        [Header("Grid Settings")]
        public Vector3 cellSize = Vector3.one;
        [Header("Brush Settings")]
        [Range(1, 16)]
        public int brushSize = 1;

        public BlockDataSO selectedBlock;


        private Dictionary<Vector3Int, GameObject> gridData = new Dictionary<Vector3Int, GameObject>();


        public bool IsCellOccupied(Vector3Int gridPos) => gridData.ContainsKey(gridPos);

        public Vector3Int WorldToGrid(Vector3 worldPos)
        {
            return new Vector3Int(
                Mathf.FloorToInt(worldPos.x / cellSize.x),
                Mathf.Max(0, Mathf.FloorToInt(worldPos.y / cellSize.y)),
                Mathf.FloorToInt(worldPos.z / cellSize.z)
            );
        }

        public Vector3 GridToWorld(Vector3Int gridPos)
        {
            return new Vector3(
                (gridPos.x + 0.5f) * cellSize.x,
                (gridPos.y + 0.5f) * cellSize.y,
                (gridPos.z + 0.5f) * cellSize.z
            );
        }

        public void PaintTile(Vector3Int gridPos)
        {
            if (IsCellOccupied(gridPos)) return;

            SetTileDirect(gridPos);
            UpdateNeighbors(gridPos);

        }

        public void EraseTile(Vector3Int gridPos)
        {
            if (!gridData.ContainsKey(gridPos)) return;

            RemoveTileDirect(gridPos);
            UpdateNeighbors(gridPos);
        }

        public bool TryGetGridPosFromObject(GameObject obj, out Vector3Int gridPos)
        {
            foreach (var pair in gridData)
            {
                if (pair.Value == obj)
                {
                    gridPos = pair.Key;
                    return true;
                }
            }
            gridPos = Vector3Int.zero;
            return false;
        }

        private void SetTileDirect(Vector3Int gridPos)
        {
            RemoveTileDirect(gridPos);

            (GameObject prefabToUse, Quaternion rotation) = GetAutoTileConfig(gridPos);

            if (prefabToUse == null) return;

            Vector3 spawnWorldPos = GridToWorld(gridPos);

#if UNITY_EDITOR
            GameObject instance = (GameObject)PrefabUtility.InstantiatePrefab(prefabToUse, transform);
            if (instance == null) instance = Instantiate(prefabToUse, transform);

            instance.transform.position = spawnWorldPos;
            instance.transform.rotation = rotation;

            BoxCollider col = instance.GetComponent<BoxCollider>();
            if (col == null) col = instance.AddComponent<BoxCollider>();
            col.size = cellSize;
            col.center = Vector3.zero;

            Undo.RegisterCreatedObjectUndo(instance, "Paint Tile 3D");
            gridData[gridPos] = instance;
            TriggerPopAnimation(instance.transform, prefabToUse.transform.localScale);
#endif
        }

        private void RemoveTileDirect(Vector3Int gridPos)
        {
            if (gridData.TryGetValue(gridPos, out GameObject obj))
            {
                if (obj != null)
                {
#if UNITY_EDITOR
                    Undo.DestroyObjectImmediate(obj);
#endif
                }
                gridData.Remove(gridPos);
            }
        }
        public void ClearGrid()
        {
            Undo.IncrementCurrentGroup();
            Undo.SetCurrentGroupName("Clear 3D Tile Grid");
            int undoGroup = Undo.GetCurrentGroup();

            List<Vector3Int> keys = new List<Vector3Int>(gridData.Keys);

            foreach (Vector3Int pos in keys)
            {
                RemoveTileDirect(pos);
            }
            for (int i = transform.childCount - 1; i >= 0; i--)
            {

                DestroyImmediate(transform.GetChild(i).gameObject);
            }
            gridData.Clear();


            Undo.CollapseUndoOperations(undoGroup);
        }

        private void UpdateNeighbors(Vector3Int centerPos)
        {
            for (int x = -1; x <= 1; x++)
            {
                for (int z = -1; z <= 1; z++)
                {
                    if (x == 0 && z == 0) continue;
                    Vector3Int nPos = centerPos + new Vector3Int(x, 0, z);
                    if (gridData.ContainsKey(nPos))
                    {
                        SetTileDirect(nPos);
                    }
                }
            }
        }

        private (GameObject prefab, Quaternion rotation) GetAutoTileConfig(Vector3Int pos)
        {

            BlockMatrix blockMatrix = new();

            // 1. Conexiones Cardinales
            bool North = gridData.ContainsKey(pos + Vector3Int.forward);
            blockMatrix.SetMiddleLayer(0, 1, North);
            bool East = gridData.ContainsKey(pos + Vector3Int.right);
            blockMatrix.SetMiddleLayer(1, 2, East);
            bool South = gridData.ContainsKey(pos + Vector3Int.back);
            blockMatrix.SetMiddleLayer(2, 1, South);
            bool West = gridData.ContainsKey(pos + Vector3Int.left);
            blockMatrix.SetMiddleLayer(1, 0, West);

            // 2. Conexiones Diagonales
            bool NorthEast = gridData.ContainsKey(pos + new Vector3Int(1, 0, 1));
            blockMatrix.SetMiddleLayer(0, 2, NorthEast);
            bool SouthEast = gridData.ContainsKey(pos + new Vector3Int(1, 0, -1));
            blockMatrix.SetMiddleLayer(2, 2, SouthEast);
            bool SouthWest = gridData.ContainsKey(pos + new Vector3Int(-1, 0, -1));
            blockMatrix.SetMiddleLayer(2, 0, SouthWest);
            bool NorthWest = gridData.ContainsKey(pos + new Vector3Int(-1, 0, 1));
            blockMatrix.SetMiddleLayer(0, 0, NorthWest);

            //  Debug.Log(blockMatrix.ToString());


            float calculatedAngle = 0f;
            TileBlockConfig tileBlock = new();
            // Debug.Log("[TilePainter]Finding proper block...");
            // Debug.Log("Block has: " + blockMatrix.GetNumberOfConnectionsMiddleLayer() + " connections");
            for (int i = 0; i < 4; i++)
            {

                (tileBlock.prefab, tileBlock.rotationOffset) = blockMatrix.FindProperBlockWHeight(selectedBlock.blockData);

                if (tileBlock != null && tileBlock.prefab != null)
                    break;

                blockMatrix.ShiftBitMask90();
                //calculatedAngle = i % 2 == 0 ? calculatedAngle + 90 : calculatedAngle;
                calculatedAngle += 90;

            }

            if (tileBlock.prefab == null)
            {
                tileBlock = selectedBlock.blockData.block0Conn;
                //Debug.LogWarning("Using fallback");
            }
            float finalAngle = calculatedAngle + tileBlock.rotationOffset;
            // Debug.Log("Using as final angle: " + finalAngle);
            return (tileBlock.prefab, Quaternion.Euler(0f, finalAngle, 0f));
        }
        private void OnEnable()
        {
            //Recover instances when rebuilding domain
            RebuildGridData();
#if UNITY_EDITOR
            EditorApplication.update += UpdateAnimations;
#endif
        }
        private void OnDisable()
        {
            // Nos desuscribimos para no dejar procesos colgados
#if UNITY_EDITOR
            EditorApplication.update -= UpdateAnimations;
#endif
            activeAnimations.Clear();
        }

        public void RebuildGridData()
        {
            if (gridData == null)
                gridData = new Dictionary<Vector3Int, GameObject>();

            gridData.Clear();


            for (int i = 0; i < transform.childCount; i++)
            {
                Transform child = transform.GetChild(i);

                Vector3Int gridPos = WorldToGrid(child.position);


                if (!gridData.ContainsKey(gridPos))
                {
                    gridData.Add(gridPos, child.gameObject);
                }
            }
        }
        private void TriggerPopAnimation(Transform target, Vector3 correctScale)
        {
            if (activeAnimations.Exists(a => a.targetTransform == target)) return;

            activeAnimations.Add(new PopAnimationData
            {
                targetTransform = target,
                originalScale = correctScale,
                startTime = EditorApplication.timeSinceStartup
            });
        }

        private void UpdateAnimations()
        {
            if (activeAnimations.Count == 0) return;

            float duration = 0.25f;
            float scaleMultiplier = 1.3f;
            bool needsRepaint = false;

            for (int i = activeAnimations.Count - 1; i >= 0; i--)
            {
                var anim = activeAnimations[i];

                if (anim.targetTransform == null)
                {
                    activeAnimations.RemoveAt(i);
                    continue;
                }

                float elapsed = (float)(EditorApplication.timeSinceStartup - anim.startTime);
                float percent = elapsed / duration;

                if (percent <= 1f)
                {
                    float curve = Mathf.Sin(percent * Mathf.PI);
                    anim.targetTransform.localScale = anim.originalScale * (1f + (scaleMultiplier - 1f) * curve);
                    needsRepaint = true;
                }
                else
                {
                    anim.targetTransform.localScale = anim.originalScale;
                    activeAnimations.RemoveAt(i);
                    needsRepaint = true;
                }
            }

            if (needsRepaint && !Application.isPlaying)
            {
                SceneView.RepaintAll();
            }
        }

        public void UndoAction()
        {
#if UNITY_EDITOR
            // Retrocede un paso en la pila de Undo del Editor de Unity
            Undo.PerformUndo();

            // Sincronizamos de nuevo el diccionario con los objetos reales de la escena
            RebuildGridData();
#endif
        }

        public void RedoAction()
        {
#if UNITY_EDITOR
            // Avanza un paso en la pila de Redo del Editor de Unity
            Undo.PerformRedo();

            // Sincronizamos de nuevo el diccionario con los objetos reales de la escena
            RebuildGridData();
#endif
        }

        private class PopAnimationData
        {
            public Transform targetTransform;
            public Vector3 originalScale;
            public double startTime; // Unity usa double para el tiempo del Editor
        }

        private List<PopAnimationData> activeAnimations = new List<PopAnimationData>();

    }

}
