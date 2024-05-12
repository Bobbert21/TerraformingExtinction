using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.GraphicsBuffer;

public class ProjectileStatsContainer : MonoBehaviour
{
    [Header("Linked Components")]
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private BoxCollider2D boxCollider2D;
    [SerializeField] private CircleCollider2D circleCollider2D;
    [SerializeField] private CapsuleCollider2D capsuleCollider2D;
    [SerializeField] private Globals globals;

    [Header("Variables From Object Shooting This")]
    [SerializeField] private int projectileStatID;
    [SerializeField] private Vector2 startPos;
    [SerializeField] private Vector2 endPos;
    [SerializeField] private LayerMask projecileLayerMask;

    [Header("Debugging")]
    [SerializeField] private bool activate;
    [SerializeField] private bool runCode;
    [SerializeField] private int amountOfPiercesLeft;
    [SerializeField] private bool deactivateObject;
    [SerializeField] private float timer;
    //[SerializeField] private Vector2 originalPosition;
    [SerializeField] private float time;
    [SerializeField] private float timer2 = 3;
    
    // This uses cutom ReadOnly attribute to allow seeing in Inspector but not allow changing values
    [ReadOnly] [SerializeField] private ProjectileStats localizedProjectileStats;


    public int ProjectileStatID { get => projectileStatID; set => projectileStatID = value; }
    public Vector2 StartPos { get => startPos; set => startPos = value; }
    public Vector2 EndPos { get => endPos; set => endPos = value; }
    public LayerMask ProjecileLayerMask { get => projecileLayerMask; set => projecileLayerMask = value; }

    public void Awake()
    {
        //originalPosition = transform.position;

        //startPos = transform.position;
        //endPos = transform.position + new Vector3(4, 3);
    }

    public void OnEnable()
    {
        localizedProjectileStats = Globals.ProjectileStatsScriptableObject[ProjectileStatID];

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

        gameObject.layer = Mathf.RoundToInt(Mathf.Log(projecileLayerMask.value, 2));
        spriteRenderer.sprite = localizedProjectileStats.ProjectileSprite;
        amountOfPiercesLeft = localizedProjectileStats.Pierce;

        activate = true;
        deactivateObject = false;
    }

    Vector2 animationEndPoint;
    Vector2 gameEndPointAdjusted;
    float angle;
    float cosAngle;
    float sinAngle;
    Matrix2x2 rotationMatrix;
    Matrix2x2 scalingMatrix;
    public void Update()
    {
        if (deactivateObject)
        {
            gameObject.SetActive(false);
            return;
        }

        if (activate)
        {
            timer = localizedProjectileStats.LifeSpan;
            time = 0;
            transform.position = startPos;

            #region Vector Translation Code
            Debug.Log(endPos);
            animationEndPoint = new Vector2(1, localizedProjectileStats.MovementPath.Evaluate(1));
            gameEndPointAdjusted = endPos - startPos;
            
            //angle of game start and end pos
            float gameAngle = Mathf.Atan2(gameEndPointAdjusted.y, gameEndPointAdjusted.x);
            //angle of anim start and end pos
            float animAngle = Mathf.Atan2(animationEndPoint.y, animationEndPoint.x);
            //find how much you need to turn from anim to game
            angle = gameAngle- animAngle;

            //use these values for rotation matrix (part of formula)
            cosAngle = Mathf.Cos(angle);
            sinAngle = Mathf.Sin(angle);
            Debug.Log("Angle: " + angle + " cos: " + cosAngle + " sin: " + sinAngle);
            //animation curve needs to be rotated this much
            rotationMatrix = new Matrix2x2(cosAngle, -sinAngle, sinAngle, cosAngle);
            //find the scaling matrix
            //First find what the end point will be when rotated
            Vector2 rotatedAnimEndPoint = rotationMatrix * animationEndPoint;
            //Second: Now know how much to scale by to get to the game adjusted end point (when start is at 0,0). This is scaling matrix

            Debug.Log("rotated animation end point: " + rotatedAnimEndPoint);
            Debug.Log("game adjusted end point: " + gameEndPointAdjusted);
            //Check how big the game end point magnitude is compared to the rotated animation end point. if anim magnitude = 0, then it means you aren't moving projectile
            float scaling = rotatedAnimEndPoint.magnitude == 0? 0f: gameEndPointAdjusted.magnitude / rotatedAnimEndPoint.magnitude;
            Debug.Log(scaling);
            scalingMatrix = new Matrix2x2(scaling, 0, 0, scaling);


            //float x_scaling = rotatedAnimEndPoint.x == 0f ? 1f:  gameEndPointAdjusted.x / rotatedAnimEndPoint.x;
            //float y_scaling = rotatedAnimEndPoint.y == 0f ? 1f : gameEndPointAdjusted.y / rotatedAnimEndPoint.y;
            #endregion

            //spriteRenderer.enabled = true;

            runCode = true;
            activate = false;
        }

        if (runCode)
        {
            time += Time.deltaTime;
            
            Vector2 animationNextPoint = new Vector2(time, localizedProjectileStats.MovementPath.Evaluate(time));
            //rotated animNextPoint to fit the game curve and then scale it. Add the startpos since we adjusted game curve to assume it starts at 0,0
            Vector2 gameNextPos = scalingMatrix * (rotationMatrix * animationNextPoint) + startPos;
            transform.position = Vector2.Lerp(transform.position, gameNextPos, time);

            if (time >= 1)
            {
                time = 1;
                runCode = false;
                deactivateObject = true;
                return;
            }


            if (localizedProjectileStats.LifeSpan > 0)
            {
                if (timer <= 0)
                {
                    //spriteRenderer.enabled = false;
                    runCode = false;
                    deactivateObject = true;

                    timer = localizedProjectileStats.LifeSpan;
                }
                else
                    timer -= Time.deltaTime;
            }
        }
    }

    private struct Matrix2x2
    {
        public float m00, m01, m10, m11;
        public Matrix2x2(float m00, float m01, float m10, float m11)
        {
            this.m00 = m00; 
            this.m01 = m01;
            this.m10 = m10;
            this.m11 = m11;
        }
        
        public static Vector2 operator*(Matrix2x2 matrix, Vector2 vector)
        {
            return new Vector2
                (
                    matrix.m00*vector.x + matrix.m01*vector.y,
                    matrix.m10*vector.x + matrix.m11 * vector.y
                );
        }
    }

    public void OnTriggerEnter2D(Collider2D collision)
    {
        print("HIT " + collision.GetComponentInParent<Health>());

        if (collision.transform.parent.TryGetComponent(out Health health))
        {
            health.TakeDamage(localizedProjectileStats.Damage, 0, out bool isImmune);
            amountOfPiercesLeft--;

            if (amountOfPiercesLeft <= 0)
            {
                runCode = false;
                deactivateObject = true;
            }
        }
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
