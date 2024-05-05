using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
[RequireComponent(typeof(SpriteRenderer))]
public class UpdateOrder : MonoBehaviour
{
    public int BaseOrder { get { return baseOrder; } set { baseOrder = value; } }

    [SerializeField] int baseOrder;
    SpriteRenderer cachedSpriteRenderer;

    void Start()
    {
        cachedSpriteRenderer = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        cachedSpriteRenderer.sortingOrder = baseOrder - Mathf.RoundToInt(transform.position.y * 100);
    }
}