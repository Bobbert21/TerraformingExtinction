using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;

public class ProjectileStatsContainer : MonoBehaviour
{
    [SerializeField] private int projectileStatID;

    [Header("Linked Components")]
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private BoxCollider2D boxCollider2D;
    [SerializeField] private CircleCollider2D circleCollider2D;
    [SerializeField] private CapsuleCollider2D capsuleCollider2D;
    [SerializeField] private Globals globals;

    [Header("Variables From Object Shooting This")]
    [SerializeField] private Vector2 startPos;
    [SerializeField] private Vector2 endPos;

    [Header("Debugging")]
    [SerializeField] private bool activate;
    [SerializeField] private bool runCode;
    [SerializeField] private float timer;
    [SerializeField] private Vector2 originalPosition;
    [SerializeField] private float time;
    [SerializeField] private float timer2 = 3;
    
    // This uses cutom ReadOnly attribute to allow seeing in Inspector but not allow changing values
    [ReadOnly] [SerializeField] private ProjectileStats localizedProjectileStats;


    public Vector2 StartPos { get => startPos; set => startPos = value; }
    public Vector2 EndPos { get => endPos; set => endPos = value; }


    public void Awake()
    {
        originalPosition = transform.position;

        //startPos = transform.position;
        //endPos = transform.position + new Vector3(4, 3);
    }

    public void OnEnable()
    {
        localizedProjectileStats = globals.ProjectileStatsScriptableObjectInstance[projectileStatID];

        if (localizedProjectileStats == null)
        {
            Debug.LogError("Could not find Projectile Stats");
            return;
        }

        switch (localizedProjectileStats.ColliderSettings.Collider2DType)
        {
            case Collider2DType.Box:
                boxCollider2D.enabled = true;
                circleCollider2D.enabled = false;
                capsuleCollider2D.enabled = false;

                boxCollider2D.size = localizedProjectileStats.ColliderSettings.Dimensions;
                boxCollider2D.offset = localizedProjectileStats.ColliderSettings.Offset;
                break;
            case Collider2DType.Circle:
                boxCollider2D.enabled = false;
                circleCollider2D.enabled = true;
                capsuleCollider2D.enabled = false;

                circleCollider2D.radius = localizedProjectileStats.ColliderSettings.Radius;
                circleCollider2D.offset = localizedProjectileStats.ColliderSettings.Offset;
                break;
            case Collider2DType.Capsule:
                boxCollider2D.enabled = false;
                circleCollider2D.enabled = false;
                capsuleCollider2D.enabled = true;

                capsuleCollider2D.size = localizedProjectileStats.ColliderSettings.Dimensions;
                capsuleCollider2D.direction = localizedProjectileStats.ColliderSettings.CapsuleDirection2D;
                capsuleCollider2D.offset = localizedProjectileStats.ColliderSettings.Offset;
                break;
        }

        spriteRenderer.sprite = localizedProjectileStats.ProjectileSprite;
    }

    Vector2 animationPointEnd;
    Vector2 pointMultiplication;

    public void Update()
    {
        if (activate)
        {
            timer = localizedProjectileStats.LifeSpan;
            time = 0;

            animationPointEnd = new Vector2(1, localizedProjectileStats.MovementPath.Evaluate(1));
            pointMultiplication = new Vector2((endPos - startPos).x / animationPointEnd.x, (endPos - startPos).y / animationPointEnd.y);

            transform.position = startPos;
            //spriteRenderer.enabled = true;

            runCode = true;
            activate = false;
        }

        if (runCode)
        {
            // Point from anim curve
            //var animationPointStart = new Vector2(time, localizedProjectileStats.MovementPath.Evaluate(time));
            
            
            time += Time.deltaTime;

            Vector2 animationNextPoint = new Vector2(time, localizedProjectileStats.MovementPath.Evaluate(time));
            Vector2 gameNextPos = ((animationNextPoint) * pointMultiplication) + startPos;
            
            //transform.position = Vector2.Lerp(startPos, gameEndPos, time);
            transform.position = Vector2.Lerp(transform.position, gameNextPos, Time.deltaTime);

            if (time >= 1)
            {
                //runCode = false;
                time = 1;
                return;
            }


            if (localizedProjectileStats.LifeSpan > 0)
            {
                if (timer <= 0)
                {
                    //spriteRenderer.enabled = false;
                    runCode = false;

                    timer = localizedProjectileStats.LifeSpan;
                }
                else
                    timer -= Time.deltaTime;
            }
        }
    }

    public void OnTriggerEnter2D(Collider2D collision)
    {
        print("HIT " + collision.gameObject.name);
        //spriteRenderer.enabled = false;
        runCode = false;
    }

    public void OnDrawGizmos()
    {
        Gizmos.DrawLine(startPos, endPos);
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(startPos, 0.5f);
        Gizmos.color = Color.blue;
        Gizmos.DrawSphere(endPos, 0.5f);
    }
}
