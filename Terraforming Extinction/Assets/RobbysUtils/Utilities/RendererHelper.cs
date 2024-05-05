//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;

//[ExecuteInEditMode]
//public class RendererHelper : MonoBehaviour
//{
//    [SerializeField] Renderer renderer;

//    [SerializeField] int sortingLayerId;
//    [SerializeField] int sortingOrderId;

//    private void OnValidate()
//    {
//        renderer.sortingLayerName = SortingLayer.layers[sortingLayerId].name;
//        renderer.sortingOrder = sortingOrderId;
//    }

//    void Start()
//    {
//        if (renderer == null)
//            renderer = GetComponent<Renderer>();

//    }

//    void Update()
//    {
        
//    }
//}
