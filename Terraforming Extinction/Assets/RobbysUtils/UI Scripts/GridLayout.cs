using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace RobbysUtils
{
    [ExecuteInEditMode]
    public class GridLayout : MonoBehaviour
    {
        [Header("Debugging")]
        [SerializeField] bool showDebugInfo = false;
        //[SerializeField] bool useChildTransforms = true;
        //[SerializeField] bool useSpawnedPrefabs;
        [SerializeField] GridChildSortType sortType;

        [Space]

        [Header("Spawned Prefabs Settings")]
        [SerializeField] GameObject prefab;
        [SerializeField] int prefabChildCount;

        [Space]

        [Header("Grid Info")]
        [SerializeField] bool enforceRowCount;
        [SerializeField] bool enforceColumnCount;
        [SerializeField] int maxRows;
        [SerializeField] int maxColumns;
        [SerializeField] Sprite[] spriteLayout;
        //[SerializeField] Sprite[] singleRowSpriteLayout;
        //[SerializeField] Sprite[] singleColSpriteLayout;

        int prevMaxRows;
        int prevMaxColumns;

        [Space]

        [Header("Spacing")]
        [SerializeField] Vector2 cellSize;
        [SerializeField] Vector2 spacing;

        [Space]

        [Header("Padding")]
        [SerializeField] int right;
        [SerializeField] int left;
        [SerializeField] int up;
        [SerializeField] int down;

        [Space]

        [Header("Debugging")]
        [SerializeField] int colIndex;
        [SerializeField] int rowIndex;
        [SerializeField] Transform[] childrenTransforms = new Transform[0];

        void Update()
        {

        }

        private void OnValidate()
        {
            if (!isActiveAndEnabled)
                return;

            if (!Application.isEditor)
                return;

            if (Application.isPlaying)
                return;

            UpdateLayout(maxRows, maxColumns);
        }

        public void UpdateLayout(int rows, int columns)
        {
            int count = 0;

            maxRows = rows;
            maxColumns = columns;

            if (sortType == GridChildSortType.ExistingChildren)
            {
                var transforms = GetComponentsInChildren<Transform>();
                if (transforms.Length == 0)
                    return;

                if (transforms.Length != childrenTransforms.Length)
                {
                    childrenTransforms = transforms.Where(i => i.parent == transform).ToArray();
                }

                count = childrenTransforms.Length;
            }
            else if (sortType == GridChildSortType.SpawnPrefabs)
            {
                count = this.prefabChildCount;

                {
                    var children = GetComponentsInChildren<Transform>();
                    children.Each((transform) =>
                    {
                        StartCoroutine(Destroy(transform.gameObject));
                    });

                    childrenTransforms = childrenTransforms.Clear();

                    for (int x = 0; x < count; x++)
                    {
                        var newObject = Instantiate(prefab);
                        newObject.transform.parent = transform;

                        childrenTransforms = childrenTransforms.AddItem(newObject.transform);
                    }
                }
            }

            if (count == 0)
                return;

            if (enforceRowCount && maxColumns != prevMaxColumns)
                maxRows = count / maxColumns;
            else if (enforceColumnCount && maxRows != prevMaxRows)
                maxColumns = count / maxRows;

            prevMaxRows = maxRows;
            prevMaxColumns = maxColumns;

            float modX = maxColumns % 2 == 0 ? (maxColumns / 2) - 0.5f : maxColumns / 2;
            float modY = maxRows % 2 == 0 ? (maxRows / 2) - 0.5f : maxRows / 2;

            colIndex = 0;
            rowIndex = 0;
            for (int x = 0; x < count; x++)
            {
                if (colIndex >= maxColumns)
                {
                    colIndex = 0;
                    rowIndex++;
                }

                childrenTransforms[x].localPosition = new Vector2(cellSize.x * (colIndex - modX), cellSize.y * (rowIndex - modY)) + new Vector2(spacing.x * colIndex, spacing.y * rowIndex);

                colIndex++;
            }

            if (spriteLayout.Length > 1)
            {
                colIndex = 0;
                rowIndex = 0;
                for (int x = 0; x < count; x++)
                {
                    if (colIndex >= maxColumns)
                    {
                        colIndex = 0;
                        rowIndex++;
                    }

                    SetSpriteWithLayout(childrenTransforms[x].GetComponent<SpriteRenderer>(), colIndex, rowIndex);

                    //if (colIndex == 0)
                    //{
                    //    if (rowIndex == 0)
                    //        childrenTransforms[x].GetComponent<SpriteRenderer>().sprite = spriteLayout[6];
                    //    else if (rowIndex < maxRows && rowIndex != maxRows - 1)
                    //        childrenTransforms[x].GetComponent<SpriteRenderer>().sprite = spriteLayout[3];
                    //    else if (rowIndex == maxRows - 1)
                    //        childrenTransforms[x].GetComponent<SpriteRenderer>().sprite = spriteLayout[0];
                    //}
                    //else if (colIndex < maxColumns && colIndex != maxColumns - 1)
                    //{
                    //    if (rowIndex == 0)
                    //        childrenTransforms[x].GetComponent<SpriteRenderer>().sprite = spriteLayout[7];
                    //    else if (rowIndex < maxRows && rowIndex != maxRows - 1)
                    //        childrenTransforms[x].GetComponent<SpriteRenderer>().sprite = spriteLayout[4];
                    //    else if (rowIndex == maxRows - 1)
                    //        childrenTransforms[x].GetComponent<SpriteRenderer>().sprite = spriteLayout[1];
                    //}
                    //else if (colIndex == maxColumns - 1)
                    //{
                    //    if (rowIndex == 0)
                    //        childrenTransforms[x].GetComponent<SpriteRenderer>().sprite = spriteLayout[8];
                    //    else if (rowIndex < maxRows && rowIndex != maxRows - 1)
                    //        childrenTransforms[x].GetComponent<SpriteRenderer>().sprite = spriteLayout[5];
                    //    else if (rowIndex == maxRows - 1)
                    //        childrenTransforms[x].GetComponent<SpriteRenderer>().sprite = spriteLayout[2];
                    //}

                    colIndex++;
                }
            }
        }

        private void SetSpriteWithLayout(SpriteRenderer spriteRenderer, int colIndex, int rowIndex)
        {
            if (colIndex == 0)
            {
                if (rowIndex == 0)
                    spriteRenderer.sprite = spriteLayout[6];
                else if (rowIndex < maxRows && rowIndex != maxRows - 1)
                    spriteRenderer.sprite = spriteLayout[3];
                else if (rowIndex == maxRows - 1)
                    spriteRenderer.sprite = spriteLayout[0];
            }
            else if (colIndex < maxColumns && colIndex != maxColumns - 1)
            {
                if (rowIndex == 0)
                    spriteRenderer.sprite = spriteLayout[7];
                else if (rowIndex < maxRows && rowIndex != maxRows - 1)
                    spriteRenderer.sprite = spriteLayout[4];
                else if (rowIndex == maxRows - 1)
                    spriteRenderer.sprite = spriteLayout[1];
            }
            else if (colIndex == maxColumns - 1)
            {
                if (rowIndex == 0)
                    spriteRenderer.sprite = spriteLayout[8];
                else if (rowIndex < maxRows && rowIndex != maxRows - 1)
                    spriteRenderer.sprite = spriteLayout[5];
                else if (rowIndex == maxRows - 1)
                    spriteRenderer.sprite = spriteLayout[2];
            }
        }

        IEnumerator Destroy(GameObject go)
        {
            yield return null;
            if (go != gameObject)
                DestroyImmediate(go);
        }
    }

    public enum GridChildSortType
    {
        None = 0,
        SpawnPrefabs = 1,
        ExistingChildren = 2,
    }
}