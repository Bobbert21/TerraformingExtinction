using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrawLineToTarget : MonoBehaviour
{
    [SerializeField] TargetLocator targetLocator;
    [SerializeField] Transform target;

    void Start()
    {
        
    }

    void Update()
    {
        target = targetLocator.GetTarget();
    }

    private void OnDrawGizmos()
    {
        if (target != null)
            Gizmos.DrawLine(transform.position, target.position);
    }
}
