using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RobbysUtils;
using System;

public class TargetLocator : MonoBehaviour
{
    [SerializeField] Transform[] targets;

    void Update()
    {
        var copy = targets;
        foreach (Transform t in copy)
        {
            if (!t.parent.gameObject.activeInHierarchy)
                targets = targets.RemoveItem(t);
        }
    }

    public bool ContainsTarget(Transform target)
    {
        return targets.HasItem(target);
    }

    public bool ContainsTargetAsParent(Transform target)
    {
        foreach(var locatedTarget in targets)
        {
            if (locatedTarget.parent == target)
                return true;
        }

        return false;
    }

    internal bool HasTargetWithinRange()
    {
        return targets.Length > 0;
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (!targets.HasItem(collision.transform))
            targets = targets.AddItem(collision.transform);
    }

    internal Transform GetTarget()
    {
        if (targets.Length == 0)
            return null;
        else
            return targets[0];
    }

    internal Transform GetClosestTarget(Vector3 position)
    {
        if (targets.Length == 0)
            return null;

        var closestTarget = targets[0];
        var closestDistance = RobbysUtils.TransformUtils.GetDistance(targets[0].position, position);
        foreach (var target in targets)
        {
            var dist = RobbysUtils.TransformUtils.GetDistance(target.position, position);
            if (closestDistance > dist)
            {
                closestDistance = dist;
                closestTarget = target;
            }
        }

        return closestTarget;
    }

    internal Transform[] GetAllTargets()
    {
        return targets;
    }

    internal Transform[] GetNTargets(int n)
    {
        Transform[] result = new Transform[0];
        for (int i = 0; i < (targets.Length < n ? targets.Length : n); i++)
        {
            result = result.AddItem(targets[i]);
        }
        return result;
    }

    internal Transform[] GetNewNTargets(int n, Transform[] targetsToCheck)
    {
        Transform[] result = new Transform[0];
        for (int i = 0; i < targets.Length; i++)
        {
            if (!targetsToCheck.HasItem(targets[i].transform))
                result = result.AddItem(targets[i]);

            if (result.Length == n)
                break;
        }

        return result;
    }

    void OnTriggerExit2D(Collider2D collision)
    {
        if (targets.HasItem(collision.transform))
            targets = targets.RemoveItem(collision.transform);
    }
}
