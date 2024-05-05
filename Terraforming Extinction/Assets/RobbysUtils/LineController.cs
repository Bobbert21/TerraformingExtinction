using RobbysUtils;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LineController : MonoBehaviour
{
    [SerializeField] LineRenderer lineRenderer;
    [SerializeField] Texture[] textures;

    int animationStep;

    [SerializeField] float fps = 30f;
    [SerializeField] float fpsCounter;

    [SerializeField] Transform[] targets = new Transform[0];

    void OnEnable()
    {
        
    }

    void Update()
    {
        int index = 0;
        foreach (var target in targets)
        {
            lineRenderer.SetPosition(index, target.position.WithZ(0));
            index++;
        }

        fpsCounter += Time.deltaTime;
        if (fpsCounter >= 1f / fps)
        {
            animationStep++;
            if (animationStep >= textures.Length)
                animationStep = 0;

            lineRenderer.material.SetTexture("_MainTex", textures[animationStep]);

            fpsCounter = 0f;
        }
    }

    public void AssignTargets(Transform[] targets)
    {
        lineRenderer.positionCount = targets.Length;

        int index = 0;
        foreach (var target in targets)
        { 
            lineRenderer.SetPosition(index, target.position.WithZ(0));
            index++;
        }

        this.targets = targets;
    }
}
